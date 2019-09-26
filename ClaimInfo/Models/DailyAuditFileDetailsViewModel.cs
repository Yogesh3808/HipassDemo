using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class DailyAuditFileDetailsViewModel
    {
        public int TotalFiles { get; set; }
        public string FileStatus { get; set; }
        public FileIntakeListViewModel FileData { get; set; }

        public List<IntakeClaimData> IntakeClaimDataList { get; set; }
    }
}