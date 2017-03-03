using LNF.Models.Scheduler;
using LNF.Repository;
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
        /// <returns>A collection of LabModel items</returns>
        [HttpGet, Route("lab/active")]
        public IEnumerable<LabModel> SelectActiveLabs()
        {
            return DA.Scheduler.Lab.SelectActive().Model<LabModel>().OrderBy(x => x.LabDisplayName);
        }

        [Route("lab/{labId}")]
        public LabModel Get(int labId)
        {
            return DA.Scheduler.Lab.Single(labId).Model<LabModel>();
        }
    }
}
