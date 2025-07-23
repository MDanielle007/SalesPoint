// Middleware/JwtMiddleware.cs
using Microsoft.IdentityModel.Tokens;
using SalesPoint.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SalesPoint.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var token = context.Request.Cookies["authToken"]; // Focus on cookie first

            if (string.IsNullOrEmpty(token))
            {
                // Fallback to Authorization header if needed
                token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            }

            if (!string.IsNullOrEmpty(token))
            {
                await AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Directly use the validated principal instead of creating new one
                context.User = principal;
            }
            catch
            {
                // Clear invalid token
                context.Response.Cookies.Delete("authToken");
            }
        }
    }
}
