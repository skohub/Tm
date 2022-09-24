using Data.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

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
