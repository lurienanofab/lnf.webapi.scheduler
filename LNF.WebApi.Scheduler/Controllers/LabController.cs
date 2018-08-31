using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class LabController : ApiController
    {
        /// <summary>
        /// Gets all currently active labs
        /// </summary>
        /// <returns>A collection of LabItem objects</returns>
        [HttpGet, Route("lab/active")]
        public IEnumerable<LabItem> SelectActiveLabs()
        {
            return DA.Current.Query<Lab>().Where(x => x.IsActive).Model<LabItem>().OrderBy(x => x.LabDisplayName);
        }

        [Route("lab/{labId}")]
        public LabItem Get(int labId)
        {
            return DA.Current.Single<Lab>(labId).Model<LabItem>();
        }
    }
}
