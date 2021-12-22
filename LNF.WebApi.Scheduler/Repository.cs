using LNF.DataAccess;
using LNF.Impl.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.WebApi.Scheduler
{
    public class Repository
    {
        public ISession Session { get; }

        public Repository(ISession session)
        {
            Session = session;
        }

        public ProcessInfo[] GetAllProcessInfos()
        {
            return Session.Query<ProcessInfo>().ToArray();
        }

        public List<ProcessInfo> GetProcessInfosByResource(int resourceId)
        {
            List<ProcessInfo> pis = Session.Query<ProcessInfo>().Where(x => x.ResourceID == resourceId).ToList<ProcessInfo>().ToList();
            return pis;
        }

        public List<ProcessInfoLine> GetAllProcessInfoLinesFor(IEnumerable<int> pi_ids)
        {
            List<ProcessInfoLine> pils = new List<ProcessInfoLine>();
            foreach (int pid in pi_ids)
            {
                //returns a list of pils(of particular ProcessInfoID)
                IEnumerable<ProcessInfoLine> tpils = Session.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == pid);
                pils.AddRange(tpils);
            }

            return pils.Distinct().ToList();
        }

        public IList<ReservationProcessInfo> GetReservationProcessInfosByReservation(int reservationId)
        {
            IList<ReservationProcessInfo> rpis = Session.Query<ReservationProcessInfo>().Where(x => x.ReservationID == reservationId).ToList();
            return rpis;
        }

        public ProcessInfo GetProcessInfo(int processInfoId)
        {
            return Session.Single<ProcessInfo>(processInfoId);
        }

        public ReservationRecurrence GetReservationRecurrence(int reservationRecurrenceId)
        {
            return Session.Single<ReservationRecurrence>(reservationRecurrenceId);
        }

        public ProcessTech GetProcessTech(int processTechId)
        {
            return Session.Single<ProcessTech>(processTechId);
        }

        public ProcessInfoLine[] GetAllProcessInfoLines()
        {
            return Session.Query<ProcessInfoLine>().ToArray();
        }

        public ProcessInfoLine GetProcessInfoLine(int processInfoLineId)
        {
            return Session.Single<ProcessInfoLine>(processInfoLineId);
        }

        public ProcessInfoLine[] GetProcessInfoLineByProcessInfo(int processInfoId)
        {
            return Session.Query<ProcessInfoLine>().Where(x => x.ProcessInfoID == processInfoId).ToArray();
        }

        public ProcessTech[] GetProcessTechs(int labId = 0)
        {
            if (labId == 0)
                return Session.Query<ProcessTech>().ToArray();
            else
                return Session.Query<ProcessTech>().Where(x => x.Lab.LabID == labId).ToArray();
        }

        public ProcessTech[] GetProcessTechs(bool active, int labId = 0)
        {
            if (labId == 0)
                return Session.Query<ProcessTech>().Where(x => x.IsActive == active).ToArray();
            else
                return Session.Query<ProcessTech>().Where(x => x.IsActive == active && x.Lab.LabID == labId).ToArray();
        }

        public Reservation GetReservation(int reservationId)
        {
            return Session.Single<Reservation>(reservationId);
        }
    }
}