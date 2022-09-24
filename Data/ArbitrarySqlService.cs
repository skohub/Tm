using Dapper;
using Data.Interfaces;
using Data.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tm.Data.Models;

namespace Data
{
    public class ArbitrarySqlService : IArbitrarySqlService
    {
        private readonly IConnectionFactory _connectionFactory;

        public ArbitrarySqlService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public dynamic Select(string connectionString, string sql, object param = null)
        {
            using (var con = _connectionFactory.Build(connectionString))
            {
                return con.Query(sql, param).ToList();
            }
        }
    }
}
