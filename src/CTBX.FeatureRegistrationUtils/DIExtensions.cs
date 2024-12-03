using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace CTBX.FeatureRegistrationUtils;
public sealed record RegistrationInvocation(string ClassName, string MethodName, IEnumerable<object> Parameters);

// this is
public static class DIExtensions
{



    public static void RegisterFeatureServices(this IServiceCollection services, Action<FeatureRegistryBuilder> config)
    {
        var builder = new FeatureRegistryBuilder();
        config(builder);
        RegisterFeatureServices(builder.Build());
    }

    public static void RegisterFeatureServices(ImmutableList<RegistrationInvocation> invocations)
    {
        invocations.CountBy(i => i.MethodName).GuardAgainst(counts => counts.Any(c => c.Value > 1), new InvalidOperationException("Invalid feature registrations, duplicate method names are not allowed even if the parameters differs"));

        var assembly = Assembly.GetEntryAssembly().GuardAgainstNull("assembly");
        assembly!
           .GetAssemblies()
           .SelectMany(a => a.GetTypes())
           .Where(t =>
                       t.IsStatic() &&
                       t.Name.Contains("FeatureRegistration") &&
                       t.GetMethods().Any(m => t.GetMethods().Any(m => invocations.Any(i => i.MethodName == m.Name)) && m.IsPublic && m.IsStatic)
                )
           .SelectMany(t => t.GetMethods()
                         .Where(m => t.GetMethods()
                                      .Any(m => invocations
                                                .Any(i => i.MethodName == m.Name)) && m.IsPublic && m.IsStatic))
           .ForEach(mi => mi?.Invoke(null, invocations.First(i => i.MethodName == mi.Name).Parameters.ToArray()));
    }

    //public void RegisterFeatureServices(this IServiceCollection services, string methodName, params object[] parameters)
    //{

    //    var assembly = Assembly.GetEntryAssembly();

    //    assembly
    //        .GetAssemblies()
    //        .SelectMany(a => a.GetTypes())
    //        .Where(t =>
    //                    t.IsStatic() &&
    //                    t.Name.Contains("FeatureRegistration") &&
    //                    t.GetMethods().Any(m => m.Name == methodName && m.IsPublic && m.IsStatic)
    //             )
    //        .Select(t => t.GetMethod(methodName))
    //        .ForEach(mi => mi?.Invoke(null, parameters));
    //}

    private static ImmutableHashSet<Assembly> GetAssemblies(this Assembly entryAssembly)
    {
        var context = DependencyContext.Load(entryAssembly)!;
        return

        context
            .RuntimeLibraries
            .SelectMany(library => library.GetDefaultAssemblyNames(context))
            .Select(TryLoadAssembly)
            .Where(assembly => assembly.IsNotNull())
            .ToImmutableHashSet()!;
    }

    private static Assembly? TryLoadAssembly(AssemblyName assemblyName)
    {
        try
        {
            return Assembly.Load(assemblyName);
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public sealed class FeatureRegistryBuilder
{
    private readonly Dictionary<string, ImmutableList<RegistrationInvocation>> _registrations;

    public FeatureRegistryBuilder()
    {
        _registrations = new();
    }

    public FeatureRegistryBuilder AddFeature<T>(Func<RegistrationConventionBuilder, ImmutableArray<RegistrationInvocation>> regFunc)
    {
        var typeName = typeof(T).FullName!;
        var builder = new RegistrationConventionBuilder();
        var registrations = regFunc(builder);
        if (!_registrations.TryGetValue(typeName, out var regs))
        {
            regs = [];
            regs = regs.AddRange(registrations);
            _registrations.Add(typeName, regs);
            return this;
        }

        regs = regs.AddRange(registrations);
        _registrations[typeName] = regs;

        return this;
    }

    internal ImmutableList<RegistrationInvocation> Build()
    {
        return _registrations.SelectMany(kv=> kv.Value).ToImmutableList() ?? [];
    }
}

public sealed class RegistrationConventionBuilder : IRegistrationClassInvocationBuilder,IRegistrationMethodInvocationBuilder
{
    private string _className = string.Empty;
    private readonly List<(string Method, IEnumerable<object> Parameters)> _methods = [];
    public IRegistrationMethodInvocationBuilder WithClass<T>()
        where T : class
    {
        typeof(T).GuardAgainst(t => !t.IsStatic(), $"{typeof(T).Name} is not a static type, only static types are allowrd.");
        _className = typeof(T).FullName!;
        return this;
    }

    public IRegistrationMethodInvocationBuilder WithMethod(string method, params IEnumerable<object> parameters)
    {
        _methods.Add((method, parameters));

        return this;
    }

    public ImmutableArray<RegistrationInvocation> Build()
    {
        return _methods
               .Select(m=> new RegistrationInvocation(_className,m.Method,m.Parameters))
               .ToImmutableArray();
    }
}


public interface IRegistrationClassInvocationBuilder
{
    IRegistrationMethodInvocationBuilder WithClass<T>() where T : class;
}

public interface IRegistrationMethodInvocationBuilder
{
    IRegistrationMethodInvocationBuilder WithMethod(string method, params IEnumerable<object> parameters);

    ImmutableArray<RegistrationInvocation> Build();
}
