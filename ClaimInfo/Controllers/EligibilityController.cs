using ClaimInfo.DataModel;
using ClaimInfo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Controllers
{
    public class EligibilityController : Controller
    {
        HiPaaS_website_offshoreEntities _entityContext;
        public EligibilityController()
        {
            _entityContext = new HiPaaS_website_offshoreEntities();
        }
        // GET: Eligibility
        public ActionResult Index()
        {
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
            return View();
        }
        public ActionResult RT276()
        {
            ProcessSummaryModel1 objProcessSummaryModel1 = new ProcessSummaryModel1();
            var a = _entityContext.Eligibility276.OrderByDescending(p => p.ID).ToList();
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
            var data = _entityContext.Eligibility276.OrderByDescending(p => p.ID).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                //float Success1 = (Convert.ToInt32(data[i].Success.Value)) / (Convert.ToInt32(data[i].TotalNumOfReq.Value));
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
        public ActionResult TransCount276(string TotalCount)
        {
            ProcessSummaryModel1 objProcessSummaryModel1 = new ProcessSummaryModel1();
            var a = _entityContext.SP_GetDetails270(TotalCount, "276").ToList();
            if (a.Count > 0)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    objProcessSummaryModel1.TypeofTransaction = a[i].TypeOfTransaction;
                    objProcessSummaryModel1.TotalNumOfReq = a[i].TotalNumOfReq;
                    objProcessSummaryModel1.Success = a[i].Success;
                    objProcessSummaryModel1.Error = a[i].Error;
                    objProcessSummaryModel1.AvgResTime = a[i].AvgResTime;
                    objProcessSummaryModel1.Date = TotalCount;
                }
            }
            else
            {
                objProcessSummaryModel1.TypeofTransaction = "";
                objProcessSummaryModel1.TotalNumOfReq = 0;
                objProcessSummaryModel1.Success = 0; ;
                objProcessSummaryModel1.Error = 0;
                objProcessSummaryModel1.AvgResTime = 0.ToString();
                objProcessSummaryModel1.Date = TotalCount;
            }
            return PartialView("EligibilityPartialView276", objProcessSummaryModel1);
        }
        public ActionResult ErrorCountDetails(string ErrorCount, string error_claimcount)
        {
            if (ErrorCount == "pie")
            {
                GetErrorCount276();
            }
            else
            {
                ViewBag.ErrorCount = ErrorCount;
                ViewBag.error_claimcount = error_claimcount;
            }
            return View();

        }

        private void GetErrorCount276()
        {
            var data = _entityContext.Eligibility276.OrderByDescending(p => p.ID).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                ViewBag.ErrorCount = data[i].Error.ToString();
            }
        }
        public JsonResult GetErrorType276()
        {
            JsonResult result = new JsonResult();

            string draw = Request.Form.GetValues("draw")[0];

            //string orderBy = Request.Params["order[0][dir]"];
            string orderBy = "desc";
            string columnIndex = Request.Params["order[0][column]"];
            string columnName = Request.Params["columns[" + columnIndex + "][data]"];

            //Get parameters
            // get Start (paging start index) and length (page size for paging)
            var searchFileName = Request.Form.GetValues("columns[1][search][value]");
            var searchFileStatus = Request.Form.GetValues("columns[2][search][value]");
            var searchFileDate = Request.Form.GetValues("columns[3][search][value]");
            var searchSubmitter = Request.Form.GetValues("columns[12][search][value]");
            var searchReceiver = Request.Form.GetValues("columns[13][search][value]");

            DateTime? dtFrom = null, dtTo = null;

            if (searchFileDate != null && searchFileDate.Length > 0 && !string.IsNullOrEmpty(searchFileDate[0]))
            {
                var split = searchFileDate[0].Split('-');
                dtFrom = !string.IsNullOrEmpty(split[0]) ? (DateTime?)Convert.ToDateTime(split[0]) : null;
                dtTo = !string.IsNullOrEmpty(split[1]) ? (DateTime?)Convert.ToDateTime(split[1]) : null;
            }


            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

            var data = _entityContext.ErrorTypes.OrderByDescending(p => p.ID).Where(p => p.Transaction_Type == "276").ToList()

            //var data = _entityContext.Process_Summary_Report.OrderByDescending(p => p.Process_Name).ToList()
            .Select(x => new ErrorType1
            {
                Error_type = x.Error_type,
                Transaction_Type = x.Transaction_Type,
                Error_Reason = x.Error_Reason



            });

            int totalRecords = data.Count();
            // Filter record count.
            int recFilter = data.Count();

            // Apply pagination.
            data = data.Skip(startRec).Take(pageSize).ToList();

            // Apply Filter
            //data = FilterFileIntakeData((searchFileName != null && searchFileName.Length > 0) ? searchFileName[0] : null,
            //                                dtFrom, dtTo,
            //                                (searchFileStatus != null && searchFileStatus.Length > 0) ? searchFileStatus[0] : null,
            //                                (searchSubmitter != null && searchSubmitter.Length > 0) ? searchSubmitter[0] : null,
            //                                (searchReceiver != null && searchReceiver.Length > 0) ? searchReceiver[0] : null,
            //                                data);

            //// Apply Sorting
            //data = SortFileInTake(orderBy, columnName, data);

            result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
            return result;
        }
        public JsonResult GetProcessSummary()
        {
            JsonResult result = new JsonResult();
            string draw = Request.Form.GetValues("draw")[0];
            //string orderBy = Request.Params["order[0][dir]"];
            string orderBy = "desc";
            string columnIndex = Request.Params["order[0][column]"];
            string columnName = Request.Params["columns[" + columnIndex + "][data]"];
            //Get parameters
            // get Start (paging start index) and length (page size for paging)
            var searchFileName = Request.Form.GetValues("columns[1][search][value]");
            var searchFileStatus = Request.Form.GetValues("columns[2][search][value]");
            var searchFileDate = Request.Form.GetValues("columns[3][search][value]");
            var searchSubmitter = Request.Form.GetValues("columns[12][search][value]");
            var searchReceiver = Request.Form.GetValues("columns[13][search][value]");
            DateTime? dtFrom = null, dtTo = null;
            if (searchFileDate != null && searchFileDate.Length > 0 && !string.IsNullOrEmpty(searchFileDate[0]))
            {
                var split = searchFileDate[0].Split('-');
                dtFrom = !string.IsNullOrEmpty(split[0]) ? (DateTime?)Convert.ToDateTime(split[0]) : null;
                dtTo = !string.IsNullOrEmpty(split[1]) ? (DateTime?)Convert.ToDateTime(split[1]) : null;
            }
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            var data = _entityContext.Eligibilty270.OrderByDescending(p => p.ID).ToList()
             //var data = _entityContext.Process_Summary_Report.OrderByDescending(p => p.Process_Name).ToList()
             .Select(x => new ProcessSummaryModel1
             {
                 TypeofTransaction = x.TypeofTransaction,
                 AvgResTime = x.AvgResTime,
                 TotalNumOfReq = x.TotalNumOfReq,
                 Success = x.Success,
                 Error = x.Error

             });
            int totalRecords = data.Count();
            // Filter record count.
            int recFilter = data.Count();
            // Apply pagination.
            data = data.Skip(startRec).Take(pageSize).ToList();
            // Apply Filter
            //data = FilterFileIntakeData((searchFileName != null && searchFileName.Length > 0) ? searchFileName[0] : null,
            //                dtFrom, dtTo,
            //                (searchFileStatus != null && searchFileStatus.Length > 0) ? searchFileStatus[0] : null,
            //                (searchSubmitter != null && searchSubmitter.Length > 0) ? searchSubmitter[0] : null,
            //                (searchReceiver != null && searchReceiver.Length > 0) ? searchReceiver[0] : null,
            //                data);
            //// Apply Sorting
            //data = SortFileInTake(orderBy, columnName, data);
            result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
            return result;
        }
        public JsonResult GetProcessSummary276()
        {
            JsonResult result = new JsonResult();
            string draw = Request.Form.GetValues("draw")[0];
            //string orderBy = Request.Params["order[0][dir]"];
            string orderBy = "desc";
            string columnIndex = Request.Params["order[0][column]"];
            string columnName = Request.Params["columns[" + columnIndex + "][data]"];
            //Get parameters
            // get Start (paging start index) and length (page size for paging)
            var searchFileName = Request.Form.GetValues("columns[1][search][value]");
            var searchFileStatus = Request.Form.GetValues("columns[2][search][value]");
            var searchFileDate = Request.Form.GetValues("columns[3][search][value]");
            var searchSubmitter = Request.Form.GetValues("columns[12][search][value]");
            var searchReceiver = Request.Form.GetValues("columns[13][search][value]");
            DateTime? dtFrom = null, dtTo = null;
            if (searchFileDate != null && searchFileDate.Length > 0 && !string.IsNullOrEmpty(searchFileDate[0]))
            {
                var split = searchFileDate[0].Split('-');
                dtFrom = !string.IsNullOrEmpty(split[0]) ? (DateTime?)Convert.ToDateTime(split[0]) : null;
                dtTo = !string.IsNullOrEmpty(split[1]) ? (DateTime?)Convert.ToDateTime(split[1]) : null;
            }
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            var data = _entityContext.Eligibility276.OrderByDescending(p => p.ID).ToList()
             //var data = _entityContext.Process_Summary_Report.OrderByDescending(p => p.Process_Name).ToList()
             .Select(x => new ProcessSummaryModel1
             {
                 TypeofTransaction = x.TypeofTransaction,
                 AvgResTime = x.AvgResTime,
                 TotalNumOfReq = x.TotalNumOfReq,
                 Success = x.Success,
                 Error = x.Error
             });
            int totalRecords = data.Count();
            // Filter record count.
            int recFilter = data.Count();
            // Apply pagination.
            data = data.Skip(startRec).Take(pageSize).ToList();
            // Apply Filter
            //data = FilterFileIntakeData((searchFileName != null && searchFileName.Length > 0) ? searchFileName[0] : null,
            //                dtFrom, dtTo,
            //                (searchFileStatus != null && searchFileStatus.Length > 0) ? searchFileStatus[0] : null,
            //                (searchSubmitter != null && searchSubmitter.Length > 0) ? searchSubmitter[0] : null,
            //                (searchReceiver != null && searchReceiver.Length > 0) ? searchReceiver[0] : null,
            //                data);
            //// Apply Sorting
            //data = SortFileInTake(orderBy, columnName, data);
            result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
            return result;
        }
        public ActionResult Pie()
        {
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

            return View();
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