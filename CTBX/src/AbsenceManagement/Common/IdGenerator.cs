using Eventuous;

namespace CTBX.AbsenceManagement.Core.Common;

public record Id<T> : Id
{
    public Id() : base($"{typeof(T).Name}-{Guid.NewGuid()}")
    {
        
    }

    public Id(string Value) : base(Value) { }
}
