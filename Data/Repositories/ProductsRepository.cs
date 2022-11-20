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

        public async Task<IList<Product>> GetRecentlyUpdatedProductsAsync()
        {
            using var connection = _connectionFactory.Build();
            var query = await connection.QueryAsync<Product>(
                sql: "call recently_updated_items(?)",
                param: new { days_offset = 1 });

            return query.ToList();
        }

        public async Task<IList<ItemRest>> GetProductsAsync()
        {
            using var connection = _connectionFactory.Build();
            var query = await connection.QueryAsync<ItemRest>(sql: "call items_rest(0)");

            return query
                .Where(p => p.StoreType == StoreType.Shop || p.StoreType == StoreType.Warehouse)
                .ToList();
        }
    }
}
