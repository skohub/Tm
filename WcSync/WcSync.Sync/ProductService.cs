using System;
using System.Linq;
using Tm.WcSync.Wc;
using Tm.WcSync.Db;
using System.Threading.Tasks;
using Tm.WcSync.Model;
using Tm.WcSync.Model.Entities;
using System.Collections.Generic;
using System.Threading;
using Serilog;

namespace Tm.WcSync.Sync
{
    public class ProductService : IProductService
    {
        private readonly IWcProductService _wcProductService;
        private readonly IDbProductRepository _dbProductRepository;
        private readonly IPriceCalculator _priceCalculator;
        private readonly ILogger _logger;

        public ProductService(
            IWcProductService wcProductService, 
            IDbProductRepository dbProductRepository,
            IPriceCalculator priceCalculator,
            ILogger logger)
        {
            _wcProductService = wcProductService;
            _dbProductRepository = dbProductRepository;
            _priceCalculator = priceCalculator;
            _logger = logger;
        }

        public async Task UpdateAllProductsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var dbProducts = await _dbProductRepository.GetProductsAsync();
                var wcProducts = await _wcProductService.GetProductsAsync(cancellationToken);
                var updatedWcProducts = new List<WcProduct>();

                foreach (var wcProduct in wcProducts)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var dbProduct = dbProducts.FirstOrDefault(p => 
                        int.TryParse(wcProduct.Sku, out int id) && p.Id == id);
                    if (dbProduct == null) 
                    {
                        if (wcProduct.StockStatus != Consts.UnavailableStatus)
                        {
                            _logger.Information("Updating product {Name} - {Sku} to \"{Status}\"",
                                wcProduct.Name, wcProduct.Sku, Consts.UnavailableStatus);
                            updatedWcProducts.Add(SetUnavailableStatus(wcProduct));
                        }

                        continue;
                    }

                    if (ProductEquals(wcProduct, dbProduct) == false)
                    {
                        updatedWcProducts.Add(UpdateProduct(wcProduct, dbProduct));
                    }
                }

                if (updatedWcProducts.Any())
                {
                    await UpdateProductsBatchAsync(updatedWcProducts, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Something went wrong");
            }
        }

        public async Task ListProductsDicrepanciesAsync(CancellationToken cancellationToken)
        {
            var dbProducts = await _dbProductRepository.GetProductsAsync();
            var wcProducts = await _wcProductService.GetProductsAsync(cancellationToken);

            foreach (var wcProduct in wcProducts.Where(p => p.Availability?.Any() == true))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var dbProduct = dbProducts.FirstOrDefault(p => int.TryParse(wcProduct.Sku, out int id) && p.Id == id);
                if (dbProduct == null) continue;

                var (price, salePrice) = _priceCalculator.GetPrice(dbProduct);
                if (price == null) continue;

                if (wcProduct.SalePrice != salePrice)
                {
                    _logger.Information("{Name} - {Sku}. Site/database: {SalePrice} / {Price}",
                        wcProduct.Name, wcProduct.Sku, salePrice, price);
                }
            }

            var notFoundProducts = dbProducts
                .Where(dbProduct => !wcProducts.Any(wcProduct => int.TryParse(wcProduct.Sku, out int id) && dbProduct.Id == id))
                .ToList();

            var notFoundProductsStr = string.Join("\r\n", notFoundProducts.Select(product => $"{product.Name} - {product.Id}"));

            notFoundProducts.ForEach(product => 
                _logger.Information("Not found product: {Name} - {Id}", product.Name, product.Id));
        }

        private async Task UpdateProductsBatchAsync(List<WcProduct> wcProducts, CancellationToken cancellationToken)
        {
            var position = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var batch = wcProducts.Skip(position).Take(Consts.BatchSize).ToList();
                _logger.Information("Updating products {Position}-{Total}", position, position + batch.Count);
                await _wcProductService.UpdateProductsAsync(batch);
                position += batch.Count;
                await Task.Delay(Consts.RequestDelay, cancellationToken);
            }
            while (position < wcProducts.Count);

            _logger.Information("Updated {Count} products", wcProducts.Count);
        }

        private bool ProductEquals(WcProduct wcProduct, DbProduct dbProduct)
        {
            return 
                wcProduct.StockStatus == dbProduct.GetStockStatus() &&
                wcProduct.Availability == dbProduct.GetAvailability() &&
                ProductPriceEquals(wcProduct, dbProduct);
        }

        private bool ProductPriceEquals(WcProduct wcProduct, DbProduct dbProduct)
        {
            if (wcProduct.FixedPrice)
            {
                _logger.Information("Product {Name} - {Sku} has fixed price.", wcProduct.Name, wcProduct.Sku);

                return true;
            }

            (var price, var salePrice) = _priceCalculator.GetPrice(dbProduct);

            if (price == null) return true;

            return 
                wcProduct.RegularPrice == price && 
                (wcProduct.SalePrice ?? price) == salePrice;
        }

        private WcProduct UpdateProduct(WcProduct wcProduct, DbProduct dbProduct) 
        {
            var stockStatus = dbProduct.GetStockStatus();
            var availability = dbProduct.GetAvailability();
            (var price, var salePrice) = _priceCalculator.GetPrice(dbProduct);

            if (wcProduct.FixedPrice)
            {
                price = wcProduct.RegularPrice;
                salePrice = wcProduct.SalePrice;
            }

            _logger.Information(
                "Updating product {Name} - {Sku} from {StockStatus} - " +
                "\"{Availability}\" price: {RegularPrice:F0}/{SalePrice:F0} to " +
                "{StockStatus} - \"{Availability}\", Price: {Price:F0}/{SalePrice:F0}",
                wcProduct.Name, wcProduct.Sku, wcProduct.StockStatus, wcProduct.Availability, 
                wcProduct.RegularPrice, wcProduct.SalePrice, stockStatus, availability, price, salePrice);

            return new WcProduct
            {
                Id = wcProduct.Id,
                StockStatus = stockStatus,
                Availability = availability,
                RegularPrice = price,
                SalePrice = salePrice,
            }; 
        }

        private WcProduct SetUnavailableStatus(WcProduct wcProduct)
        {
            return new WcProduct
            {
                Id = wcProduct.Id,
                StockStatus = Consts.UnavailableStatus,
                Availability = string.Empty,
                RegularPrice = null,
                SalePrice = null,
            };
        }
    }
}