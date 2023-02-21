using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.QuerySupporter
{
    public class QuerySupporter
    {
        public string? Filter { get; set; }
        public string[]? FilterParams { get; set; }
        public string? OrderBy { get; set; }
        public int Skip { get; set; } = -1;
        public int Top { get; set; } = -1;
    }
}
