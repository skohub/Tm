using Api.Service.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Api.Tests
{
    public class TestConfiguration
    {
        [Test]
        public void TestUserWithoutConnectionStringName()
        {
            // arrange
            var userName = "UserWithoutConnectionString";
            var configuration = Configuration.GetConfiguration();

            // act
            var user = configuration
                .GetSection("Users")
                .Get<User[]>()
                .First(x => x.Name == userName);

            // assert
            user.ConnectionStringName.Should().BeNull();
        }

        [Test]
        public void Test()
        {
            var configuration = Configuration.GetConfiguration();
            var connectionStrings = configuration
                .GetSection("ConnectionStrings")
                .Get<Dictionary<string, string>>();
        }
    }
}