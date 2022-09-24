using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IArbitrarySqlService
    {
        dynamic Select(string connectionString, string sql, object param = null);
    }
}
