using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalTrainerApi.Controllers
{
    /// <summary>
    /// Kontroler autoryzacyjny.
    /// </summary>
    [Authorize("user")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class AuthorizedController : ControllerBase
    {
    }
}
