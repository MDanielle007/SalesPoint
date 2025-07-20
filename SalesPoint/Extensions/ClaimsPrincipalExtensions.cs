using SalesPoint.Enums;
using System.Security.Claims;

namespace SalesPoint.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        public static UserRole GetUserRole(this ClaimsPrincipal user)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            if (Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                return role;
            }
            return UserRole.Cashier; // Default role
        }

        public static bool IsInRole(this ClaimsPrincipal user, UserRole role)
        {
            return user.GetUserRole() == role;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(UserRole.Admin);
        }

        public static bool IsManager(this ClaimsPrincipal user)
        {
            return user.IsInRole(UserRole.Manager);
        }

        public static bool IsEmployee(this ClaimsPrincipal user)
        {
            return user.IsInRole(UserRole.Cashier);
        }

        public static bool HasAccessTo(this ClaimsPrincipal user, params UserRole[] allowedRoles)
        {
            var userRole = user.GetUserRole();
            return allowedRoles.Contains(userRole);
        }
    }
}