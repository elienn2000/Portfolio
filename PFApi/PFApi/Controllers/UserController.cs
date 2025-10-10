using Dapper.SimpleRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PortfolioApi.Models;
using PortfolioApi.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserRepository _repository;
        private readonly ILogger<UserController> _logger;


        public UserController(IConfiguration config, UserRepository repository, ILogger<UserController> logger)
        {
            _config = config;
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] AuthRequest request)
        {
            try
            {
                var result = await _repository.InsertUserAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message == "ERR_USER_EXISTS")
            {
                return Conflict(new { errorCode = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione");
                return StatusCode(500, new { errorCode = "ERR_INTERNAL" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var result = await _repository.GetAllAsync<User>();
                return Ok(result); // Restituisce l'Id o il risultato di InsertAsync
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'inserimento dell'utente");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
