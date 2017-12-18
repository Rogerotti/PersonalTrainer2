using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalTrainerApi.Model.Authorization
{
    /// <summary>
    ///  Atrybut do autoryzacji
    /// </summary>
    public class AuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        /// <summary>
        /// Wywołany w momencie autoryzacji
        /// </summary>
        /// <param name="context">Kontekst</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.Any(x => x.Key.Equals("secure-token")))
            {
                //context.Result = new UnauthorizedResult();
            }
        }
    }
}
