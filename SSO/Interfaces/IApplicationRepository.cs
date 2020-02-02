using SSO.Models;

namespace SSO.Interfaces
{
    public interface IApplicationRepository
    {
        public ApplicationModel GetApplication(ApplicationAuthModel auth);

        public UserModel VerifyToken(string token);

        public bool IsRedirectUrlRegistered(string redirectUrl);
    }
}
