using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HomeCore.DAL
{
    public class SqliteConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        public SqliteConnectionFactory(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }
        public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);
    }
}
