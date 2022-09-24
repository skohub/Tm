using Api.Service.Controllers;
using Data;
using Tm.Data.Models;

namespace Api.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestArbitrarySqlController()
        {
            // arrange
            var connectionFactory = new ConnectionFactory();
            var service = new ArbitrarySqlService(connectionFactory);
            var controller = new ArbitrarySqlController(service);

            // act
            var result = controller.Select("select * from places", new List<SqlParameter>());

            // assert
            Assert.IsNotNull(result);
        }
    }
}