using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Models
{
    public class FileIntakeClaimViewModel
    {
        public FileIntakeListViewModel FileData { get; set; }

        [Display(Name = "Claim Code")]
        public string ClaimNoID { get; set; }
        public IEnumerable<SelectListItem> ClaimNoList { get; set; }

    }
}