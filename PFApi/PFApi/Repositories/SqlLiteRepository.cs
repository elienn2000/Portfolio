
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;

namespace PortfolioApi.Repositories
{
    public class SQLiteRepository
    {
        private readonly IConfiguration _configuration;

        public SQLiteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            return connection;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(int? commandTimeout = null) where T : class
        {
            using var connection = OpenConnection();
            return await connection.GetAllAsync<T>(transaction: null, commandTimeout);
        }

        public async Task<T?> GetAsync<T>(object id, int? commandTimeout = null) where T : class
        {
            using var connection = OpenConnection();
            return await connection.GetAsync<T>(id, transaction: null, commandTimeout);
        }

        public async Task<IEnumerable<T>> GetFromQueryAsync<T>(
            string query,
            object? parms = null,
            int? commandTimeout = null)
        {
            using var connection = OpenConnection();
            var results = await Dapper.SqlMapper.QueryAsync<T>(
                connection,
                query,
                parms,
                transaction: null,
                commandTimeout: commandTimeout,
                commandType: System.Data.CommandType.Text
            );
            return results;
        }

        public async Task<long> InsertAsync<T>(T entity, int? commandTimeout = null) where T : class
        {
            using var connection = OpenConnection();
            return await connection.InsertAsync(entity, transaction: null, commandTimeout);
        }

        public async Task<bool> UpdateAsync<T>(T entity, int? commandTimeout = null) where T : class
        {
            using var connection = OpenConnection();
            return await connection.UpdateAsync(entity, transaction: null, commandTimeout);
        }

        public async Task<bool> DeleteAsync<T>(T id, int? commandTimeout = null) where T : class
        {
            using var connection = OpenConnection();
            return await connection.DeleteAsync<T>(id, transaction: null, commandTimeout);
        }
    }
}
