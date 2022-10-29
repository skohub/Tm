using Dapper;
using Data.Interfaces;

namespace Api.Service.Services
{
    public class ArbitrarySqlService : IArbitrarySqlService
    {
        private readonly IConnectionFactory _connectionFactory;

        public ArbitrarySqlService(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public dynamic Select(string connectionStringName, string sql, object? param = null)
        {
            using (var connection = _connectionFactory.Build(connectionStringName))
            {
                return con.Query(sql, param).ToList();
            }
        }
    }
}
