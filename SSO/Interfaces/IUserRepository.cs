using SSO.Models;
using System;

namespace SSO.Interfaces
{
    public interface IUserRepository
    {
        public bool ValidateLastChanged(long userId, DateTime lastChanged);

        public bool RegisterUser(RegisterUserModel user);

        public UserModel GetUserByLogin(LoginModel login);

        public UserModel GetUserById(long userId);
    }
}
