using System.Linq;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using Data.Models.Products;
using Data.Interfaces;

namespace Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public ProductsRepository(IConnectionFactory connectionFactory) =>
            _connectionFactory = connectionFactory;

        public async Task<IList<Product>> GetProductsAsync(int organizationId)
        {
            using var connection = _connectionFactory.Build();
            var query = await connection.QueryAsync<Product>(
                sql: "call items_rest_by_organization(?)",
                param: new { organizationid = organizationId });

            return query
                .Where(p => p.StoreType == StoreType.Shop || p.StoreType == StoreType.Warehouse)
                .ToList();
        }
    }
}
