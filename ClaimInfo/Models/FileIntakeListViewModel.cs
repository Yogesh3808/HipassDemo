using System;
using System.ComponentModel.DataAnnotations;

namespace ClaimInfo.Models
{
    public class FileIntakeListViewModel
    {
        [Display(Name = "File ID")]
        public string FileID { get; set; } //public int? FileID { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "File Date")]
        public string FileDate { get; set; }

        [Display(Name = "File Status")]
        public string FileStatus { get; set; }

        public string ISA09 { get; set; }
        public string ISA10 { get; set; }
        public string GSA04 { get; set; }
        public string GSA05 { get; set; }
        public string ST01 { get; set; }
        public string ST02 { get; set; }
        public string ST03 { get; set; }
        public string BHT03 { get; set; }

        [Display(Name = "Submitter")]
        public string Submitter_N103 { get; set; }

        [Display(Name = "Receiver")]
        public string Receiver_N103 { get; set; }

        [Display(Name = "Created Date")]
        public string Created_Date { get; set; }

        [Display(Name = "Create Date Time")]
        public string CreateDateTime { get; set; }

        //public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public Nullable<int> ClaimCount { get; set; }
        public Nullable<int> error_claimcount { get; set; }

    }
}