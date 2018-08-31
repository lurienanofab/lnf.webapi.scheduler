using LNF.Models.Scheduler;
using LNF.Repository;
using System.Collections.Generic;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessInfoLineController : ApiController
    {
        [Route("process-info-line")]
        public IEnumerable<ProcessInfoLineItem> Get()
        {
            return Repository.GetAllProcessInfoLines().Model<ProcessInfoLineItem>();
        }

        [Route("process-info-line/{processInfoLineId}")]
        public ProcessInfoLineItem Get(int processInfoLineId)
        {
            return Repository.GetProcessInfoLine(processInfoLineId).Model<ProcessInfoLineItem>();
        }

        [Route("process-info-line/process-info/{processInfoId}")]
        public IEnumerable<ProcessInfoLineItem> GetByProcessInfo(int processInfoId)
        {
            return Repository.GetProcessInfoLineByProcessInfo(processInfoId).Model<ProcessInfoLineItem>();
        }
    }
}
