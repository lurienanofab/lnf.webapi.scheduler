using LNF.Impl;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessTechController : SchedulerApiController
    {
        public ProcessTechController(IProvider provider) : base(provider) { }

        [HttpGet, Route("proctech")]
        public IEnumerable<IProcessTech> GetMany(bool? active = null, int labId = 0)
        {
            IEnumerable<IProcessTech> result;

            if (active.HasValue)
                result = Repository.GetProcessTechs(active.Value, labId).CreateModels<IProcessTech>();
            else
                result = Repository.GetProcessTechs(labId).CreateModels<IProcessTech>();

            return result.OrderBy(x => x.ProcessTechName).ToArray();
        }

        [HttpGet, Route("proctech/{processTechId}")]
        public IProcessTech GetSingle([FromUri] int processTechId)
        {
            return Repository.GetProcessTech(processTechId).CreateModel<IProcessTech>();
        }
    }
}
