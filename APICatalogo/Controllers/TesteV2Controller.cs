using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiVersion}/teste")]
    [ApiController]
    public class TesteV2Controller : ControllerBase
    {
        [NonAction]
        public IActionResult Get()
        {
            return Content("<html><body><h2>Versão 2222 </h2></body></html>", "text/html");
        }
    }
}