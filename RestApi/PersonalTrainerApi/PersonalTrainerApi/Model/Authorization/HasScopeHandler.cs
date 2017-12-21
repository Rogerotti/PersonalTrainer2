using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalTrainerApi.Model.Authorization
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            if (filterContext == null)
                return Task.CompletedTask;

            var authorizationToken = filterContext.HttpContext.Request.Headers["Authorization"];
            if (authorizationToken.Count == 0)
                return Task.CompletedTask;

            var tokenString = authorizationToken.ToString();
            var token = tokenString.Split('\"')[1];
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            if(!jwtToken.Claims.Any()) return Task.CompletedTask;

            if (!jwtToken.Claims.Any(x => x.Type.Contains("userType") && x.Value == requirement.Scope))
                return Task.CompletedTask;

            if (!jwtToken.Claims.Any(x => x.Type == "scope" && x.Value == requirement.Issuer))
                return Task.CompletedTask;

            if (!jwtToken.Claims.Any(x => x.Type == "expireDate" && DateTime.Parse(x.Value) >= DateTime.Now ))
                return Task.CompletedTask;

          
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
