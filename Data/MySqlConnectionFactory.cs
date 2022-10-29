using Data.Interfaces;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;

namespace Data
{
    public class MySqlConnectionFactory : IConnectionFactory
    {
        private readonly Dictionary<string, string> _connectionStrings;

        public MySqlConnectionFactory(Dictionary<string, string> connectionStrings) =>
            _connectionStrings = connectionStrings;

        public DbConnection Build(string connectionStringName) =>
            new MySqlConnection(_connectionStrings[connectionStringName]);
    }
}
