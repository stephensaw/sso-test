using Microsoft.AspNetCore.Mvc;
using SSO.Models;
using SSOTests.Repositories;
using Xunit;
using ApiController = SSO.Controllers.ApiController;

namespace SSOTests
{
    public class ApiControllerTests
    {
        [Fact(DisplayName = "Should fail when invalid client ID")]
        public void GetAuthUrl_Should404WithWrongID()
        {
            var repo = new MockApplicationRepository();
            var api = new ApiController(repo);

            var authModel = new ApplicationAuthModel
            {
                ClientId = "fakeId",
                ClientSecret = "clientsecret1"
            };

            var response = api.GetAuthUrl(authModel);
            var result = response as NotFoundObjectResult;

            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "Should fail when invalid client secret")]
        public void GetAuthUrl_ShouldFailWithWrongSecret()
        {
            var repo = new MockApplicationRepository();
            var api = new ApiController(repo);

            var authModel = new ApplicationAuthModel
            {
                ClientId = "clientid1",
                ClientSecret = "fakesecret"
            };

            var response = api.GetAuthUrl(authModel);
            var result = response as NotFoundObjectResult;

            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "Should fail when no redirect url")]
        public void GetAuthUrl_ShouldFailWhenNoRedirectUrl()
        {
            var repo = new MockApplicationRepository();
            var api = new ApiController(repo);

            var authModel = new ApplicationAuthModel
            {
                ClientId = "clientid2",
                ClientSecret = "clientsecret2"
            };

            var response = api.GetAuthUrl(authModel);
            var result = response as NotFoundObjectResult;

            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "Should fail when token invalid")]
        public void VerifyToken_ShouldFailWhenTokenInvalid()
        {
            var repo = new MockApplicationRepository();
            var api = new ApiController(repo);
            var token = "1111";

            var response = api.VerifyToken(token);
            var result = response as NotFoundObjectResult;

            Assert.Equal(404, result.StatusCode);
        }

        [Fact(DisplayName = "Should return user when token invalid")]
        public void VerifyToken_ShouldReturnUserWhenTokenValid()
        {
            var repo = new MockApplicationRepository();
            var api = new ApiController(repo);
            var token = "12345";

            var response = api.VerifyToken(token);
            var result = response as OkObjectResult;

            Assert.Equal(200, result.StatusCode);
            Assert.Equal("test@email.com", (result.Value as UserModel).Email);
        }
    }
}
