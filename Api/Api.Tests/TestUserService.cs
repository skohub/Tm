using Api.Service.Models;
using Api.Service.Services;
using FluentAssertions;

namespace Api.Tests
{
    [TestFixture]
    public class TestUserService
    {
        [Test]
        public void TestSucessfulValidation()
        {
            // arrange
            var users = new[]
            {
                new User { Name = "User", Token = "Token" },
            };

            // act
            Action act = () => new UserService(users);

            // assert
            act.Should().NotThrow();
        }

        [Test]
        public void TestDuplicateToken()
        {
            // arrange
            var users = new[]
            {
                new User { Name = "User", Token = "Token" },
                new User { Name = "User", Token = "Token" },
            };

            // act
            Action act = () => new UserService(users);

            // assert
            act.Should().Throw<ArgumentException>();
        }
    }
}