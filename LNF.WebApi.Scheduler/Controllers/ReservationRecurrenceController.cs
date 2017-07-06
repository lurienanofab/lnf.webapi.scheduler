using LNF.Models.Scheduler;
using LNF.Repository;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ReservationRecurrenceController : ApiController
    {
        /// <summary>
        /// Retrieves a single ReservationRecurrence.
        /// </summary>
        /// <param name="reservationRecurrenceId">The id for a single reservation recurrence.</param>
        /// <returns>A ReservationRecurrenceModel item.</returns>
        [Route("reservation-recurrence/{reservationRecurrenceId}")]
        public ReservationRecurrenceModel GetReservation(int reservationRecurrenceId)
        {
            return Repository.GetReservationRecurrence(reservationRecurrenceId).Model<ReservationRecurrenceModel>();
        }
    }
}
