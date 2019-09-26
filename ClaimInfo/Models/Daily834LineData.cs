using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ClaimInfo.Models
{
    public class Daily834LineData
    {
 
     
        public string FileName { get; set; }

        [Display(Name = "File ID")]
        public string FileID { get; set; }
      

        [Display(Name = "Sender")]
        public string sender { get; set; }

        [Display(Name = "Receiver")]
        public string Receiver { get; set; }

        [Display(Name = "Subscriber No")]
        public string SubscriberNo { get; set; }

        [Display(Name = "First Name ")]
        public string MemberFName { get; set; }

        [Display(Name = "Last Name")]
        public string MemberLName { get; set; }

        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "PostalCode")]
        public string PostalCode { get; set; }

        [Display(Name = "Enrollment Type")]
        public string Enrollment_type { get; set; }

        [Display(Name = "Dob")]
        public string dob { get; set; }

        [Display(Name = "Gender")]
        public string gender { get; set; }

        [Display(Name = "InsLine Code")]
        public string InsLineCode { get; set; }

       

        [Display(Name = "Member Amount")]
        public string MemberAmount { get; set; }

        [Display(Name = "Enrollment Status")]
        public string EnrollmentStatus { get; set; }

        public List<SP_834FileHeaderDetails_Result> Obj834LineData { get; set; }



    }
}