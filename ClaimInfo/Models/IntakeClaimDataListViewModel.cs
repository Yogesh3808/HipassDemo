using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class IntakeClaimDataListViewModel
    {
        public int ClaimDataSeqID { get; set; }
        public string FileID { get; set; }
        public string ClaimExtNmbr { get; set; }
        public string ClaimID { get; set; }
        public string ClaimTMTrackingID { get; set; }
        public string DischargeHour { get; set; }
        public string AdmissionDate { get; set; }
        public string Claim_Amount { get; set; }
        public string PatientPaid { get; set; }
        public string NetBalance { get; set; }
        public string Adjust { get; set; }
        public string InsuranceBalance { get; set; }
        public string BillingProvider { get; set; }
        public string BillingProviderAddress { get; set; }
        public string Payer { get; set; }
        public string PayerAddress { get; set; }
        public string ClaimStatus { get; set; }
        public string ClaimCode { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string SubscriberLastName { get; set; }
        public string SubscriberFirstName { get; set; }
        public string AccountNumber { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
    }
}