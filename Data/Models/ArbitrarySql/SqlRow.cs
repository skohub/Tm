using System.Collections.Generic;

namespace Data.Models.ArbitrarySql
{
    public class SqlRow
    {
        public IDictionary<string, object> Columns { get; set; } = new Dictionary<string, object>();
    }
}
