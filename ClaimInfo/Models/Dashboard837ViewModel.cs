using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class Dashboard837ViewModel
    {
        public string FileDate { get; set; }
        public int FileCount { get; set; }
        public int SubmittedClaimCount { get; set; }
        public int AcceptedClaimCount { get; set; }
        public int NotAcceptedClaimCount { get; set; }
        public int PendingClaimCount { get; set; }
        public int RejectedClaimCount { get; set; }
        public int EDIFailLoadCount { get; set; }

        //
        public List<SP_GetPendingClaimError_Result> PendingClaimError;
        public List<SP_GetRejectedClaimError_Result> RejectedClaimError;
        public int SubmittedClaimAmount { get; set; }
        public int AcceptedClaimAmount { get; set; }
        public int NotAcceptedClaimAmount { get; set; }

        public int PaidClaimsCount { get; set; }
        public int DeniedClaimsCount { get; set; }
        public int WorkinPClaimsCount { get; set; }

        public List<FileClaimDetails> fileclaim_list;
    }
    public class Dashboard835ViewModel
    {
        public string FileDate { get; set; }
        public int FileCount { get; set; }
        public int SubmittedClaimCount { get; set; }
        public int AcceptedClaimCount { get; set; }
        public int NotAcceptedClaimCount { get; set; }
        public int PendingClaimCount { get; set; }
        public int RejectedClaimCount { get; set; }
        public int EDIFailLoadCount { get; set; }

        //
        public List<SP_GetPendingClaimError_Result> PendingClaimError;
        public List<SP_GetRejectedClaimError_Result> RejectedClaimError;
        public int SubmittedClaimAmount { get; set; }
        public int AcceptedClaimAmount { get; set; }
        public int NotAcceptedClaimAmount { get; set; }

        public int PaidClaimsCount { get; set; }
        public int DeniedClaimsCount { get; set; }
        public int WorkinPClaimsCount { get; set; }

        public List<FileClaimDetails835> fileclaim_list;
    }

}