using System.Security.Claims;
using Api.Service.Auth;
using Api.Service.Controllers;
using Data;
using Data.Models.ArbitrarySql;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Api.Tests.Integration
{
    [TestFixture]
    [Ignore("Integration")]
    public class TestArbitrarySqlController
    {
        private ArbitrarySqlController BuildController(string? connectionStringName)
        {
            var connectionStrings = GetConnectionStrings();
            var connectionFactory = new MySqlConnectionFactory(connectionStrings);
            var service = new ArbitrarySqlService(connectionFactory);
            var controller = new ArbitrarySqlController(service);
            var user = BuildUser(connectionStringName);
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(user);
            controller.ControllerContext = new ControllerContext { HttpContext = httpContextMock.Object };

            return controller;
        }

        private ClaimsPrincipal BuildUser(string? connectionStringName)
        {
            var claims = new List<Claim>();
            if (connectionStringName != null)
            {
                claims.Add(new Claim(TokenClaimTypes.ConnectionStringName, connectionStringName));
            }
            var identity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(identity);
        }

        private Dictionary<string, string> GetConnectionStrings() => Configuration
            .GetConfiguration()
            .GetSection("ConnectionStrings")
            .Get<Dictionary<string, string>>();

        [Test]
        public void TestHappyPath()
        {
            // arrange
            var controller = BuildController(GetConnectionStrings().First().Key);

            // act
            var result = controller.Select("select * from places", new List<SqlParameter>());

            // assert
            result.Should().BeOfType<JsonResult>();
            var sqlResult = (result as JsonResult)!.Value as SqlResult;
            sqlResult.Should().NotBeNull();
            sqlResult!.Rows.Should().NotBeEmpty();
        }

        [Test]
        public void TestUserWithoutConnectionStringName()
        {
            // arrange
            var controller = BuildController(null);

            // act
            var result = controller.Select("select * from places", new List<SqlParameter>());

            // assert
            result.Should().BeOfType<StatusCodeResult>();
            (result as StatusCodeResult)!.StatusCode.Should().Be(403);
        }
    }
}