using SSO.Interfaces;
using SSO.Models;

namespace SSOTests.Repositories
{
    class MockApplicationRepository : IApplicationRepository
    {
        public ApplicationModel GetApplication(ApplicationAuthModel auth)
        {
            if (
                (auth.ClientId != "clientid1" && auth.ClientSecret != "clientsecret1") &&
                (auth.ClientId != "clientid2" && auth.ClientSecret != "clientsecret2"))
            {
                return null;
            }

            var url = "https://localhost:12345/callback";

            if (auth.ClientId == "clientid2")
            {
                url = string.Empty;
            }

            return new ApplicationModel
            {
                Id = 1,
                RedirectUrl = url
            };
        }

        public bool IsRedirectUrlRegistered(string redirectUrl)
        {
            return "https://localhost:12345/callback".Equals(redirectUrl);
        }

        public UserModel VerifyToken(string token)
        {
            if (token != "12345")
            {
                return null;
            }

            return new UserModel
            {
                Id = 1,
                Email = "test@email.com",
                FullName = "Test User",
                LastChanged = new System.DateTime()
            };
        }
    }
}
