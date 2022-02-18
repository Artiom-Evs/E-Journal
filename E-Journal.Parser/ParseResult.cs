using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Journal.Parser
{
    public class ParseResult
    {
        public string Name { get; set; } = String.Empty;
        public string? DateRange { get; init; }
        public DateTime[]? Days { get; init; }
        public string[][]? Timetable { get; init; }
        public int HashCode { get; set; }
        public Exception? Exception { get; init; }
    }
}
