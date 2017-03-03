using LNF.Models.Data;
using OnlineServices.Api;
using OnlineServices.Api.Data;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LNF.WebApi.Scheduler.Models
{
    public class RoomAccessExpirationCheck
    {
        public async Task Run()
        {
            var dataFeed = await GetDataFeed();
            EmailExpiringCards(dataFeed.Data["default"]);
        }

        private DateTime GetMinDateTime(DateTime d1, DateTime d2)
        {
            if (d1 < d2)
                return d1;
            else
                return d2;
        }

        private void EmailExpiringCards(IEnumerable<ExpiringCard> data)
        {
            foreach (var item in data)
            {
                if (!item.Email.StartsWith("none"))
                {
                    try
                    {
                        var addr = new MailAddress(item.Email);
                        string from = "lnf-access@umich.edu";
                        string subj = "LNF Access Card Expiring Soon";
                        string body = string.Format("{1}{0}{0}Your LNF access card is set to expire on {2}. Please update your safety training per information on www.LNF.umich.edu/getting-access/", Environment.NewLine, item.DisplayName, GetMinDateTime(item.CardExpireDate, item.BadgeExpireDate));

                        Providers.Email.SendMessage(0, "LNF.Scheduler.Service.Process.CheckClientIssues.EmailExpiringCards", subj, body, from, new[] { item.Email });
                    }
                    catch
                    {
                        //invalid address so skip it
                    }
                }
            }
        }

        public async Task<DataFeedModel<ExpiringCard>> GetDataFeed()
        {
            using (FeedClient fc = await ApiProvider.NewFeedClient())
            {
                DataFeedModel<ExpiringCard> result = await fc.GetDataFeedResult<ExpiringCard>("expiring-cards");
                return result;
            }
        }
    }
}