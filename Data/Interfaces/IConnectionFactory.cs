using System.Data.Common;

namespace Data.Interfaces
{
    public interface IConnectionFactory
    {
        // The first connection string is used when connectionStringName is null
        DbConnection Build(string? connectionStringName = null);
    }
}