using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class DashboardCountViewModel
    {
        public string FileName { get; set; }
        public int FileCount { get; set; }
        public int SubmittedClaimCount { get; set; }
        public int AcceptedClaimCount { get; set; }
        public int NotAcceptedClaimCount { get; set; }
        public int PendingClaimCount { get; set; }
        public int RejectedClaimCount { get; set; }
        public int EDIFailLoadCount { get; set; }
    }
}