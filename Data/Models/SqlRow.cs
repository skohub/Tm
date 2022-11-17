using System.Collections.Generic;

namespace Data.Models
{
    public class SqlRow
    {
        public IDictionary<string, object> Columns { get; set; } = new Dictionary<string, object>();
    }
}
