using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SalesPoint.DTO;
using SalesPoint.Exceptions;
using SalesPoint.Interfaces;
using SalesPoint.Models;
using SalesPoint.ViewModels;
using System.Security.Claims;

namespace SalesPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { errors = ModelState });
                }

                var response = await _userService.LoginUserAsync(loginDto);

                // Set JWT token in HTTP-only cookie for better security
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Only send over HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = response.Expiration
                };

                Response.Cookies.Append("authToken", response.Token, cookieOptions);

                return Ok(new
                {
                    token = response.Token,
                    expiration = response.Expiration,
                    user = response.User,
                    success = true,
                    message = "Login successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for username: {Username}", loginDto.Username);

                if (ex.Message.Contains("Invalid username or password"))
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                // Clear the authentication cookie
                Response.Cookies.Delete("authToken");

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not found in token" });
                }

                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current user");
                return StatusCode(500, new { message = "Failed to retrieve user information" });
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { errors = ModelState });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User not found in token" });
                }

                await _userService.ChangePassword(userId, dto);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to change password");

                if (ex.Message.Contains("Failed to change password"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, new { message = "An error occurred while changing password" });
            }
        }

        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                return Ok(new
                {
                    isValid = true,
                    userId = userId,
                    userName = userName,
                    role = userRole
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return Unauthorized(new { message = "Invalid token" });
            }
        }
    }
}
