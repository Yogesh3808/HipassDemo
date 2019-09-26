using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class Dailt834Model
    {
        public string TotalFile { get; set; }
        public string TotalEnrollment { get; set; }
        public string Additional { get; set; }
        public string Change { get; set; }
        public string Term { get; set; }
        public string ErrorCount { get; set; }
        public List<SP_Daily834headerData_Result> objVisitDetailsList { get; set; }
        public List<SP_834Filecountwisedetails_Result> objFileEnrollmentDetails { get; set; }
        public List<SP_834FileDetails_Result> objFile834Details { get; set; }
    }
}