using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class CAResponse
    {
        public int ResponseID { get; set; }
        public Nullable<int> FileID { get; set; }
        public string ClaimID { get; set; }
        public string ClaimStatus { get; set; }
        public string ClaimStatusErrorCode { get; set; }
        public string ClaimDescription { get; set; }
    }
}