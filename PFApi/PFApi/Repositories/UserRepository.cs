using PortfolioApi.Models;
using Dapper;
using Dapper.Contrib.Extensions;

namespace PortfolioApi.Repositories
{
    public class UserRepository : SQLiteRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        // Metodo specifico per gli utenti
        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = this.OpenConnection();
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User?> GetByLoginRequestAsync(AuthRequest request)
        {
            using var connection = OpenConnection();

            string sql;
            object parameters;

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                sql = "SELECT * FROM Users WHERE Email = @Email";
                parameters = new { Email = request.Email };
            }
            else if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                sql = "SELECT * FROM Users WHERE Username = @UserName";
                parameters = new { UserName = request.UserName };
            }
            else
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);
        }


        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var connection = OpenConnection();
            var sql = "SELECT * FROM Users WHERE Username = @Username";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task SaveTokensAsync(User user, string accessToken, DateTime accessExpires, string refreshToken, DateTime refreshExpires)
        {
            using var connection = OpenConnection();

            user.AccessToken = accessToken;
            user.AccessTokenExpiryTime = accessExpires; 
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshExpires;

            await connection.UpdateAsync<User>(user);
        }


        public async Task<long?> InsertUserAsync(AuthRequest request)
        {
            try
            {
                using var connection = OpenConnection();

                // Check if user already exists by the email
                var existingUser = await GetByEmailAsync(request.Email);

                if (existingUser != null)
                    throw new InvalidOperationException("ERR_USER_EXISTS");


                // Crate new user
                var user = new User
                {
                    Email = request.Email,
                    Username = request.UserName,
                    PasswordHash = PasswordHelper.HashPassword(request.Password),
                };

                var userId = await connection.InsertAsync(user);
                return userId;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore durante l'inserimento dell'utente", ex);
            }
        }



        public async Task<User?> CheckEmailExistAsync(AuthRequest request)
        {
            using var connection = OpenConnection();

            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = request.Email });
        }

        public async Task<User?> CheckUsernameExistAsync(AuthRequest request)
        {
            using var connection = OpenConnection();

            var sql = "SELECT * FROM Users WHERE Username = @UserName";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserName = request.UserName });
        }

        public async Task<VerificationStatus> CheckUserActiviationCodeAsync(EmailVerificationRequest request)
        {
            var user = await GetByEmailAsync(request.Email);

            if(user == null)
                return VerificationStatus.UserNotFound;

            if (user.VerificationCode != request.VerificationCode)
                return VerificationStatus.CodeInvalid;

            if (user.VerificationCodeExpiryTime < request.ExpiryTime)
                return VerificationStatus.CodeExpired;

            // Update on schema
            user.VerificationCode = null;
            user.VerificationCodeExpiryTime = null;
            user.Active = 1;
            await UpdateAsync(user);

            return VerificationStatus.Success;
        }

    }
}
