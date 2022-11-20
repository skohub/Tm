using System.Collections.Generic;

namespace Data.Models.ArbitrarySql
{
    public class SqlResult
    {
        public IList<SqlRow> Rows { get; set; } = new List<SqlRow>();
        
    }
}
