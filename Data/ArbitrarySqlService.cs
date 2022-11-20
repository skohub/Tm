using System.Linq;
using Dapper;
using Data.Interfaces;

namespace Data
{
    public class ArbitrarySqlService : IArbitrarySqlService
    {
        private readonly IConnectionFactory _connectionFactory;

        public ArbitrarySqlService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public dynamic Select(string connectionStringName, string sql, object param = null)
        {
            using (var connection = _connectionFactory.Build(connectionStringName))
            {
                return connection.Query(sql, param).ToList();
            }
        }
    }
}
