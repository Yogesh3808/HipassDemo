using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class ProcessSummaryModel
    {
        public int Seq_ID { get; set; }
        public string Process_Name { get; set; }
        public Nullable<double> Avg_Response { get; set; }
        public Nullable<double> Standard_Deviation { get; set; }
        public Nullable<int> executions { get; set; }
        public string Avg_inbound_size { get; set; }
        public Nullable<double> Avg_inbound_count { get; set; }
        public string Avg_outbound_size { get; set; }
        public Nullable<double> Avg_outbound_count { get; set; }
        public string Avg_returned_size { get; set; }
        public Nullable<double> Avg_returned_count { get; set; }
    }

    public class Eligibility270_Details
    {
        public string Total_File { get; set; }
        public string Status { get; set; }

        public List<SP_GetDetails270_Result> objVisitDetailsList { get; set; }


    }

    public class ProcessSummaryModel1
    {
        public int ID { get; set; }
        public string TypeofTransaction { get; set; }
        public string AvgResTime { get; set; }

        public Nullable<int> TotalNumOfReq { get; set; }
        public Nullable<int> Success { get; set; }
        public Nullable<int> Error { get; set; }
        public string Date { get; set; }
        public List<SP_GetDetails270_Result> objVisitDetailsList { get; set; }
    }

    public class ErrorType1
    {
        public int ID { get; set; }
        public string Error_type { get; set; }
        public string Transaction_Type { get; set; }
        public string Error_Reason { get; set; }
    }
}