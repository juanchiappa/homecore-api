using System;
using Dapper;
using HomeCore.Entities;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory ConnectionFactory)
        {
            _connectionFactory = ConnectionFactory;
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserName = @UserName",
                new { UserName = userName });
        }
}
}
