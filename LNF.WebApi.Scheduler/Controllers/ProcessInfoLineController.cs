using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using LNF.Models.Scheduler;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessInfoLineController : ApiController
    {
        [Route("process-info-line")]
        public IEnumerable<ProcessInfoLineModel> Get()
        {
            return Repository.GetAllProcessInfoLines().Model<ProcessInfoLineModel>();
        }

        [Route("process-info-line/{processInfoLineId}")]
        public ProcessInfoLineModel Get(int processInfoLineId)
        {
            return Repository.GetProcessInfoLine(processInfoLineId).Model<ProcessInfoLineModel>();
        }

        [Route("process-info-line/process-info/{processInfoId}")]
        public IEnumerable<ProcessInfoLineModel> GetByProcessInfo(int processInfoId)
        {
            return Repository.GetProcessInfoLineByProcessInfo(processInfoId).Model<ProcessInfoLineModel>();
        }
    }
}
