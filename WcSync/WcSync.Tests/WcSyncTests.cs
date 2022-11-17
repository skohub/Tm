﻿using Moq;
using NUnit.Framework;
using Tm.WcSync.Wc;
using Tm.WcSync.Db;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tm.WcSync.Model.Entities;
using Tm.WcSync.Sync;
using System.Linq;
using System.Threading;
using Serilog;

namespace Tm.WcSync.Tests
{
    [TestFixture]
    public class WcSyncTests
    {
        private ProductService _productService;
        private Mock<IWcProductService> _wcProductServiceMock;
        private Mock<IDbProductRepository> _dbProductRepositoryMock;
        private WcProduct DefaultWcProduct;
        private DbProduct DefaultDbProduct;

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

            DefaultDbProduct = new DbProduct
            {
                Id = 0,
                Availability = new List<Store>
                {
                    new Store
                    {
                        Name = "test",
                        Quantity = 1,
                        Price = 0,
                        Type = StoreType.Shop,
                    }
                }
            };

            var cancellationToken = new CancellationToken();

            _wcProductServiceMock = new Mock<IWcProductService>();
            _wcProductServiceMock
                .Setup(m => m.GetProductsAsync(cancellationToken))
                .Returns(Task.FromResult(new List<WcProduct>{ DefaultWcProduct }));

            _dbProductRepositoryMock = new Mock<IDbProductRepository>();
            _dbProductRepositoryMock
                .Setup(r => r.GetProductsAsync())
                .ReturnsAsync(new List<DbProduct>{ DefaultDbProduct });

            var loggerMock = new Mock<ILogger>();

            _productService = new ProductService(
                _wcProductServiceMock.Object,
                _dbProductRepositoryMock.Object,
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
            DefaultDbProduct.Availability.First().Price = 10000;
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
            DefaultDbProduct.Availability.First().Price = 10000;
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
