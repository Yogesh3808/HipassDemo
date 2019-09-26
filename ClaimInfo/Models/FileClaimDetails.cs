using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ClaimInfo.DataModel;

namespace ClaimInfo.Models
{
    public class FileClaimDetails
    {
        public string FileStatus { get; set; }
        public FileIntakeListViewModel FileData { get; set; }

        public List<IntakeClaimData> IntakeClaimDataList { get; set; }
        public int intake_claim_count { get; set; }
    }
    public class FileClaimDetails835
    {
        public string FileStatus { get; set; }
        public FileIntakeListViewModel FileData { get; set; }

        public List<IntakeClaimData835> IntakeClaimDataList { get; set; }
    }
}