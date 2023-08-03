using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;
using Product = WooCommerceNET.WooCommerce.v3.Product;
using ProductMeta = WooCommerceNET.WooCommerce.v2.ProductMeta;
using WcSync.Model.Entities;
using System.Net;
using WcSync.Model;
using System.Threading;
using Serilog;

namespace WcSync.Wc
{
    public class WcProductService : IWcProductService
    {
        private const string _availabilityMetaKey = "product_availability";
        private const string _fixedPriceMetaKey = "fixed_price";

        private readonly WcConfiguration _configuration;
        private readonly ILogger _logger;
        private WCObject _wcClient;
        private int _totalPages;

        public WcProductService(WcConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            _wcClient = Connect();
        }

        public async Task UpdateProductsAsync(List<WcProduct> products, CancellationToken cancellationToken)
        {
            var position = 0;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var batch = products.Skip(position).Take(Consts.BatchSize).ToList();
                _logger.Information("Updating products {Position}-{Total}", position, position + batch.Count);
                await UpdateProductRange(batch);
                position += batch.Count;
                await Task.Delay(Consts.RequestDelay, cancellationToken);
            }
            while (position < products.Count);
        }

        private async Task UpdateProductRange(List<WcProduct> products)
        {
            var batch = new ProductBatch()
            {
                update = products
                    .Select(x => new Product 
                    {
                        id = x.Id,
                        stock_status = x.StockStatus,
                        manage_stock = true,
                        stock_quantity = x.Quantity,
                        regular_price = x.RegularPrice,
                        meta_data = new List<ProductMeta>
                        {
                            new ProductMeta 
                            {
                                key = _availabilityMetaKey,
                                value = x.Availability,
                            }
                        },
                    })
                    .ToList(),
            };

            await _wcClient.Product.UpdateRange(batch);
        }

        public async Task<List<WcProduct>> GetProductsAsync(CancellationToken cancellationToken)
        {
            var products = new List<Product>();
            var page = 1;
            _totalPages = 1; // Until we figure out by parsing response headers

            // Keep requests single threaded to prevent WooCommerce server overload
            while (page <= _totalPages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.Information("Retrieveing products, page {Page}/{Total}",
                    page, _totalPages == 1 ? "?" : _totalPages.ToString());

                try
                {
                    products.AddRange(await _wcClient.Product.GetAll(new Dictionary<string, string> 
                    { 
                        ["page"] = page.ToString(),
                        ["per_page"] = Consts.BatchSize.ToString(),
                    }));
                    page += 1;
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Failed to retrieve products page");
                    
                    await Task.Delay(FailedRequestDelay(), cancellationToken);
                }

                await Task.Delay(RequestDelay(), cancellationToken);
            }

            return products
                .Where(product => product.id != null)
                .Where(Product => int.TryParse(Product.sku, out _))
                .Select(product => new WcProduct
                {
                    Id = product.id!.Value,
                    Sku = product.sku,
                    Name = product.name,
                    Availability = (string?) product.meta_data.FirstOrDefault(meta => meta.key == _availabilityMetaKey)?.value,
                    RegularPrice = product.regular_price,
                    SalePrice = product.sale_price,
                    StockStatus = product.stock_status,
                    Quantity = product.stock_quantity,
                    FixedPrice = GetFixedPriceProperty(product), 
                })
                .ToList();
        }

        private bool GetFixedPriceProperty(Product product)
        {
            var value = (string?) product.meta_data.FirstOrDefault(meta => meta.key == _fixedPriceMetaKey)?.value;
            
            return bool.TryParse(value, out var result) ? result : false;
        }

        private void ResponseFilter(HttpWebResponse response)
        {
            if (_configuration.TotalPages is null)
            {
                int.TryParse(response.Headers["X-WP-TotalPages"], out _totalPages);
            }
            else 
            {
                _totalPages = _configuration.TotalPages.Value;
            }
        }

        private WCObject Connect() 
        {
            RestAPI rest = new RestAPI(
                $"{_configuration.Host}/wp-json/wc/v3/", 
                _configuration.Client, 
                _configuration.Secret,
                authorizedHeader: false,
                responseFilter: ResponseFilter);

            return new WCObject(rest);
        }

        private int RequestDelay() =>
            _configuration.RequestDelay ?? Consts.RequestDelay;

        private int FailedRequestDelay() =>
            _configuration.FailedRequestDelay ?? Consts.FailedRequestDelay;
    }
}
