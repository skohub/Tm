using Api.Service.Models;

namespace Api.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IList<User> _users;

        public UserService(IList<User> users)
        {
            _users = users;
            Validate();
        }

        public User? GetUserByToken(string token) => _users.FirstOrDefault(x => x.Token == token);

        public void Validate()
        {
            if (_users != null && _users.GroupBy(x => x.Token).Any(x => x.Count() > 1))
            {
                throw new ArgumentException("Token duplicates are not allowed");
            }
        }
    }
}
