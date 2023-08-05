using System;
using System.Linq;
using WcSync.Wc;
using System.Threading.Tasks;
using WcSync.Model;
using WcSync.Model.Entities;
using System.Collections.Generic;
using System.Threading;
using Serilog;
using Data.Interfaces;
using WcSync.Sync.Extensions;

namespace WcSync.Sync
{
    public class SyncService : ISyncService
    {
        private readonly IWcProductService _wcProductService;
        private readonly IProductsRepository _productsRepository;
        private readonly IPriceCalculator _priceCalculator;
        private readonly int _organizationId;
        private readonly ILogger _logger;

        public SyncService(
            IWcProductService wcProductService, 
            IProductsRepository productsRepository,
            IPriceCalculator priceCalculator,
            int organizationId,
            ILogger logger)
        {
            _wcProductService = wcProductService;
            _productsRepository = productsRepository;
            _priceCalculator = priceCalculator;
            _organizationId = organizationId;
            _logger = logger;
        }

        public async Task UpdateAllProductsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productsRepository.GetProductsAsync(_organizationId);
                var dbProducts = products.Map();
                var wcProducts = await _wcProductService.GetProductsAsync(cancellationToken);
                var wcProductsToUpdate = new List<WcProduct>();

                foreach (var wcProduct in wcProducts)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!int.TryParse(wcProduct.Sku, out int productId))
                    {
                        _logger.Warning("Invalid Sku {@WcProduct}", wcProduct);
                        continue;
                    }

                    var dbProduct = dbProducts.FirstOrDefault(p => p.Id == productId);
                    if (dbProduct == null) 
                    {
                        if (wcProduct.Quantity > 0 || wcProduct.StockStatus != Consts.UnavailableStatus)
                        {
                            wcProductsToUpdate.Add(SetUnavailableStatus(wcProduct));
                        }

                        continue;
                    }

                    if (ProductEquals(wcProduct, dbProduct) == false)
                    {
                        wcProductsToUpdate.Add(UpdateProductProperties(wcProduct, dbProduct));
                    }
                }

                if (wcProductsToUpdate.Any())
                {
                    await _wcProductService.UpdateProductsAsync(wcProductsToUpdate, cancellationToken);
                    _logger.Information("Updated {Count} products", wcProductsToUpdate.Count);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Something went wrong");
            }
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

        private WcProduct UpdateProductProperties(WcProduct wcProduct, DbProduct dbProduct) 
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
                Quantity = dbProduct.GetQuantity(),
                RegularPrice = price,
                SalePrice = salePrice,
            }; 
        }

        private WcProduct SetUnavailableStatus(WcProduct wcProduct)
        {
            _logger.Information("Updating product {Name} - {Sku} to \"{Status}\"",
                wcProduct.Name, wcProduct.Sku, Consts.UnavailableStatus);
            return new WcProduct
            {
                Id = wcProduct.Id,
                StockStatus = Consts.UnavailableStatus,
                Availability = string.Empty,
                Quantity = 0,
                RegularPrice = null,
                SalePrice = null,
            };
        }
    }
}