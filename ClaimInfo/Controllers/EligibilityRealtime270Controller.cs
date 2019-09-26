using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClaimInfo.DataModel;
using ClaimInfo.Models;
using Newtonsoft.Json;

namespace ClaimInfo.Controllers
{
    public class EligibilityRealtime270Controller : Controller
    {
        // GET: EligibilityRealtime270
        HiPaaS_website_offshoreEntities _entityContext   = new HiPaaS_website_offshoreEntities();
      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Claims270()
        {
            ProcessSummaryModel1 objProcessSummaryModel1 = new ProcessSummaryModel1();
            var a = _entityContext.Eligibilty270.OrderByDescending(p => p.ID).ToList();
            for (int i = 0; i < a.Count; i++)
            {
                objProcessSummaryModel1.TypeofTransaction = a[i].TypeofTransaction;
                objProcessSummaryModel1.TotalNumOfReq = a[i].TotalNumOfReq;
                objProcessSummaryModel1.Success = a[i].Success;
                objProcessSummaryModel1.Error = a[i].Error;
                objProcessSummaryModel1.AvgResTime = a[i].AvgResTime;
            }
            List<DataPoint> dataPoints = new List<DataPoint>();
            List<DataPoint> dataPoints1 = new List<DataPoint>();
            var data = _entityContext.Eligibilty270.OrderByDescending(p => p.ID).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                decimal percentage = ((decimal)data[i].Success.Value / data[i].TotalNumOfReq.Value) * 100;
                double Success = Convert.ToDouble(System.Math.Round(percentage, 2));
                decimal percentage1 = ((decimal)data[i].Error.Value / data[i].TotalNumOfReq.Value) * 100;
                double Error = Convert.ToDouble(System.Math.Round(percentage1, 2));
                dataPoints1.Add(new DataPoint("Completed", Success));
                dataPoints1.Add(new DataPoint("Errored", Error));
            }
            dataPoints.Add(new DataPoint("20:22", 206));
            dataPoints.Add(new DataPoint("20:23", 226));
            dataPoints.Add(new DataPoint("20:24", 140));
            dataPoints.Add(new DataPoint("20:25", 166));
            dataPoints.Add(new DataPoint("20:29", 166));
            dataPoints.Add(new DataPoint("20:31", 200));
            dataPoints.Add(new DataPoint("20:34", 169));
            dataPoints.Add(new DataPoint("20:36", 136));
            dataPoints.Add(new DataPoint("20:22", 78));
            dataPoints.Add(new DataPoint("21:22", 200));
            dataPoints.Add(new DataPoint("21:22", 169));
            dataPoints.Add(new DataPoint("21:22", 136));
            dataPoints.Add(new DataPoint("21:22", 78));
            ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);
            ViewBag.DataPoints1 = JsonConvert.SerializeObject(dataPoints1);
            return View(objProcessSummaryModel1);
        }

        public ActionResult ErrorCountDetails(string ErrorCount, string error_claimcount)
        {
            if (ErrorCount == "pie")
            {
                GetErrorCount();
            }
            else
            {
                ViewBag.ErrorCount = ErrorCount;
                ViewBag.error_claimcount = error_claimcount;
            }
            return View();

        }

        private void GetErrorCount()
        {
            var data = _entityContext.Eligibilty270.OrderByDescending(p => p.ID).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                ViewBag.ErrorCount = data[i].Error.ToString();
            }
        }
        public ActionResult Claims270Details()
        {
            string TotalCount = "";
            Eligibility270_Details objEligibility270_Details = new Eligibility270_Details();
            objEligibility270_Details.objVisitDetailsList = _entityContext.SP_GetDetails270(TotalCount, "270").ToList();
            return View(objEligibility270_Details);
        }
    }
}