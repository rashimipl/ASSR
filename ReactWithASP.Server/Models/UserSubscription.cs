using System;
using System.Collections.Generic;

namespace ReactWithASP.Server.Models
{
    public partial class UserSubscription
    {
        public int Id { get; set; }
        public string UserGuid { get; set; } = null!;
        public int SubsPlanId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = null!;

        public virtual SubscriptionPlan SubsPlan { get; set; } = null!;
        public virtual AspNetUser UserGu { get; set; } = null!;
    }
}
