using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventuous.Projections.MongoDB.Tools;

namespace MinimalApiArchitecture.Application
{
    public record ReadModelDocument : ProjectedDocument
    {
        public ReadModelDocument(string id ) : base(id) { }
        public int EmployeeId { get; set; }
        public int? SupervisorId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public DateTimeOffset ScheduledAt { get; set; }
        public DateTimeOffset RequestedAt { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? RejectedAt { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;


    }
}
