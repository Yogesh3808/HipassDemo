using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ClaimInfo.Models
{
    public class clsView
    {
        public int ClaimLineDataSeqID { get; set; }
        public Nullable<int> FileID { get; set; }
        public string BatchID { get; set; }
        public string ClaimID { get; set; }
        public string CreatedBy { get; set; }
        public string CreateDateTime { get; set; }
        public string ServiceDate { get; set; }
        public string Created_Date { get; set; }
        public string ServiceFacilityLocationName { get; set; }
        public string ServiceFacilityLocation_NM102 { get; set; }
        public string ServiceFacilityLocation_NM108 { get; set; }
        public string ServiceFacilityLocation_NM109 { get; set; }
        public string ServiceFacilityLocationAddress { get; set; }
        public string SBR02 { get; set; }
        public string SBR03 { get; set; }
        public string SBR04 { get; set; }
        public string SBR09 { get; set; }
        public string AMT01_PayerPaidAmmount { get; set; }
        public string AMT02_Amount { get; set; }
        public string AMT01_RemainingPatientLiability { get; set; }
        public string AMT02_RPAmount { get; set; }
        public string SVD01 { get; set; }
        public string SVD02 { get; set; }
        public string SVD03 { get; set; }
        public string SVD05 { get; set; }
        public string RemainingPatientLiability { get; set; }
        public string LX { get; set; }
        public string AmmountOwed { get; set; }
        public string LineCheckOrRemittanceDate { get; set; }
        public string STC01_2 { get; set; }
        public string STC03 { get; set; }
        public string ClaimStatus { get; set; }
        public string ClaimStatusErrorCode { get; set; }
        public string ClaimDescription { get; set; }
    }

    [DataContract]
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }

   
}