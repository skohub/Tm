using System.Collections.Generic;
using System.Linq;
using Data.Models.Products;
using WcSync.Model.Entities;

namespace WcSync.Sync.Extensions;
public static class DbProductExtensions
{
    public static IList<DbProduct> Map(this IEnumerable<Product> products) => products
        .GroupBy(
            product => product.ProductId, 
            product => product,
            (id, products) => new DbProduct
            {
                Id = id,
                Name = products.First(p => p.ProductId == id).ProductName,
                Availability = products
                    .Select(p => new Store
                    {
                        Name = p.StoreName,
                        Quantity = p.Quantity,
                        Price = p.Price,
                        Type = (WcSync.Model.Entities.StoreType) p.StoreType,
                    })
                    .ToList(),
            })
        .ToList();
}