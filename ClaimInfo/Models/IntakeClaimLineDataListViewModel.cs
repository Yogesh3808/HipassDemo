using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class IntakeClaimLineDataListViewModel
    {
        public int ClaimLineDataSeqID { get; set; }
        //public Nullable<int> FileID { get; set; }
        public string FileID { get; set; }
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
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }

        public string adjustment { get; set; }
        public string type_of_adjustment { get; set; }
        public string RemainigAmt { get; set; }


    }
}