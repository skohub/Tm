using System.Linq;
using System.Collections.Generic;
using WcSync.Model.Entities;
using Microsoft.Extensions.Configuration;
using Dapper;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace WcSync.Db
{
    public class DbProductRepository : IDbProductRepository
    {
        private IConfiguration _configuration;
        private MySqlConnection _connection;

        private MySqlConnection Connection => 
            _connection ?? (_connection = new MySqlConnection(_configuration["ConnectionString"]));

        public DbProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<DbProduct>> GetRecentlyUpdatedProductsAsync()
        {
            // get a flat list of products and group by productid
            var query = await Connection.QueryAsync<FlatProductDto>(
                sql: "call recently_updated_items(?)",
                param: new { days_offset = 1 });
            return query
                .GroupBy(
                    flatProduct => flatProduct.ProductId, 
                    flatProduct => flatProduct,
                    (id, flatProducts) => new DbProduct
                    {
                        Id = id,
                        Name = flatProducts.First(p => p.ProductId == id).ProductName,
                        Availability = flatProducts
                            .Select(p => new Store
                            {
                                Name = p.StoreName,
                                Quantity = p.Quantity,
                                Type = p.StoreType,
                            })
                            .ToList(),
                    })
                .ToList();
        }

        public async Task<List<DbProduct>> GetProductsAsync()
        {
            // get a flat list of products and group by productid
            var query = await Connection.QueryAsync<ItemRestDto>(sql: "call items_rest(0)");
            return query
                .Where(p => p.StoreType == StoreType.Shop || p.StoreType == StoreType.Warehouse)
                .GroupBy(
                    product => product.ItemID, 
                    product => product,
                    (id, products) => new DbProduct
                    {
                        Id = id,
                        Name = products.First(p => p.ItemID == id).i_n,
                        Availability = products
                            .Select(p => new Store
                            {
                                Name = p.name,
                                Quantity = p.summ,
                                Price = p.price,
                                Type = p.StoreType,
                            })
                            .ToList(),
                    })
                .ToList();
        }
    }
}
