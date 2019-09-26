using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Models
{
    public class LCD_Covered_OriginalViewModel
    {
        public List<SelectListItem> Codes { get; set; }
        public int[] CodeID { get; set; }
        public List<SP_GetLCD_Covered_Original_Result> listitem;

    }
}