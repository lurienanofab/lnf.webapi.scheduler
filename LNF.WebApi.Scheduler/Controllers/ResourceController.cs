using LNF.Models.Scheduler;
using LNF.Repository;
using System;
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

        [Route("resource/offset")]
        public IEnumerable<int> GetOffsets(int granularity)
        {
            var result = new List<int>();

            result.Add(0);

            if (granularity > 60)
                result.Add(1);

            if (granularity > 120)
                result.Add(2);

            return result;
        }

        [Route("resource/min-reservation-time")]
        public IEnumerable<ReservationTime> GetMinReservationTime(int granularity)
        {
            // Load Hours
            var result = new List<ReservationTime>();

            for (int i = 1; i <= 6; i++)
            {
                double minReservTime = i * granularity;
                TimeSpan ts = TimeSpan.FromMinutes(minReservTime);
                double day, hour, minute;

                //hour = Math.Floor(minReservTime / 60);
                //minute = minReservTime % 60;
                day = ts.Days;
                hour = ts.Hours;
                minute = ts.Minutes;

                string text = string.Empty;

                if (day > 0) text += string.Format("{0} day ", day);
                if (hour > 0) text += string.Format("{0} hr ", hour);
                if (minute > 0) text += string.Format("{0} min ", minute);

                result.Add(ReservationTime.Create(minReservTime, text.Trim()));
            }

            return result;
        }

        [Route("resource/max-reservation-time")]
        public IEnumerable<int> GetMaxReservTime(int granularity, int minReservTime)
        {
            //                                                                  1               2   3    6   12   24      days
            int[] maxReservTimeList = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 18, 24, 30, 36, 42, 48, 72, 144, 288, 576 }; //hours

            // the max is 576 because the max granularity is now 1440 (1440 * 24 / 60 = 576)

            int maxValue = Convert.ToInt32(granularity * 24 / 60);
            int minValue = Convert.ToInt32(Math.Ceiling((double)minReservTime / 60));

            var result = new List<int>();

            for (int i = 0; i < maxReservTimeList.Length; i++)
            {
                int h = maxReservTimeList[i];
                if (h > maxValue) break;
                if (h >= minValue && (h * 60) % granularity == 0)
                    result.Add(h);
            }

            return result;
        }

        [Route("resource/grace-period-hour")]
        public IEnumerable<int> GetGracePeriodHour(int granularity, int minReservTime)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            var stepSize = Convert.ToInt32(Math.Ceiling((double)granularity / 60));

            int minValue = 0;

            if (granularity >= 60) minValue = stepSize;

            for (int i = minValue; i <= maxHour; i += stepSize)
            {
                result.Add(i);
            }

            return result;
        }

        [Route("resource/grace-period-minute")]
        public IEnumerable<int> GetGracePeriodMinute(int granularity, int minReservTime, int gracePeriodHour)
        {
            var maxHour = Convert.ToInt32(Math.Floor((double)minReservTime / 60));

            var result = new List<int>();

            if (gracePeriodHour == maxHour && granularity < 60)
            {
                var maxMinute = minReservTime % 60;
                for (int i = 0; i <= maxMinute; i += granularity)
                {
                    result.Add(i);
                }
            }
            else
            {
                var count = Convert.ToInt32(Math.Ceiling(60 / (double)granularity));
                for (int i = 0; i < count; i++)
                {
                    var minute = granularity * i;
                    result.Add(minute);
                }
            }

            return result;
        }
    }
}
