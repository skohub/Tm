using Moq;
using NUnit.Framework;
using WcSync.Model.Entities;
using WcSync.Sync;
using Serilog;
using System.Linq;

namespace WcSync.Tests
{
    [TestFixture]
    public class PriceCalculatorTests
    {
        [Test]
        public void EmptyAvailabilityShouldReturnNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var product = BuildDbProduct();

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(product);

            // Assert
            Assert.IsNull(price);
            Assert.IsNull(salePrice);
        }

        [Test]
        public void InconsistentAvailabilityShouldReturnAny()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var dbProduct = BuildDbProduct(
                (StoreType.Shop, 1000, 1),
                (StoreType.Shop, 2000, 1)
            );

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.IsTrue(
                (price == 1000 && salePrice == 1000) ||
                (price == 2000 && salePrice == 2000));
        }

        [Test]
        public void WrongTypeOfAvailabilityShouldReturnNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var dbProduct = BuildDbProduct(
                (StoreType.ClosedShop, 10000, 1),
                (StoreType.Inactive, 10000, 1),
                (StoreType.RepairShop, 10000, 1)
            );

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.IsNull(price);
            Assert.IsNull(salePrice);
        }

        [Test]
        public void ZeroQuantityShouldReturnNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var dbProduct = BuildDbProduct((StoreType.Shop, 1000, 0));

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.IsNull(price);
            Assert.IsNull(salePrice);
        }

        [Test]
        public void AbsentAvailabilityShouldReturnNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var dbProduct = BuildDbProduct();

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.IsNull(price);
            Assert.IsNull(salePrice);
        }

        [Test]
        public void TestCalculationBelowLowerBoundary()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var dbProduct = BuildDbProduct((StoreType.Shop, 1000, 1));

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.AreEqual(1000, price);
            Assert.AreEqual(1000, salePrice);
        }

        [Test]
        public void TestCalculationAboveLowerBoundary()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);
            var dbProduct = BuildDbProduct((StoreType.Shop, 10000, 1));

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.AreEqual(10000, price);
            Assert.AreEqual(10000, salePrice); // Discounts are disabled
        }

        [Test]
        public void WrongAvailabilityShouldNotAffectCalculation()
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var priceCalculator = new PriceCalculator(loggerMock.Object);

            var dbProduct = BuildDbProduct(
                (StoreType.Shop, 10000, 1),
                (StoreType.Shop, 10000, 0),
                (StoreType.ClosedShop, 10000, 1),
                (StoreType.ClosedShop, 0, 1)
            );

            // Act
            (var price, var salePrice) = priceCalculator.GetPrice(dbProduct);

            // Assert
            Assert.AreEqual(10000, price);
            Assert.AreEqual(10000, salePrice);
        }

        private DbProduct BuildDbProduct(params 
            (StoreType type, decimal price, int quantity)[] availability) =>
            new DbProduct
            {
                Id = 0,
                Name = "test",
                Availability = availability
                    .Select(x => new Store
                    {
                        Name = "test",
                        Type = x.Item1,
                        Price = x.Item2,
                        Quantity = x.Item3
                    })
                    .ToList()
            };
    }
}