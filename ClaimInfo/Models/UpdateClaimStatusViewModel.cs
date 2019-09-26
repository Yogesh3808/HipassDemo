using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class UpdateClaimStatusViewModel
    {
        public int ClaimDataSeqID { get; set; }
        public string ClaimStatus { get; set; }
        public string ClaimExtNmbr { get; set; }
        public string ICDCode { get; set; }
    }
}