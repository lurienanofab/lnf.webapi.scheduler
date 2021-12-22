using LNF.Scheduler;
using System.Collections.Generic;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ResourceController : SchedulerApiController
    {
        public ResourceController(IProvider provider) : base(provider) { }

        /// <summary>
        /// Select a single resource
        /// </summary>
        /// <param name="resourceId">The unique id of the Resource</param>
        /// <returns>A ResourceModel item</returns>
        [HttpGet, Route("resource/{resourceId}")]
        public IResource GetResource(int resourceId)
        {
            return Provider.Scheduler.Resource.GetResource(resourceId);
        }

        /// <summary>
        /// Select all active resources
        /// </summary>
        /// <returns>A collection of ResoureModel items</returns>
        [HttpGet, Route("resource")]
        public IEnumerable<IResource> GetResources()
        {
            return Provider.Scheduler.Resource.GetResources();
        }

        /// <summary>
        /// Select all active resources
        /// </summary>
        /// <returns>A collection of ResoureModel items</returns>
        [HttpPost, Route("resource")]
        public IEnumerable<IResource> GetResourcesById([FromBody] int[] ids)
        {
            return Provider.Scheduler.Resource.GetResources(ids);
        }

        /// <summary>
        /// Select all active resources
        /// </summary>
        /// <returns>A collection of ResoureModel items</returns>
        [HttpGet, Route("resource/active")]
        public IEnumerable<IResource> GetActiveResources()
        {
            return Provider.Scheduler.Resource.GetActiveResources();
        }

        [HttpGet, Route("resource/active/list")]
        public IEnumerable<GenericListItem> AllActiveResources()
        {
            return Provider.Scheduler.Resource.AllActiveResources();
        }

        /// <summary>
        /// Select active resources by lab
        /// </summary>
        /// <param name="search">The lab for which to search. When omitted only resources from the default labs will be selected. Allowed values [default|all|#] where # is the LabID.</param>
        /// <returns>A collection of ResourceModel items</returns>
        [HttpGet, Route("resource/active/lab/{search?}")]
        public IEnumerable<IResource> SelectByLab(string search = "default")
        {
            //Note that search = 0 and search = "all" have the same result.

            int? labId;

            if (search == "all" || search == "0")
                labId = null;
            else
                labId = search == "default" ? 1 : int.Parse(search);

            return SelectByLab(labId);
        }

        [HttpGet, Route("resource/active/lab")]
        public IEnumerable<IResource> SelectByLab(int? labId = null)
        {
            return Provider.Scheduler.Resource.GetResourcesByLab(labId);
        }

        [Route("resource/offset")]
        public IEnumerable<int> GetOffsets(int granularity)
        {
            return Provider.Scheduler.Resource.GetOffsets(granularity);
        }

        [Route("resource/min-reservation-time")]
        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            return Provider.Scheduler.Resource.GetMinReservationTime(granularity);
        }

        [Route("resource/max-reservation-time")]
        public IEnumerable<int> GetMaxReservationTime(int granularity, int minReservTime)
        {
            return Provider.Scheduler.Resource.GetMaxReservationTime(granularity, minReservTime);
        }

        [Route("resource/grace-period-hour")]
        public IEnumerable<int> GetGracePeriodHour(int granularity, int minReservTime)
        {
            return Provider.Scheduler.Resource.GetGracePeriodHour(granularity, minReservTime);
        }

        [Route("resource/grace-period-minute")]
        public IEnumerable<int> GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            return Provider.Scheduler.Resource.GetGracePeriodMinute(granularity, minReservTime, gracePeriodHour);
        }
    }
}
