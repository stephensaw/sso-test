using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SSO.Interfaces;
using SSO.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SSO.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private string _connectionString;
        private IMemoryCache _cache;
        private IUserRepository _userRepository;

        public ApplicationRepository(IConfiguration configuration, IUserRepository userRepository, IMemoryCache cache)
        {
            this._connectionString = configuration["ConnectionString:Default"];
            this._cache = cache;
            this._userRepository = userRepository;
        }

        public ApplicationModel GetApplication(ApplicationAuthModel auth)
        {
            ApplicationModel app = null;

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (var sqlCommand = new SqlCommand("dbo.GetApplication", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@clientId", auth.ClientId);
                sqlCommand.Parameters.AddWithValue("@clientSecret", auth.ClientSecret);
                sqlCommand.Connection.Open();

                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            app = new ApplicationModel
                            {
                                Id = reader.GetInt64("id"),
                                RedirectUrl = reader.GetString("redirecturl")
                            };
                        }
                    }
                }
            }

            return app;
        }

        public bool IsRedirectUrlRegistered(string redirectUrl)
        {
            var registeredUrl = string.Empty;
            
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (var sqlCommand = new SqlCommand("dbo.GetRegisteredUrl", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@redirecturl", redirectUrl);
                sqlCommand.Connection.Open();

                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            registeredUrl = reader.GetString("redirecturl");
                        }
                    }
                }
            }

            return registeredUrl.Length > 0;
        }

        public UserModel VerifyToken(string token)
        {
            var userId = this._cache.Get(token);
            
            if (userId == null)
            {
                return null;
            }

            this._cache.Remove(token);

            return this._userRepository.GetUserById(Convert.ToInt64(userId));
        }
    }
}
