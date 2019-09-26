using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Models
{
    public class ClaimDetailsViewModel
    {
        [Display(Name = "Seq ID")]
        public int ClaimDataSeqID { get; set; }

        [Display(Name = "File ID")]
        public string FileID { get; set; }//public Nullable<int> FileID { get; set; }

        //[Display(Name = "Claim Ext Nmbr")]
        [Display(Name = "Accident Date")]
        public string ClaimExtNmbr { get; set; }

        [Display(Name = "ClaimID")]
        public string ClaimID { get; set; }

        [Display(Name = "Claim TM Tracking ID")]
        public string ClaimTMTrackingID { get; set; }

        [Display(Name = "Discharge Hour")]
        public string DischargeHour { get; set; }

        [Display(Name = "Admission Date")]
        public string AdmissionDate { get; set; }

        [Display(Name = "Claim Amount")]
        public string Claim_Amount { get; set; }

        [Display(Name = "Patient Paid")]
        public string PatientPaid { get; set; }

        [Display(Name = "Net Balance")]
        public string NetBalance { get; set; }

        [Display(Name = "Adjust")]
        public string Adjust { get; set; }

        [Display(Name = "Insurance Balance")]
        public string InsuranceBalance { get; set; }

        [Display(Name = "BillingProvider")]
        public string BillingProvider { get; set; }

        [Display(Name = "Billing Provider Address")]
        public string BillingProviderAddress { get; set; }

        [Display(Name = "Payer")]
        public string Payer { get; set; }

        [Display(Name = "Payer Address")]
        public string PayerAddress { get; set; }

        [Display(Name = "Claim Status")]
        public string ClaimStatus { get; set; }
        public IEnumerable<SelectListItem> ClaimStatusList { get; set; }

        [Display(Name = "Claim Code")]
        public string ClaimCode { get; set; }

        [Display(Name = "Patient First Name")]
        public string PatientFirstName { get; set; }

        [Display(Name = "Patient Last Name")]
        public string PatientLastname { get; set; }

        [Display(Name = "ICD Code")]
        public string ICDCode { get; set; }
        public IEnumerable<SelectListItem> ICDCodeList { get; set; }

        public string ClaimLevelICDErrorFlag { get; set; }
        public string ClaimLevelCLMErrorFlag { get; set; }

        public IEnumerable<IntakeClaimLineDataListViewModel> IntakeClaimLineData { get; set; }
    }
}