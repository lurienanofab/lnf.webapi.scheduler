using LNF.CommonTools;
using System;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class InterlockController : ApiController
    {
        /// <summary>
        /// Gets the current interlock state of a resource
        /// </summary>
        /// <param name="resourceId">The resource id for which to get the current interlock state</param>
        /// <returns></returns>
        [HttpGet, Route("interlock/{resourceId}")]
        public bool GetState(int resourceId)
        {
            var result = WagoInterlock.GetPointState(resourceId);
            return result;
        }

        /// <summary>
        /// Sets the current interlock state of a resource
        /// </summary>
        /// <param name="resourceId">The resource id for which to set the current interlock state</param>
        /// <param name="state">The state to which the resource interlock should be set - allowed values are [on|off]</param>
        /// <param name="duration">The duration after which the interlock state will revert back to the previous state - the state will not revert when zero or omitted</param>
        /// <returns></returns>
        [HttpGet, Route("interlock/{resourceId}/{state}")]
        public bool SetStateOn(int resourceId, string state, int duration = 0)
        {
            if (new[] { "on", "off" }.Contains(state))
            {
                uint d = Convert.ToUInt32(Math.Max(0, duration));
                WagoInterlock.ToggleInterlock(resourceId, state == "on", d);
                return GetState(resourceId);
            }
            else
                throw new ArgumentException("Invalid state. Allowed values are on|off", "state");
        }
    }
}
