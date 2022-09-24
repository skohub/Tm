using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class SqlRow
    {
        public IDictionary<string, object> Columns { get; set; } = new Dictionary<string, object>();
    }
}
