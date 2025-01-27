using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// change vacation 
    public record ChangeVacation(
    string Id,
    int EmployeeId,
    DateTimeOffset From,
    DateTimeOffset To,
    string Comment);
