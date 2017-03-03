using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LNF.WebApi.Scheduler.Models
{
    public class ExpiringCard
    {
        public bool LabUser { get; set; }
        public bool Staff { get; set; }
        public bool PhysicalAccess { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string CardNumber { get; set; }
        public string CardStatus { get; set; }
        public DateTime CardExpireDate { get; set; }
        public DateTime BadgeExpireDate { get; set; }
    }
}