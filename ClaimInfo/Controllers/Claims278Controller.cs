using ClaimInfo.DataModel;
using ClaimInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Controllers
{
    public class Claims278Controller : Controller
    {
        HiPaaS_website_offshoreEntities _entityContext;
        public Claims278Controller()
        {
            _entityContext = new HiPaaS_website_offshoreEntities();
        }

        // GET: Claims278
        public ActionResult Index()
        {
            int total= _entityContext.Claims278.ToList().Count();
            int SubCount = _entityContext.Claims278.Where(x => x.ErrorCode == "Validate Error").ToList().Count();
            int FailedFileCount = _entityContext.Claims278.Where(x => x.ErrorCode == "TA1").ToList().Count();
            int AccCount = total - SubCount - FailedFileCount;
            int NotAccCount = 0;
            int PenCount = 0;
            int RejCount = 0;
        

            Dashboard837ViewModel model = new Dashboard837ViewModel();
            model.FileCount = total;
            model.SubmittedClaimCount = SubCount;
            model.AcceptedClaimCount = AccCount;
            model.NotAcceptedClaimCount = NotAccCount;
            model.PendingClaimCount = PenCount;
            model.RejectedClaimCount = RejCount;
            //model.EDIFailLoadCount = NotAccCount - PenCount - RejCount;
            model.EDIFailLoadCount = FailedFileCount;
            model.PendingClaimError = _entityContext.SP_GetPendingClaimError().ToList();
            model.RejectedClaimError = _entityContext.SP_GetRejectedClaimError().ToList();
            model.SubmittedClaimAmount = SubCount * 1000;
            model.AcceptedClaimAmount = SubCount * 800;
            model.NotAcceptedClaimAmount = SubCount * 200;
            return View(model);
        }

        public ActionResult Details()
        {
            return SearchDetails("", "total");
        }

        public ActionResult SearchDetails(string sSelectedDate, string sMenu)
        {
            List<Claims278> model = new List<Claims278>();
            if (!string.IsNullOrEmpty(sSelectedDate))
            {
                //model = _entityContext.Claims278.Where(o => o.TranDate == sSelectedDate).OrderByDescending(o => o.TranDate).ToList();
                if (sMenu == "total")
                {
                    model = _entityContext.Claims278.Where(o => o.TranDate == sSelectedDate).OrderByDescending(o => o.TranDate).ToList();
                }
                else if (sMenu == "FEC")
                {
                    model = _entityContext.Claims278.Where(o => o.ErrorCode == "TA1"&& o.TranDate == sSelectedDate).OrderByDescending(o => o.TranDate).ToList();
                }
                else if (sMenu == "SC")
                {
                    model = _entityContext.Claims278.Where(o => o.ErrorCode == "Validate Error" && o.TranDate == sSelectedDate).OrderByDescending(o => o.TranDate).ToList();
                }
                else if (sMenu == "AC")
                {
                    model = _entityContext.Claims278.Where(o => o.ErrorCode != "Validate Error" && o.ErrorCode == "TA1" && o.TranDate == sSelectedDate).OrderByDescending(o => o.TranDate).ToList();
                }
            }
            else
            {
                if (sMenu == "total" )
                {
                    model = _entityContext.Claims278.OrderByDescending(o => o.TranDate).ToList();
                }
                else if(sMenu== "FEC")
                {
                    model = _entityContext.Claims278.Where(o=>o.ErrorCode== "TA1").OrderByDescending(o => o.TranDate).ToList();
                }
                else if(sMenu == "SC"){
                    model = _entityContext.Claims278.Where(o => o.ErrorCode == "Validate Error").OrderByDescending(o => o.TranDate).ToList();
                }
                else if (sMenu == "AC")
                {
                    model = _entityContext.Claims278.Where(o => o.ErrorCode != "Validate Error" && o.ErrorCode != "TA1").OrderByDescending(o => o.TranDate).ToList();
                }
            }
            
            return View("SearchDetails", model);
        }

        //public ActionResult Details()
        //{
        //    PartialView("_Details")
        //}
    }
}