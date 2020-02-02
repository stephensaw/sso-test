using Microsoft.Extensions.Configuration;
using SSO.Interfaces;
using SSO.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace SSO.Repositories
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            this._connectionString = configuration["ConnectionString:Default"];
        }

        public bool ValidateLastChanged(long userId, DateTime lastChanged)
        {
            var user = this.GetUserById(userId);

            if (user == null)
            {
                return false;
            }

            return Math.Abs((user.LastChanged - lastChanged).TotalSeconds) < 1;
        }

        public bool RegisterUser(RegisterUserModel user)
        {

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                using (var sqlCommand = new SqlCommand("dbo.RegisterUser", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("fullname", user.FullName);
                    sqlCommand.Parameters.AddWithValue("email", user.Email);
                    sqlCommand.Parameters.AddWithValue("password", this.HashPassword(user.Password));
                    sqlCommand.Connection.Open();

                    try
                    {   
                        var result = sqlCommand.ExecuteScalar();

                        return result != null;
                    }
                    catch (SqlException sqlException)
                    {
                        // log error and return error to user
                        throw sqlException;
                    }
                    catch (Exception exception)
                    {
                        // log error
                        throw exception;
                    }
                }
            }
        }

        public UserModel GetUserById(long userId)
        {
            UserModel user = null;

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (var sqlCommand = new SqlCommand("dbo.GetUserById", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@userId", userId);
                sqlCommand.Connection.Open();

                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            user = new UserModel
                            {
                                Id = reader.GetInt64("id"),
                                FullName = reader.GetString("fullname"),
                                Email = reader.GetString("email"),
                                LastChanged = reader.GetDateTime("modified")
                            };
                        }
                    }
                }
            }

            return user;
        }

        public UserModel GetUserByLogin(LoginModel login)
        {
            UserModel user = null;
            
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (var sqlCommand = new SqlCommand("dbo.GetUserByLogin", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("email", login.Email);
                sqlCommand.Parameters.AddWithValue("password", this.HashPassword(login.Password));
                sqlCommand.Connection.Open();

                using (var reader = sqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            user = new UserModel
                            {
                                Id = reader.GetInt64("id"),
                                FullName = reader.GetString("fullname"),
                                Email = reader.GetString("email"),
                                LastChanged = reader.GetDateTime("modified")
                            };
                        }
                    }
                }
            }

            return user;
        }

        private string HashPassword(string password)
        {
            byte[] bytes = new byte[password.Length];
            SHA256 hash = new SHA256CryptoServiceProvider();
            bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            return Convert.ToBase64String(bytes);
        }
    }
}
