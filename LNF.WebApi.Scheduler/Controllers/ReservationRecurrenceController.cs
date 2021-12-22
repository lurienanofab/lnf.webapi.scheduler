using LNF.Impl;
using LNF.Scheduler;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ReservationRecurrenceController : SchedulerApiController
    {
        public ReservationRecurrenceController(IProvider provider) : base(provider) { }

        /// <summary>
        /// Retrieves a single ReservationRecurrence.
        /// </summary>
        /// <param name="reservationRecurrenceId">The id for a single reservation recurrence.</param>
        /// <returns>A ReservationRecurrenceModel item.</returns>
        [HttpGet, Route("reservation-recurrence/{reservationRecurrenceId}")]
        public IReservationRecurrence GetReservation([FromUri] int reservationRecurrenceId)
        {
            return Repository.GetReservationRecurrence(reservationRecurrenceId).CreateModel<IReservationRecurrence>();
        }
    }
}
