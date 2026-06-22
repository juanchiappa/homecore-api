using System;
using Dapper;
using HomeCore.Entities;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.DAL
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DbInitializer(IDbConnectionFactory ConnectionFactory)
        {
            _connectionFactory = ConnectionFactory;
        }
        public void Initialize(string adminUserName, string adminPasswordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            connection.Execute("""
            CREATE TABLE IF NOT EXISTS Users (
                Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                UserName     TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                Role         TEXT NOT NULL DEFAULT 'admin',
                CreatedAt    TEXT NOT NULL
            )
        """);

            var existingAdmin = connection.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE UserName = @UserName",
                new { UserName = adminUserName });

            if (existingAdmin is null)
            {
                connection.Execute("""
                INSERT INTO Users (UserName, PasswordHash, Role, CreatedAt)
                VALUES (@UserName, @PasswordHash, @Role, @CreatedAt)
            """, new
                {
                    UserName = adminUserName,
                    PasswordHash = adminPasswordHash,
                    Role = "admin",
                    CreatedAt = DateTime.UtcNow.ToString("o")
                });
            }
        }
    }
}
