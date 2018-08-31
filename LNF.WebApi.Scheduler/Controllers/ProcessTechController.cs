using LNF.Models.Scheduler;
using LNF.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessTechController : ApiController
    {
        [Route("proctech")]
        public IEnumerable<ProcessTechItem> Get(bool? active = null, int labId = 0)
        {
            IEnumerable<ProcessTechItem> result;

            if (active.HasValue)
                result = Repository.GetProcessTechs(active.Value, labId).Model<ProcessTechItem>();
            else
                result = Repository.GetProcessTechs(labId).Model<ProcessTechItem>();

            return result.OrderBy(x => x.ProcessTechName).ToArray();
        }

        [Route("proctech/{processTechId}")]
        public ProcessTechItem Get(int processTechId)
        {
            return Repository.GetProcessTech(processTechId).Model<ProcessTechItem>();
        }
    }
}
