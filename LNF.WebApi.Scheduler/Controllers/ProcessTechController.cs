using LNF.Models.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessTechController : ApiController
    {
        [Route("proctech")]
        public IEnumerable<ProcessTechModel> Get(bool? active = null, int labId = 0)
        {
            IEnumerable<ProcessTechModel> result;

            if (active.HasValue)
                result = Repository.GetProcessTechs(active.Value, labId).Model<ProcessTechModel>();
            else
                result = Repository.GetProcessTechs(labId).Model<ProcessTechModel>();

            return result.OrderBy(x => x.ProcessTechName).ToArray();
        }

        [Route("proctech/{processTechId}")]
        public ProcessTechModel Get(int processTechId)
        {
            return Repository.GetProcessTech(processTechId).Model<ProcessTechModel>();
        }
    }
}
