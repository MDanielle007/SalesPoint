using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesPoint.DTO;
using SalesPoint.Exceptions;
using SalesPoint.Interfaces;
using SalesPoint.Models;
using SalesPoint.ViewModels;

namespace SalesPoint.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login() => View();

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dto = new LoginDTO
                {
                    Username = model.Username,
                    Password = model.Password,
                };

                var success = await _userService.LoginUserAsync(dto);
                return Ok(success);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex, "Login failed for user: {Username}", model.Username);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", model.Username);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }
}
