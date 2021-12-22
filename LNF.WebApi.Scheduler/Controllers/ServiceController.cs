using LNF.Billing;
using LNF.Billing.Process;
using LNF.Billing.Reports;
using LNF.CommonTools;
using LNF.Data;
using LNF.PhysicalAccess;
using LNF.Scheduler;
using LNF.Scheduler.Service;
using System;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ServiceController : SchedulerApiController
    {
        public ServiceController(IProvider provider) : base(provider) { }

        [HttpGet, Route("service/task-5min")]
        public FiveMinuteTaskResult RunFiveMinuteTask()
        {
            // every five minutes tasks

            var pastEndableRepairReservations = Provider.Scheduler.Reservation.SelectPastEndableRepair();
            var autoEndReservations = Provider.Scheduler.Reservation.SelectAutoEnd();
            var pastUnstartedReservations = Provider.Scheduler.Reservation.SelectPastUnstarted();

            var util = Reservations.Create(Provider, DateTime.Now);

            return new FiveMinuteTaskResult
            {
                EndRepairReservationsProcessResult = util.HandleRepairReservations(pastEndableRepairReservations),
                EndUnstartedReservationsProcessResult = util.HandleUnstartedReservations(pastUnstartedReservations),
                HandleAutoEndReservationsProcessResult = util.HandleAutoEndReservations(autoEndReservations)
            };
        }

        [HttpGet, Route("service/task-daily")]
        public DailyTaskResult RunDailyTask(bool noEmail = false)
        {
            DateTime ed = DateTime.Now.Date.AddDays(-1); //must be yesterday
            DateTime period = ed.FirstOfMonth();

            var bp = Provider.Billing.Process;

            var expiringClients = ResourceClients.SelectExpiringClients();
            var expiringEveryone = ResourceClients.SelectExpiringEveryone();
            var expiredClients = ResourceClients.SelectExpiredClients();
            var expiredEveryone = ResourceClients.SelectExpiredEveryone();

            var result = new DailyTaskResult
            {
                // Check client tool auths
                CheckExpiringClientsProcessResult = ResourceClients.CheckExpiringClients(expiringClients, expiringEveryone, noEmail),
                CheckExpiredClientsProcessResult = ResourceClients.CheckExpiredClients(expiredClients, expiredEveryone, noEmail),

                // Update Data and DataClean tables
                DataUpdateProcessResult = bp.Update(new UpdateCommand
                {
                    ClientID = 0,
                    Period = period,
                    BillingTypes = BillingCategory.Tool | BillingCategory.Room | BillingCategory.Store
                }),

                BillingProcessStep1Result = bp.Step1(new Step1Command
                {
                    ClientID = 0,
                    Record = 0,
                    Period = period,
                    Delete = true,
                    IsTemp = true,
                    BillingCategory = BillingCategory.Tool | BillingCategory.Room | BillingCategory.Store
                }),
            };

            return result;
        }

        [HttpGet, Route("service/task-monthly")]
        public MonthlyTaskResult RunMonthlyTask(bool noEmail = false)
        {
            // This is run at midnight on the 1st of the month. So the period should be the 1st of the previous month.
            DateTime period = DateTime.Now.FirstOfMonth().AddMonths(-1);

            var br = Provider.Billing.Report;
            var bp = Provider.Billing.Process;

            // This sends apportionment emails to clients
            var userApportionmentReportCount = br.SendUserApportionmentReport(new UserApportionmentReportOptions
            {
                Period = period,
                NoEmail = noEmail,
                Message = null
            }).TotalEmailsSent;

            // 2008-04-30
            // Monthly financial report
            var financialManagerReportCount = br.SendFinancialManagerReport(new FinancialManagerReportOptions
            {
                ClientID = 0,
                ManagerOrgID = 0,
                Period = period,
                IncludeManager = !noEmail,
                Message = null
            }).TotalEmailsSent;

            // This sends room expiration emails
            RoomAccessExpirationCheck roomAccessExpirationCheck = new RoomAccessExpirationCheck();
            var roomAccessExpirationCheckCount = roomAccessExpirationCheck.Run();

            var result = new MonthlyTaskResult
            {
                UserApportionmentReportCount = userApportionmentReportCount,
                FinancialManagerReportCount = financialManagerReportCount,
                RoomAccessExpirationCheckCount = roomAccessExpirationCheckCount,

                //2009-08-01 Populate the BillingTables
                BillingProcessStep1Result = bp.Step1(new Step1Command
                {
                    ClientID = 0,
                    Record = 0,
                    Period = period,
                    Delete = true,
                    IsTemp = false,
                    BillingCategory = BillingCategory.Tool | BillingCategory.Room | BillingCategory.Store
                }),

                PopulateSubsidyBillingProcessResult = bp.Step4(new Step4Command
                {
                    ClientID = 0,
                    Period = period,
                    Command = "subsidy"
                })
            };

            return result;
        }

        [HttpGet, Route("service/expiring-cards")]
        public DataFeedResult GetExpiringCards()
        {
            RoomAccessExpirationCheck check = new RoomAccessExpirationCheck();
            var dataFeed = check.GetDataFeed();
            return dataFeed;
        }

        [HttpGet, Route("service/expiring-cards/email")]
        public int SendExpiringCardsEmail()
        {
            RoomAccessExpirationCheck roomAccessExpirationCheck = new RoomAccessExpirationCheck();
            int count = roomAccessExpirationCheck.Run();
            return count;
        }
    }
}
