using Moq;
using NUnit.Framework;
using WcSync.Wc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WcSync.Model.Entities;
using WcSync.Sync;
using System.Threading;
using Serilog;
using Data.Interfaces;
using Data.Models.Products;

namespace WcSync.Tests
{
    [TestFixture]
    public class WcSyncTests
    {
        [Test]
        public async Task HappyFlow()
        {
            // Arrange
            var services = BuildServices(
                dbProductPrice: 10000,
                wcProductRegularPrice: 10000,
                wcProductSalePrice: 10000
            );

            // Act
            await services.ProductService.UpdateAllProductsAsync(new CancellationToken());

            // Assert
        }

        [Test]
        public async Task TestProductUpToDate()
        {
            // Arrange
            var services = BuildServices(
                dbProductPrice: 10000,
                wcProductRegularPrice: 10000,
                wcProductSalePrice: 10000
            );

            // Act
            await services.ProductService.UpdateAllProductsAsync(new CancellationToken());

            // Assert
            services.WcProductServiceMock.Verify(
                s => s.UpdateProductsAsync(It.IsAny<List<WcProduct>>()),
                Times.Never);
        }

        [Test]
        [TestCase(0, 9700)]
        [TestCase(10000, 0)]
        public async Task TestUpdateProductCalledWhenPriceDiffers(decimal regularPrice, decimal salePrice)
        {
            // Arrange
            var services = BuildServices(
                dbProductPrice: 10000,
                wcProductRegularPrice: regularPrice,
                wcProductSalePrice: salePrice
            );

            // Act
            await services.ProductService.UpdateAllProductsAsync(new CancellationToken());

            // Assert
            services.WcProductServiceMock.Verify(
                s => s.UpdateProductsAsync(It.IsAny<List<WcProduct>>()),
                Times.Once);
        }

        internal class Services
        {
            required public ProductService ProductService;
            required public Mock<IWcProductService> WcProductServiceMock;
            required public Mock<IProductsRepository> ProductsRepositoryMock;
        }

        private Services BuildServices(
            decimal dbProductPrice, decimal wcProductRegularPrice, decimal wcProductSalePrice)
        {
            var wcProduct = new WcProduct
            {
                Id = default,
                Availability = "test",
                Sku = "0",
                RegularPrice = wcProductRegularPrice,
                SalePrice = wcProductSalePrice,
                StockStatus = "instock",
            };

            var dbProduct = new Product
            {
                ProductId = 0,
                ProductName = "test",
                StoreName = "test",
                StoreType = Data.Models.Products.StoreType.Shop,
                Quantity = 1,
                Price = dbProductPrice,
            };

            var cancellationToken = new CancellationToken();

            var wcProductServiceMock = new Mock<IWcProductService>();
            wcProductServiceMock
                .Setup(m => m.GetProductsAsync(cancellationToken))
                .Returns(Task.FromResult(new List<WcProduct>{ wcProduct }));

            var productsRepositoryMock = new Mock<IProductsRepository>();
            productsRepositoryMock
                .Setup(r => r.GetProductsAsync())
                .ReturnsAsync(new List<Product>{ dbProduct });

            var loggerMock = new Mock<ILogger>();

            var productService = new ProductService(
                wcProductServiceMock.Object,
                productsRepositoryMock.Object,
                new PriceCalculator(loggerMock.Object),
                loggerMock.Object);

            return new Services
            {
                WcProductServiceMock = wcProductServiceMock,
                ProductsRepositoryMock = productsRepositoryMock,
                ProductService = productService
            };
        }
    }
}
