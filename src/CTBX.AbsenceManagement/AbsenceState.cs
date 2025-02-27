using Eventuous;

namespace MinimalApiArchitecture.Application
{
    public record AbsenceState : State<AbsenceState>
    {
        public AbsenceStatus Status { get; private init; } = AbsenceStatus.Drafted;
        //on the record -> state changes 
        public AbsenceState()
        {
            On<VacationRequested>((state, evt) => state with { Status = AbsenceStatus.Requested });
            On<VacationApproved>((state, evt) => state with { Status = AbsenceStatus.VacationApproved });
            On<SickLeaveConfirmed>((state, evt) => state with { Status = AbsenceStatus.SickLeaveConfirmed });
        }
    }
    public enum AbsenceStatus
    {
        Drafted,
        Requested,
        VacationApproved,
        SickLeaveConfirmed,
        Rejected,
        Abondon
    }
}
