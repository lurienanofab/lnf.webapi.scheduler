using LNF.Impl;
using LNF.Scheduler;
using System.Collections.Generic;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessInfoLineController : SchedulerApiController
    {
        public ProcessInfoLineController(IProvider provider) : base(provider) { }

        [HttpGet, Route("process-info-line")]
        public IEnumerable<IProcessInfoLine> GetMany()
        {
            return Repository.GetAllProcessInfoLines().CreateModels<IProcessInfoLine>();
        }

        [HttpGet, Route("process-info-line/{processInfoLineId}")]
        public IProcessInfoLine GetSingle([FromUri] int processInfoLineId)
        {
            return Repository.GetProcessInfoLine(processInfoLineId).CreateModel<IProcessInfoLine>();
        }

        [HttpGet, Route("process-info-line/process-info/{processInfoId}")]
        public IEnumerable<IProcessInfoLine> GetByProcessInfo([FromUri] int processInfoId)
        {
            return Repository.GetProcessInfoLineByProcessInfo(processInfoId).CreateModels<IProcessInfoLine>();
        }
    }
}
