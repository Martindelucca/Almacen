// Almacen.Data/Repositories/SqlRepository.cs
using Microsoft.Data.SqlClient;
using System.Data;

namespace Almacen.Data.Repositories
{
    public abstract class SqlRepository
    {
        private readonly string _connectionString;

        public SqlRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}