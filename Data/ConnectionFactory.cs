using Data.Interfaces;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        public DbConnection Build(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
