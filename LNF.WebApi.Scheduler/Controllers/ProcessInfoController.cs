using LNF.Impl;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessInfoController : SchedulerApiController
    {
        public ProcessInfoController(IProvider provider) : base(provider) { }

        [HttpGet, Route("process-info")]
        public IEnumerable<IProcessInfo> GetMany()
        {
            return Repository.GetAllProcessInfos().CreateModels<IProcessInfo>();
        }

        [HttpGet, Route("process-info/{processInfoId}")]
        public IProcessInfo GetSingle([FromUri] int processInfoId)
        {
            var result = Repository.GetProcessInfo(processInfoId).CreateModel<IProcessInfo>();
            return result;
        }

        [HttpGet, Route("process-info-details/resource/{resourceId}")]
        public string GetProcessInfoDetailsByResource([FromUri] int resourceId) // Returns Structure retrieved from Resource
        {
            IEnumerable<ProcessInfoItem> pis = Repository.GetProcessInfosByResource(resourceId).CreateModels<ProcessInfoItem>();
            IEnumerable<int> pi_ids = pis.Select(x => x.ProcessInfoID);
            IEnumerable<ProcessInfoItem> pils = Repository.GetAllProcessInfoLinesFor(pi_ids).CreateModels<ProcessInfoItem>();
            object obj = new { ProcessInfos = pis, ProcessInfoLines = pils };
            return CommonTools.Utility.ToJson(obj);
        }

        [HttpGet, Route("process-info/resource/{resourceId}")]
        public string GetProcessInfosByResource([FromUri] int resourceId) // Returns Structure retrieved from Resource
        {
            IEnumerable<IProcessInfo> pis = Repository.GetProcessInfosByResource(resourceId).CreateModels<IProcessInfo>();
            return CommonTools.Utility.ToJson(pis);
        }

        [HttpGet, Route("process-info-line/resource/{resourceId}")]
        public string GetProcessInfoLinesByProcessInfos([FromUri] int resourceId)
        {
            IEnumerable<IProcessInfo> pis = Repository.GetProcessInfosByResource(resourceId).CreateModels<IProcessInfo>();
            int[] ids = pis.Select(x => x.ProcessInfoID).ToArray();
            IEnumerable<IProcessInfo> pils = Repository.GetAllProcessInfoLinesFor(ids).CreateModels<IProcessInfo>();
            return CommonTools.Utility.ToJson(pils);
        }

        [HttpGet, Route("reservation-process-infos/{reservationId}")]
        public string GetReservationProcessInfosByReservation([FromUri] int reservationId) // Retrieved from ReservationProcessInfo of a particular reservation
        {
            IEnumerable<IProcessInfo> rpis = Repository.GetReservationProcessInfosByReservation(reservationId).CreateModels<IProcessInfo>();
            return CommonTools.Utility.ToJson(rpis);
        }

        [HttpPost, Route("reservation-process-infos-save")]
        public string SaveReservationProcessInfosByReservation([FromBody] string[] jsons)
        {
            foreach (var value in jsons)
            {
                var rpi = JsonConvert.DeserializeObject<ReservationProcessInfo>(value);
                DataSession.SaveOrUpdate(rpi);
            }

            return "saved";
        }
    }
}
