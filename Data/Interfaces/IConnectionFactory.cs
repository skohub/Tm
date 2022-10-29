using System.Data.Common;

namespace Data.Interfaces
{
    public interface IConnectionFactory
    {
        DbConnection Build(string connectionStringName);
    }
}