using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

//using LNF.CommonTools;

namespace LNF.WebApi.Scheduler
{
    public static class Repository
    {
        public static IEmailManager EmailManager => DA.Use<IEmailManager>();

        public static ProcessInfo[] GetAllProcessInfos()
        {
            return DA.Current.Query<ProcessInfo>().ToArray();
        }

        public static List<ProcessInfo> GetProcessInfosByResource(int resourceId)
        {
            List<ProcessInfo> pis = DA.Current.Query<ProcessInfo>().Where(x => x.Resource.ResourceID == resourceId).ToList<ProcessInfo>().ToList();
            return pis;
        }
        public static List<ProcessInfoLine> GetAllProcessInfoLinesFor(IEnumerable<int> pi_ids)
        {
            List<ProcessInfoLine> pils = new List<ProcessInfoLine>();
            foreach (int pid in pi_ids)
            {
                //returns a list of pils(of particular ProcessInfoID)
                IEnumerable<ProcessInfoLine> tpils = DA.Current.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == pid);
                pils.AddRange(tpils);
            }

            return pils.Distinct().ToList();
        }

        public static IList<ReservationProcessInfo> GetReservationProcessInfosByReservation(int reservationId)
        {
            IList<ReservationProcessInfo> rpis = DA.Current.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == reservationId).ToList();
            return rpis;
        }

        //public static void SaveReservationProcessInfosByReservation(){}
        public static ProcessInfo GetProcessInfo(int processInfoId)
        {
            return DA.Current.Single<ProcessInfo>(processInfoId);
        }

        public static ReservationRecurrence GetReservationRecurrence(int reservationRecurrenceId)
        {
            return DA.Current.Single<ReservationRecurrence>(reservationRecurrenceId);
        }

        internal static ProcessTech GetProcessTech(int processTechId)
        {
            return DA.Current.Single<ProcessTech>(processTechId);
        }

        public static ProcessInfoLine[] GetAllProcessInfoLines()
        {
            return DA.Current.Query<ProcessInfoLine>().ToArray();
        }

        public static ProcessInfoLine GetProcessInfoLine(int processInfoLineId)
        {
            return DA.Current.Single<ProcessInfoLine>(processInfoLineId);
        }

        public static ProcessInfoLine[] GetProcessInfoLineByProcessInfo(int processInfoId)
        {
            return DA.Current.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).ToArray();
        }

        public static IQueryable<Reservation> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = DA.Current.Query<Reservation>().Where(x =>
                    (x.BeginDateTime < ed && x.EndDateTime > sd)
                    || ((x.ActualBeginDateTime == null ? false : x.ActualBeginDateTime < ed) && (x.ActualEndDateTime == null ? true : x.ActualEndDateTime > sd)));

            if (clientId != 0)
                query = query.Where(x => x.Client.ClientID == clientId);

            if (resourceId != 0)
                query = query.Where(x => x.Resource.ResourceID == resourceId);

            if (activityId != 0)
                query = query.Where(x => x.Activity.ActivityID == activityId);

            if (started.HasValue)
                query = query.Where(x => x.IsStarted == started.Value);

            if (active.HasValue)
                query = query.Where(x => x.IsActive == active.Value);

            return query;
        }

        public static ProcessTech[] GetProcessTechs(int labId = 0)
        {
            if (labId == 0)
                return DA.Current.Query<ProcessTech>().ToArray();
            else
                return DA.Current.Query<ProcessTech>().Where(x => x.Lab.LabID == labId).ToArray();
        }

        public static ProcessTech[] GetProcessTechs(bool active, int labId = 0)
        {
            if (labId == 0)
                return DA.Current.Query<ProcessTech>().Where(x => x.IsActive == active).ToArray();
            else
                return DA.Current.Query<ProcessTech>().Where(x => x.IsActive == active && x.Lab.LabID == labId).ToArray();
        }

        public static bool UpdateReservationHistory(int clientId, int reservationId, int? accountId, double? chargeMultiplier, string notes, bool emailClient)
        {
            //this does not update billing, that needs to be done separately

            bool saveReservation = false;
            bool addReservationHistory = false;
            bool sendEmail = false;
            double forgivenAmount = 0;

            Reservation rsvMain = DA.Current.Single<Reservation>(reservationId);

            if (rsvMain == null) return false;

            Reservation rsv = rsvMain;

            while (true)  // this will loop through all linked reservations and update them.
            {
                if (accountId.HasValue && rsv.Account.AccountID != accountId.Value)
                {
                    Account acct = DA.Current.Single<Account>(accountId);
                    if (acct == null) return false;
                    rsv.Account = acct;
                    saveReservation = true;
                    addReservationHistory = true;
                }

                if (chargeMultiplier.HasValue && rsv.ChargeMultiplier != chargeMultiplier.Value)
                {
                    rsv.ChargeMultiplier = chargeMultiplier.Value;
                    saveReservation = true;
                    addReservationHistory = true;
                    sendEmail = emailClient;
                    forgivenAmount = (1 - chargeMultiplier.Value) * 100.0;
                }

                if (notes != null && notes != rsv.Notes)
                {
                    rsv.Notes = notes;
                    saveReservation = true;
                    // no need to add ReservationHistory for a notes change
                }

                if (saveReservation)
                {
                    DA.Current.SaveOrUpdate(rsv);

                    if (addReservationHistory)
                    {
                        //need to add ReservationHistory
                        DA.Current.SaveOrUpdate(new ReservationHistory()
                        {
                            Reservation = rsv,
                            UserAction = "UpdateReservationHistory",
                            ActionSource = "LNF.WebApi.Scheduler.Repository",
                            Account = rsv.Account,
                            BeginDateTime = rsv.BeginDateTime,
                            EndDateTime = rsv.EndDateTime,
                            ChargeMultiplier = rsv.ChargeMultiplier,
                            ModifiedByClientID = clientId,
                            ModifiedDateTime = DateTime.Now
                        });
                    }
                }

                ReservationHistory rh1 = DA.Current.Query<ReservationHistory>().FirstOrDefault(x => x.Reservation == rsv && x.LinkedReservationID != null && x.UserAction == ReservationHistory.INSERT_FOR_MODIFICATION);

                if (rh1 == null)  // if there are no more linked reservations
                    break;
                else
                {
                    rsv = DA.Current.Query<Reservation>().FirstOrDefault(x => x.ReservationID == rh1.LinkedReservationID.Value);

                    if (rsv == null)
                    {
                        // this should never happen
                        break;
                    }
                }
            }

            if (sendEmail)
                EmailManager.EmailOnForgiveCharge(rsv, forgivenAmount, true, clientId);

            return true;
        }

        public static Reservation GetReservation(int reservationId)
        {
            return DA.Current.Single<Reservation>(reservationId);
        }
    }
}