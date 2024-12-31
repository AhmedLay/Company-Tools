using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventuous.Projections.MongoDB.Tools;

namespace MinimalApiArchitecture.Application
{
    public record VacationDocument : ProjectedDocument
    {
        public VacationDocument(string id ) : base(id) { }
        public int EmployeeId { get; init; }
        public DateTimeOffset From { get; init; }
        public DateTimeOffset To { get; init; }
        public DateTimeOffset ScheduledAt { get; init; }
        public string Comment { get; init; } = string.Empty;

    }
}
