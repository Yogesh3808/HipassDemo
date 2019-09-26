using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClaimInfo.DataModel;

namespace ClaimInfo.Models
{
    public class Claims278ViewModel
    {
        public int TranID { get; set; }

        public string TranName { get; set; }

        public string TranDate { get; set; }

        public string TranStatus { get; set; }

        public string Submitter { get; set; }

        public string ErrorCode { get; set; }
    }
    public class ClaimsList278ViewModel
    {
        public Claims278ViewModel claims278 { get; set; }
    }
}