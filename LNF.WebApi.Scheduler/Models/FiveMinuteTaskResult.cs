namespace LNF.WebApi.Scheduler.Models
{
    public class FiveMinuteTaskResult
    {
        public int PastEndableRepairReservations { get; set; }
        public int AutoEndReservations { get; set; }
        public int PastUnstartedReservations { get; set; }
    }
}