using LNF.Models.Scheduler;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ProcessInfoController : ApiController
    {
        [Route("process-info")]
        public IEnumerable<ProcessInfoItem> Get()
        {
            return Repository.GetAllProcessInfos().Model<ProcessInfoItem>();
        }

        [Route("process-info/{processInfoId}")]
        public ProcessInfoItem Get(int processInfoId)
        {
            var result = Repository.GetProcessInfo(processInfoId).Model<ProcessInfoItem>();
            return result;
        }
        [Route("process-info-details/resource/{resourceId}")]
        public string GetProcessInfoDetailsByResource(int resourceId) // Returns Structure retrieved from Resource
        {
            IEnumerable<ProcessInfoItem> pis = Repository.GetProcessInfosByResource(resourceId).Model<ProcessInfoItem>();
            IEnumerable<int> pi_ids = pis.Select(x => x.ProcessInfoID);
            IEnumerable<ProcessInfoItem> pils = Repository.GetAllProcessInfoLinesFor(pi_ids).Model<ProcessInfoItem>();
            Object obj = new { ProcessInfos = pis, ProcessInfoLines = pils };
            return LNF.CommonTools.Utility.ToJson(obj);
        }
        [Route("process-info/resource/{resourceId}")]
        public string GetProcessInfosByResource(int resourceId) // Returns Structure retrieved from Resource
        {
            IEnumerable<ProcessInfoItem> pis = Repository.GetProcessInfosByResource(resourceId).Model<ProcessInfoItem>();
            return CommonTools.Utility.ToJson(pis);
        }
        [Route("process-info-line/resource/{resourceId}")]
        public string GetProcessInfoLinesByProcessInfos(int resourceId)
        {
            IEnumerable<ProcessInfoItem> pis = Repository.GetProcessInfosByResource(resourceId).Model<ProcessInfoItem>();
            IEnumerable<int> pi_ids = pis.Select(x=>x.ProcessInfoID);
            IEnumerable<ProcessInfoItem> pils = Repository.GetAllProcessInfoLinesFor(pi_ids).Model<ProcessInfoItem>();

            return CommonTools.Utility.ToJson(pils);
        }
        [Route("reservation-process-infos/{reservationId}")]
        public string GetReservationProcessInfosByReservation(int reservationId) // Retrieved from ReservationProcessInfo of a particular reservation
        {
            IEnumerable<ProcessInfoItem> rpis = Repository.GetReservationProcessInfosByReservation(reservationId).Model<ProcessInfoItem>();
            return CommonTools.Utility.ToJson(rpis);
        }
        [Route("reservation-process-infos-save/")]
        public string SaveReservationProcessInfosByReservation(string[] jsonrpis)
        {
            //???????????????????????????????????Implement json to Object...
            IEnumerable<LNF.Repository.Scheduler.ReservationProcessInfo> rpis = null;
            foreach(LNF.Repository.Scheduler.ReservationProcessInfo rpi in rpis)
            {
                DA.Current.SaveOrUpdate(rpi);
            }
            return "saved";
        }
    }
}
