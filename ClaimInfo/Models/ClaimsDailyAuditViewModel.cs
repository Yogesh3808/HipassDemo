using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class ClaimsDailyAuditViewModel
    {
        public Nullable<int> SubTotal { get; set; }
        public Nullable<int> InBizstockTotal { get; set; }
        public Nullable<int> RejTotal { get; set; }
        public Nullable<int> PenTotal { get; set; }
        public Nullable<int> VeriTotal { get; set; }
        public Nullable<int> errTotal { get; set; }
        public List<SP_GetClaimDailyAudit_Result> listitem;
    }
}