using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClaimInfo.Models
{
    public class IntakeClaimLineDetailsViewModel
    {
        [Display(Name = "Seq ID")]
        public int ClaimLineDataSeqID { get; set; }

        [Display(Name = "File ID")]
        public string FileID { get; set; }//public Nullable<int> FileID { get; set; }

        [Display(Name = "Batch ID")]
        public string BatchID { get; set; }

        [Display(Name = "Claim ID")]
        public string ClaimID { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Create Date Time")]
        public string CreateDateTime { get; set; }

        [Display(Name = "Created Date")]
        public string Created_Date { get; set; }

        [Display(Name = "Service Facility Loc")]
        public string ServiceFacilityLocationName { get; set; }

        [Display(Name = "Service Facility Loc NM102")]
        public string ServiceFacilityLocation_NM102 { get; set; }

        [Display(Name = "Service Facility Loc NM108")]
        public string ServiceFacilityLocation_NM108 { get; set; }

        [Display(Name = "Service Facility Loc NM109")]
        public string ServiceFacilityLocation_NM109 { get; set; }

        [Display(Name = "Service Facility Loc Adrs")]
        public string ServiceFacilityLocationAddress { get; set; }

        [Display(Name = "SBR02")]
        public string SBR02 { get; set; }

        [Display(Name = "SBR03")]
        public string SBR03 { get; set; }

        [Display(Name = "SBR04")]
        public string SBR04 { get; set; }

        [Display(Name = "SBR09")]
        public string SBR09 { get; set; }

        [Display(Name = "Seq ID")]
        public string AMT01_PayerPaidAmmount { get; set; }

        [Display(Name = "AMT02 Amount")]
        public string AMT02_Amount { get; set; }

        [Display(Name = "AMT01 Rem. Patient Liability")]
        public string AMT01_RemainingPatientLiability { get; set; }

        [Display(Name = "AMT02 RPAmount")]
        public string AMT02_RPAmount { get; set; }
        [Display(Name = "Other Payer Primary Identifier")]
        public string SVD01 { get; set; }
        [Display(Name = "Patient Paid")]
        public string SVD02 { get; set; }
        [Display(Name = "SVD03")]
        public string SVD03 { get; set; }


        [Display(Name = "Procedure Code")]
        public string SVD05 { get; set; }


        [Display(Name = "Line Check Or Rmtnc Dt.")]
        public string LineCheckOrRemittanceDate { get; set; }

        [Display(Name = "Rem Patient Liability")]
        public string RemainingPatientLiability { get; set; }

        [Display(Name = "Service Date")]
        public string ServiceDate { get; set; }

        [Display(Name = "LX")]
        public string LX { get; set; }



        [Display(Name = "Patient Liability")]
        public string AmmountOwed { get; set; }


        [Display(Name = "Paid Service Unit Count")]
        public string Qty { get; set; }



        public FileIntakeListViewModel FileData { get; set; }

        public IntakeClaimDataListViewModel IntakeClaimData { get; set; }


    }
}