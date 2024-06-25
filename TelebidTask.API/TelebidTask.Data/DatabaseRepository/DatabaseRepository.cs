using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using TelebidTask.Data.Contracts;
using TelebidTask.Data.Models;

namespace TelebidTask.Data.DatabaseRepository
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private string connectionString;

        public DatabaseRepository()
        {
            this.connectionString = "Server=.;Database=UserDB;Integrated Security=true;";
        }

        public async Task<User> CreateUser(User user)
        {
            var rowsAffected = 0;
            var userId = Guid.NewGuid();

            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("createUser", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var id = new SqlParameter()
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = userId,
                    Direction = ParameterDirection.Input
                };

                var name = new SqlParameter()
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Name,
                    Direction = ParameterDirection.Input
                };

                var email = new SqlParameter()
                {
                    ParameterName = "@Email",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Email,
                    Direction = ParameterDirection.Input
                };

                var password = new SqlParameter()
                {
                    ParameterName = "@Password",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Password,
                    Direction = ParameterDirection.Input
                };

                var salt = new SqlParameter()
                {
                    ParameterName = "@Salt",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Salt,
                    Direction = ParameterDirection.Input
                };

                cmd.Parameters.Clear();

                cmd.Parameters.Add(id);
                cmd.Parameters.Add(name);
                cmd.Parameters.Add(email);
                cmd.Parameters.Add(password);
                cmd.Parameters.Add(salt);

                await connection.OpenAsync();

                rowsAffected = await cmd.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }

            return rowsAffected > 0 ? await GetUserById(userId) : null;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            User user = null;

            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("getUsersWithEmail", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var parameter = new SqlParameter()
                {
                    ParameterName = "@Email",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = email,
                    Direction = ParameterDirection.Input
                };

                cmd.Parameters.Clear();

                cmd.Parameters.Add(parameter);

                await connection.OpenAsync();

                var dataReader = await cmd.ExecuteReaderAsync();

                while (dataReader.Read())
                {
                    user = new User();
                    user.Id = (Guid)dataReader["Id"];
                    user.Name = $"{dataReader["Name"]}";
                    user.Email = $"{dataReader["Email"]}";
                    user.Password = $"{dataReader["Password"]}";
                    user.Salt = $"{dataReader["Salt"]}";
                }

                await connection.CloseAsync();
            }

            return user;
        }

        public async Task<User> GetUserById(Guid id)
        {
            User user = null;

            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("getUserById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var parameter = new SqlParameter()
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = id,
                    Direction = ParameterDirection.Input
                };

                cmd.Parameters.Clear();

                cmd.Parameters.Add(parameter);

                await connection.OpenAsync();
                
                var dataReader = await cmd.ExecuteReaderAsync();

                while (dataReader.Read())
                {
                    user = new User();
                    user.Id = (Guid)dataReader["Id"];
                    user.Name = $"{dataReader["Name"]}";
                    user.Email = $"{dataReader["Email"]}";
                    user.Password = $"{dataReader["Password"]}";
                    user.Salt = $"{dataReader["Salt"]}";
                }

                await connection.CloseAsync();
            }

            return user;
        }

        public async Task UpdateUser(Guid id, User user)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand("updateUserInfo", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                var userId = new SqlParameter()
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Value = id,
                    Direction = ParameterDirection.Input
                };

                var name = new SqlParameter()
                {
                    ParameterName = "@Name",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Name,
                    Direction = ParameterDirection.Input
                };

                var email = new SqlParameter()
                {
                    ParameterName = "@Email",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Email,
                    Direction = ParameterDirection.Input
                };

                var password = new SqlParameter()
                {
                    ParameterName = "@Password",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = user.Password,
                    Direction = ParameterDirection.Input
                };

                cmd.Parameters.Clear();

                cmd.Parameters.Add(userId);
                cmd.Parameters.Add(name);
                cmd.Parameters.Add(email);
                cmd.Parameters.Add(password);

                await connection.OpenAsync();

                await cmd.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
        }
    }
}
