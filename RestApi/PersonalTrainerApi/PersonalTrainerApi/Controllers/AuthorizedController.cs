using Microsoft.AspNetCore.Mvc;
using PersonalTrainerApi.Model.Authorization;

namespace PersonalTrainerApi.Controllers
{
    /// <summary>
    /// Kontroler autoryzacyjny.
    /// </summary>
    [Authorization]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class AuthorizedController : ControllerBase
    {
    }
}
