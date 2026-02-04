using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Almacen.Data.Repositories
{
    public abstract class SqlRepository : IDisposable 
    {
        private readonly string _connectionString;
        private IDbConnection? _connection;

        protected SqlRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string no puede estar vacía", nameof(connectionString));

            _connectionString = connectionString;
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}