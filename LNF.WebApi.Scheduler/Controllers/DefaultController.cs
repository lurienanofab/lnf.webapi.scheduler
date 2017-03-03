using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class DefaultController : ApiController
    {
        [AllowAnonymous, Route("")]
        public string Get()
        {
            return "scheduler-api";
        }
    }
}
