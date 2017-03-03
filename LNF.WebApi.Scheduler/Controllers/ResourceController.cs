using LNF.Models.Scheduler;
using LNF.Repository;
using System.Collections.Generic;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ResourceController : ApiController
    {
        /// <summary>
        /// Select a single resource
        /// </summary>
        /// <param name="resourceId">The unique id of the Resource</param>
        /// <returns>A ResourceModel item</returns>
        [Route("resource/{resourceId}")]
        public ResourceModel Get(int resourceId)
        {
            return DA.Scheduler.Resource.Single(resourceId).Model<ResourceModel>();
        }

        /// <summary>
        /// Select all active resources
        /// </summary>
        /// <returns>A collection of ResoureModel items</returns>
        [HttpGet, Route("resource/active")]
        public IEnumerable<ResourceModel> SelectActiveResources()
        {
            return DA.Scheduler.Resource.SelectActive().Model<ResourceModel>();
        }

        /// <summary>
        /// Select active resources by lab
        /// </summary>
        /// <param name="search">The lab for which to search. When omitted only resources from the default labs will be selected. Allowed values [default|all|#] where # is the LabID.</param>
        /// <returns>A collection of ResourceModel items</returns>
        [HttpGet, Route("resource/active/lab/{search?}")]
        public IEnumerable<ResourceModel> SelectByLab(string search = "default")
        {
            //Note that search = 0 and search = "all" have the same result.

            switch (search)
            {
                case "default":
                    return DA.Scheduler.Resource.SelectByLab(null).Model<ResourceModel>();
                case "all":
                    return DA.Scheduler.Resource.SelectByLab(0).Model<ResourceModel>();
                default:
                    return DA.Scheduler.Resource.SelectByLab(int.Parse(search)).Model<ResourceModel>();
            }
        }
    }
}
