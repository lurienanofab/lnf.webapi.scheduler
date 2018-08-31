﻿using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Scheduler;
using LNF.WebApi.Scheduler.Models;
using OnlineServices.Api.Billing;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace LNF.WebApi.Scheduler.Controllers
{
    public class ServiceController : ApiController
    {
        public IReservationManager ReservationManager => DA.Use<IReservationManager>();

        [HttpGet, Route("service/task-5min")]
        public async Task<FiveMinuteTaskResult> RunFiveMinuteTask()
        {
            // every five minutes tasks
            var pastEndableRepairReservations = ReservationManager.SelectPastEndableRepair();
            ReservationManager.EndRepairReservations(pastEndableRepairReservations);

            var autoEndReservations = ReservationManager.SelectAutoEnd();
            await ReservationManager.EndAutoEndReservations(autoEndReservations);

            var pastUnstartedReservations = ReservationManager.SelectPastUnstarted();
            ReservationManager.EndUnstartedReservations(pastUnstartedReservations);

            return new FiveMinuteTaskResult()
            {
                PastEndableRepairReservations = pastEndableRepairReservations.Count,
                AutoEndReservations = autoEndReservations.Count,
                PastUnstartedReservations = pastUnstartedReservations.Count
            };
        }

        [HttpGet, Route("service/task-daily")]
        public async Task<bool> RunDailyTask(bool noEmail = false)
        {
            bool result = true;

            // Check client tool auths
            ResourceClientUtility.CheckExpiringClients(ResourceClientUtility.SelectExpiringClients(), ResourceClientUtility.SelectExpiringEveryone(), noEmail);
            ResourceClientUtility.CheckExpiredClients(ResourceClientUtility.SelectExpiredClients(), ResourceClientUtility.SelectExpiredEveryone(), noEmail);

            using (var bc = new BillingClient())
            {
                BillingProcessResult bpr;

                // Update Data and DataClean tables
                bpr = await bc.BillingProcessDataUpdate(BillingCategory.Tool, true);
                result = result && bpr.Success;

                bpr = await bc.BillingProcessDataUpdate(BillingCategory.Room, true);
                result = result && bpr.Success;

                bpr = await bc.BillingProcessDataUpdate(BillingCategory.Store, true);
                result = result && bpr.Success;

                //2009-08-01 Populate the Billing temp tables
                DateTime ed = DateTime.Now.Date.AddDays(-1); //must be yesterday
                DateTime period = ed.FirstOfMonth();

                bpr = await bc.BillingProcessStep1(BillingCategory.Tool, period, period.AddMonths(1), 0, 0, true, true);
                result = result && bpr.Success;

                bpr = await bc.BillingProcessStep1(BillingCategory.Room, period, period.AddMonths(1), 0, 0, true, true);
                result = result && bpr.Success;

                bpr = await bc.BillingProcessStep1(BillingCategory.Store, period, period.AddMonths(1), 0, 0, true, true);
                result = result && bpr.Success;
            }

            return result;
        }

        [HttpGet, Route("service/task-monthly")]
        public async Task<bool> RunMonthlyTask(bool noEmail = false)
        {
            try
            {
                bool result = true;

                // This is run at midnight on the 1st of the month. So the period should be the 1st of the previous month.
                DateTime period = DateTime.Now.FirstOfMonth().AddMonths(-1);

                using (var bc = new BillingClient())
                {
                    // This sends apportionment emails to clients
                    await bc.SendUserApportionmentReport(new UserApportionmentReportOptions()
                    {
                        Period = period,
                        NoEmail = noEmail
                    });

                    // 2008-04-30
                    // Monthly financial report
                    await bc.SendFinancialManagerReport(new FinancialManagerReportOptions()
                    {
                        Period = period,
                        IncludeManager = !noEmail
                    });

                    // This sends room expiration emails
                    RoomAccessExpirationCheck roomAccessExpirationCheck = new RoomAccessExpirationCheck();
                    int count = await roomAccessExpirationCheck.Run();

                    ////2009-08-01 Populate the BillingTables

                    //first day of last month
                    DateTime sd = period;
                    DateTime ed = period.AddMonths(1);

                    BillingProcessResult bpr;

                    bpr = await bc.BillingProcessStep1(BillingCategory.Tool, sd, ed, 0, 0, false, true);
                    result = result && bpr.Success;

                    bpr = await bc.BillingProcessStep1(BillingCategory.Room, sd, ed, 0, 0, false, true);
                    result = result && bpr.Success;

                    bpr = await bc.BillingProcessStep1(BillingCategory.Store, sd, ed, 0, 0, false, true);
                    result = result && bpr.Success;

                    bpr = await bc.BillingProcessStep4("subsidy", sd, 0);
                    result = result && bpr.Success;
                }

                return result;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return false;
            }
        }

        [HttpGet, Route("service/expiring-cards")]
        public async Task<DataFeedModel<ExpiringCard>> GetExpiringCards()
        {
            RoomAccessExpirationCheck check = new RoomAccessExpirationCheck();
            var dataFeed = await check.GetDataFeed();
            return dataFeed;
        }

        [HttpGet, Route("service/expiring-cards/email")]
        public async Task<int> SendExpiringCardsEmail()
        {
            RoomAccessExpirationCheck roomAccessExpirationCheck = new RoomAccessExpirationCheck();
            int count = await roomAccessExpirationCheck.Run();
            return count;
        }
    }
}
