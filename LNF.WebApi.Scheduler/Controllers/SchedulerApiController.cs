using LNF.DataAccess;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public abstract class SchedulerApiController : ApiController
    {
        protected IProvider Provider { get; }
        protected Repository Repository { get; }
        protected ISession DataSession => Repository.Session;

        public SchedulerApiController(IProvider provider)
        {
            Provider = provider;
            Repository = new Repository(Provider.DataAccess.Session);
        }
    }
}
