using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Interfaces
{
    public interface IArbitrarySqlService
    {
        dynamic Select(string connectionStringName, string sql, object param = null);
    }
}
