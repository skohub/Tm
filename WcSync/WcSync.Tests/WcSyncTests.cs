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
        private ProductService _productService;
        private Mock<IWcProductService> _wcProductServiceMock;
        private Mock<IProductsRepository> _productsRepositoryMock;
        private WcProduct DefaultWcProduct;
        private ItemRest DefaultDbProduct;

        [SetUp]
        public void SetUp()
        {
            DefaultWcProduct = new WcProduct
            {
                Availability = "test",
                Sku = "0",
                RegularPrice = 0,
                SalePrice = 0,
                StockStatus = "instock",
            };

            DefaultDbProduct = new ItemRest
            {
                ItemID = 0,
                name = "test",
                i_n = "test",
                summ = 1,
                price = 0,
                StoreType = Data.Models.Products.StoreType.Shop
            };

            var cancellationToken = new CancellationToken();

            _wcProductServiceMock = new Mock<IWcProductService>();
            _wcProductServiceMock
                .Setup(m => m.GetProductsAsync(cancellationToken))
                .Returns(Task.FromResult(new List<WcProduct>{ DefaultWcProduct }));

            _productsRepositoryMock = new Mock<IProductsRepository>();
            _productsRepositoryMock
                .Setup(r => r.GetProductsAsync())
                .ReturnsAsync(new List<ItemRest>{ DefaultDbProduct });

            var loggerMock = new Mock<ILogger>();

            _productService = new ProductService(
                _wcProductServiceMock.Object,
                _productsRepositoryMock.Object,
                new PriceCalculator(loggerMock.Object),
                loggerMock.Object);
        }

        [Test]
        public async Task HappyFlow()
        {
            // Arrange

            // Act
            await _productService.UpdateAllProductsAsync(new CancellationToken());

            // Assert
        }

        [Test]
        public async Task TestProductUpToDate()
        {
            // Arrange
            DefaultDbProduct.price = 10000;
            DefaultWcProduct.RegularPrice = 10000;
            DefaultWcProduct.SalePrice = 10000;

            // Act
            await _productService.UpdateAllProductsAsync(new CancellationToken());

            // Assert
            _wcProductServiceMock.Verify(
                s => s.UpdateProductsAsync(It.IsAny<List<WcProduct>>()),
                Times.Never);
        }

        [Test]
        [TestCase(0, 9700)]
        [TestCase(10000, 0)]
        public async Task TestUpdateProductCalledWhenPriceDiffers(decimal regularPrice, decimal salePrice)
        {
            // Arrange
            DefaultDbProduct.price = 10000;
            DefaultWcProduct.RegularPrice = regularPrice;
            DefaultWcProduct.SalePrice = salePrice;

            // Act
            await _productService.UpdateAllProductsAsync(new CancellationToken());

            // Assert
            _wcProductServiceMock.Verify(
                s => s.UpdateProductsAsync(It.IsAny<List<WcProduct>>()),
                Times.Once);
        }
    }
}
