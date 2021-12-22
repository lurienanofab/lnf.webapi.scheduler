using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ReservationController : SchedulerApiController
    {
        public ReservationController(IProvider provider) : base(provider) { }

        [HttpGet, Route("reservation")]
        public IEnumerable<IReservation> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            return Provider.Scheduler.Reservation.GetReservations(sd, ed, clientId, resourceId, activityId, started, active);
        }

        [HttpGet, Route("reservation/with-invitees")]
        public IEnumerable<IReservationWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            return Provider.Scheduler.Reservation.GetReservationsWithInvitees(sd, ed, clientId, resourceId, activityId, started, active);
        }

        [HttpGet, Route("reservation/{reservationId}")]
        public IReservation GetReservation([FromUri] int reservationId)
        {
            return Provider.Scheduler.Reservation.GetReservation(reservationId);
        }

        [HttpGet, Route("reservation/{reservationId}/with-invitees")]
        public IReservationWithInvitees GetReservationWithInvitees([FromUri] int reservationId)
        {
            return Provider.Scheduler.Reservation.GetReservationWithInvitees(reservationId);
        }

        [HttpPut, Route("reservation/history")]
        public bool UpdateReservationHistory([FromBody] ReservationHistoryUpdate model)
        {
            return Provider.Scheduler.Reservation.UpdateReservationHistory(model);
        }

        [HttpGet, Route("reservation/{reservationId}/invitees")]
        public IEnumerable<IReservationInviteeItem> GetReservationInvitees([FromUri] int reservationId)
        {
            return Provider.Scheduler.Reservation.GetInvitees(reservationId);
        }

        [HttpGet, Route("reservation/states")]
        public IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null)
        {
            return Provider.Scheduler.Reservation.GetReservationStates(sd, ed, kioskIp, clientId, resourceId, reserverId);
        }
    }
}
