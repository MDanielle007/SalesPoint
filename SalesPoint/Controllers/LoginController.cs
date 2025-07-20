using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPoint.DTO;
using SalesPoint.Interfaces;
using SalesPoint.ViewModels;
using System.Security.Claims;

namespace SalesPoint.Controllers
{
    [Route("")]
    public class LoginController : Controller
    {
        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            // Redirect to dashboard if already authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View(new LoginViewModel());
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromServices] IUserService userService, [FromBody] LoginDTO loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { errors = ModelState });
                }

                var response = await userService.LoginUserAsync(loginDto);

                // Set JWT token in HTTP-only cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
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
                    redirectUrl = response.User.Role == Enums.UserRole.Cashier? Url.Action("PointOfSales","Transaction", new { area = "Sales"}) : Url.Action("Index", "Dashboard", new { area = "Management"})
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Invalid username or password"))
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

    }
}
