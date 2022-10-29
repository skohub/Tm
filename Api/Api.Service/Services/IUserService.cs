using Api.Service.Models;

namespace Api.Service.Services
{
    public interface IUserService
    {
        User? GetUserByToken(string token);
    }
}
