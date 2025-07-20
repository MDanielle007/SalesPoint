using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SalesPoint.Enums;
using System.Security.Claims;

namespace SalesPoint.Authorization
{
    public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserRole[] _allowedRoles;

        public RoleAuthorizationAttribute(params UserRole[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRoleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userRoleClaim))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (System.Enum.TryParse<UserRole>(userRoleClaim, out var userRole))
            {
                if (!_allowedRoles.Contains(userRole))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            else
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}