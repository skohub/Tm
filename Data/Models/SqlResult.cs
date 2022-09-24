using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class SqlResult
    {
        public IList<SqlRow> Rows { get; set; } = new List<SqlRow>();
        
    }
}
