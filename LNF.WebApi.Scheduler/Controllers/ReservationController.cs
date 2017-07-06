using LNF.Models.Scheduler;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ReservationController : ApiController
    {
        [Route("reservation/{reservationId}")]
        public ReservationModel GetReservation(int reservationId)
        {
            return Repository.GetReservation(reservationId).Model<ReservationModel>();
        }

        [Route("reservation")]
        public IEnumerable<ReservationModel> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            return Repository.GetReservations(sd, ed, clientId, resourceId, activityId, started, active).Model<ReservationModel>();
        }

        [HttpPut, Route("reservation/history")]
        public bool UpdateHistory([FromBody] ReservationHistoryUpdate model)
        {
            return Repository.UpdateReservationHistory(model.ClientID, model.ReservationID, model.AccountID, model.ChargeMultiplier, model.Notes, model.EmailClient);
        }
    }
}
