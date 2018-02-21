using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ReservationController : ApiController
    {
        [Route("reservation/{reservationId}")]
        public ReservationItem GetReservation(int reservationId)
        {
            return Repository.GetReservation(reservationId).Model<ReservationItem>();
        }

        [Route("reservation")]
        public IEnumerable<ReservationItem> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            return Repository.GetReservations(sd, ed, clientId, resourceId, activityId, started, active).Model<ReservationItem>();
        }

        [HttpPut, Route("reservation/history")]
        public bool UpdateHistory([FromBody] ReservationHistoryUpdate model)
        {
            return Repository.UpdateReservationHistory(model.ClientID, model.ReservationID, model.AccountID, model.ChargeMultiplier, model.Notes, model.EmailClient);
        }

        [HttpGet, Route("reservation/states")]
        public IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, bool inlab = true, int? cid = null, int? rid = null, int? reserver = null)
        {
            var invitees = DA.Current.Query<ReservationInvitee>().Select(x => new LNF.Models.Scheduler.ReservationInviteeItem
            {
                ReservationID = x.Reservation.ReservationID,
                ClientID = x.Invitee.ClientID,
                LName = x.Invitee.LName,
                FName = x.Invitee.FName
            }).ToList();

            var resourceClients = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == rid.GetValueOrDefault(x.ResourceID)).ToList();

            var clients = DA.Current.Query<ClientInfo>().Select(x => x.GetClientItem()).ToList();

            var query = DA.Current.Query<Reservation>()
                .Where(x => x.Resource.ResourceID == rid.GetValueOrDefault(x.Resource.ResourceID)
                    && x.Client.ClientID == reserver.GetValueOrDefault(x.Client.ClientID)
                    && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime < ed && x.ActualEndDateTime.HasValue && x.ActualEndDateTime > sd)));

            var items = query.Select(x => new ReservationItem()
            {
                ReservationID = x.ReservationID,
                ResourceID = x.Resource.ResourceID,
                ResourceName = x.Resource.ResourceName,
                ClientID = x.Client.ClientID,
                LName = x.Client.LName,
                FName = x.Client.FName,
                Privs = x.Client.Privs,
                BeginDateTime = x.BeginDateTime,
                EndDateTime = x.EndDateTime,
                ActualBeginDateTime = x.ActualBeginDateTime,
                ActualEndDateTime = x.ActualEndDateTime,
                Editable = x.Activity.Editable,
                IsFacilityDownTime = x.Activity.IsFacilityDownTime,
                MinCancelTime = x.Resource.MinCancelTime,
                MinReservTime = x.Resource.MinReservTime,
                StartEndAuth = (ClientAuthLevel)x.Activity.StartEndAuth
            }).OrderBy(x => x.ResourceName).ThenBy(x => x.BeginDateTime).ThenBy(x => x.EndDateTime).ToList();

            return items.Select(x =>
            {
                var reserverUser = clients.First(c => c.ClientID == x.ClientID);

                var currentUser = clients.First(c => c.ClientID == cid.GetValueOrDefault(x.ClientID));

                var args = ReservationStateArgs.Create(x, currentUser, inlab, invitees, resourceClients);

                try
                {
                    var state = ReservationUtility.GetReservationState(args);

                    return new ReservationStateItem()
                    {
                        ReservationID = x.ReservationID,
                        ResourceID = x.ResourceID,
                        ResourceName = x.ResourceName,
                        Reserver = reserverUser,
                        CurrentUser = currentUser,
                        State = state,
                        BeginDateTime = x.BeginDateTime,
                        EndDateTime = x.EndDateTime,
                        ActualBeginDateTime = x.ActualBeginDateTime,
                        ActualEndDateTime = x.ActualEndDateTime,
                        IsToolEngineer = args.IsToolEngineer,
                        IsReserver = args.IsReserver,
                        IsInvited = args.IsInvited,
                        IsAuthorized = args.IsAuthorized,
                        BeforeMinCancelTime = args.IsBeforeMinCancelTime
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("{0} ReservationID: {1}, Resource: {2} [{3}], Client: {4}, {5} [{6}]", ex.Message, x.ReservationID, x.ResourceName, x.ResourceID, x.LName, x.FName, x.ClientID));
                }
            });
        }
    }
}
