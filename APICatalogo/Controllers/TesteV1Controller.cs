using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [Route("api/{v:apiVersion}/teste")]
    [ApiController]
    public class TesteV1Controller : ControllerBase
    {
        [NonAction]
        public IActionResult Get()
        {
            return Content("<html><body><h2> Versão 1 </h2></body></html>", "text/html");
        }
    }
}