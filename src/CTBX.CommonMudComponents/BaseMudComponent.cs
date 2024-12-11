using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace CTBX.CommonMudComponents;

public class BaseMudComponent : ComponentBase
{
    [Inject]
    private ILoggerFactory LoggerFactory { get; set; } = null!;
    private ILogger? _logger;
    protected ILogger Logger => _logger ??= LoggerFactory.CreateLogger(GetType());


    [Inject]
    public ISnackbar? Snackbar { get; set; }

    protected override void OnInitialized()
    {
        Snackbar!.Configuration.ShowCloseIcon = true;
        Snackbar.Configuration.ShowTransitionDuration = 500;
        Snackbar.Configuration.HideTransitionDuration = 200;
        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
        Snackbar.Configuration.VisibleStateDuration = 2000;

        base.OnInitialized();
    }

    protected Task NotifyError(string message)
    {
        Snackbar?.Add(message, Severity.Error);
        return Task.CompletedTask;
    }

    protected Task NotifyWarning(string message)
    {
        Snackbar?.Add(message, Severity.Warning);
        return Task.CompletedTask;
    }

    protected Task NotifyInfo(string message)
    {
        Snackbar?.Add(message, Severity.Info);
        return Task.CompletedTask;
    }

    protected Task NotifySuccess(string message)
    {
        Snackbar?.Add(message, Severity.Success);
        return Task.CompletedTask;
    }

    protected async Task OnHandleOperation<T>(Func<T, bool> condition, Func<Task<T>> operation, string errMessage, string? successMssage = null)
    {
        try
        {
            var result = await operation();

            var success = condition(result);

            if (success && successMssage!.IsNotEmpty())
                await NotifySuccess(successMssage!);

            if (!success)
                await NotifyError(errMessage);

        }
        catch (Exception ex)
        {
            await NotifyError(errMessage);
            Logger.LogError(ex, ex.Message);
        }
    }

    protected async Task OnHandleOperation(Func<Task> operation, string errMessage, string? successMssage = null)
    {
        try
        {
            await operation();
            await NotifySuccess(successMssage!);
        }
        catch (Exception ex)
        {
            await NotifyError(errMessage);
            Logger.LogError(ex, ex.Message);
        }
    }

    protected async Task OnHandleOperation(Func<bool> condition, Func<Task> operation, string errMessage, string? successMssage = null)
    {
        try
        {
            await operation();

            var success = condition();

            if (success && successMssage!.IsNotEmpty())
                await NotifySuccess(successMssage!);

            if (!success)
                await NotifyError(errMessage);

        }
        catch (Exception ex)
        {
            await NotifyError(errMessage);
            Logger.LogError(ex, ex.Message);
        }
    }

    protected async Task OnHandleOperation<T>(Func<T, bool> condition,
                                              Func<Task<T>> operation,
                                              Action? OnFailure = null,
                                              Action? OnSuccess = null)
    {
        try
        {
            var result = await operation();

            var success = condition(result);

            if (success && OnSuccess.IsNotNull())
                OnSuccess!();

            if (!success && OnFailure.IsNotNull())
                OnFailure!();

        }
        catch (Exception ex)
        {
            await NotifyError("Operation failed");
            Logger.LogError(ex, ex.Message);
        }
    }

    protected async Task OnHandleOperation<T>(Func<T, bool> condition,
                                          Func<Task<T>> operation,
                                          Func<Task>? OnFailure = null,
                                          Func<Task>? OnSuccess = null)
    {
        try
        {
            var result = await operation();

            var success = condition(result);

            if (success && OnSuccess.IsNotNull())
                await OnSuccess!();

            if (!success && OnFailure.IsNotNull())
                await OnFailure!();

        }
        catch (Exception ex)
        {
            await NotifyError("Operation failed");
            Logger.LogError(ex, ex.Message);
        }
    }
}
