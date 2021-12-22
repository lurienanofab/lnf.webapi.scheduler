using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class LabController : SchedulerApiController
    {
        public LabController(IProvider provider) : base(provider) { }

        /// <summary>
        /// Gets all currently active labs
        /// </summary>
        /// <returns>A collection of LabItem objects</returns>
        [HttpGet, Route("lab/active")]
        public IEnumerable<ILab> SelectActiveLabs()
        {
            return Provider.Scheduler.Resource.GetLabs().Where(x => x.BuildingIsActive && x.LabIsActive).OrderBy(x => x.LabDisplayName).ToList();
        }

        [Route("lab/{labId}")]
        public ILab Get(int labId)
        {
            return Provider.Scheduler.Resource.GetLab(labId);
        }
    }
}
