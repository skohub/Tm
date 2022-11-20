using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        private WCObject _wcObject;
        private int _totalPages;

        private WCObject WcClient => _wcObject ?? (_wcObject = Connect());

        public WcProductService(WcConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task UpdateProductAsync(int productId, string stockStatus, string availability, decimal? regularPrice, decimal? salePrice)
        {
            await WcClient.Product.Update(productId, new Product
            {
                stock_status = stockStatus,
                regular_price = regularPrice,
                sale_price = salePrice,
                meta_data = new List<ProductMeta>
                {
                    new ProductMeta 
                    {
                        key = _availabilityMetaKey,
                        value = availability,
                    }
                }
            });
        }

        public async Task UpdateProductsAsync(List<WcProduct> products)
        {
            var batch = new ProductBatch()
            {
                update = products
                    .Select(x => new Product 
                    {
                        id = x.Id,
                        stock_status = x.StockStatus,
                        regular_price = x.RegularPrice,
                        sale_price = x.SalePrice,
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

            await WcClient.Product.UpdateRange(batch);
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
                    products.AddRange(await WcClient.Product.GetAll(new Dictionary<string, string> 
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
                .Where(p => p.id != null)
                .Where(p => int.TryParse(p.sku, out _))
                .Select(product => new WcProduct
                {
                    Id = product.id.Value,
                    Sku = product.sku,
                    Name = product.name,
                    Availability = (string) product.meta_data.FirstOrDefault(meta => meta.key == _availabilityMetaKey)?.value,
                    RegularPrice = product.regular_price,
                    SalePrice = product.sale_price,
                    StockStatus = product.stock_status,
                    FixedPrice = GetFixedPriceProperty(product), 
                })
                .ToList();
        }

        private bool GetFixedPriceProperty(Product product)
        {
            var value = (string) product.meta_data.FirstOrDefault(meta => meta.key == _fixedPriceMetaKey)?.value;
            
            return bool.TryParse(value, out var result) ? result : false;
        }

        private async Task<Product> GetProductBySku(string sku)
        {
            var products = await WcClient.Product.GetAll(new Dictionary<string, string>
            { 
                { "sku", sku },
            });

            return products.FirstOrDefault() ?? throw new Exception($"Product with sku {sku} was not found");
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
