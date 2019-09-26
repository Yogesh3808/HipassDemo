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
    public class Claims837InboundController : Controller
    {

        HiPaaS_website_offshoreEntities _entityContext;
        public Claims837InboundController()
        {
            _entityContext = new HiPaaS_website_offshoreEntities();
        }
        #region Action Methods
        // GET: Claims
        /// <summary>
        /// Claim 837 Dashboard
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            int SubCount = _entityContext.IntakeClaimDatas.ToList().Count();
            int AccCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted").ToList().Count();
            int NotAccCount = SubCount - AccCount;
            int PenCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Pending").ToList().Count();
            int RejCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Rejected").ToList().Count();
            int FailedFileCount = _entityContext.FileInTakes.Where(x => x.ExtraField2 == "File Error").ToList().Count();
            int PaidCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Paid").ToList().Count();
            int DeniedCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Denied").ToList().Count();
            int WIPCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Work in Progress").ToList().Count();
            //int PaidCount = 0;
            //int DeniedCount = 0;
            //int WIPCount = 0;

            Dashboard837ViewModel model = new Dashboard837ViewModel();
            model.FileCount = _entityContext.FileInTakes.ToList().Count();
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
            //
            model.PaidClaimsCount = PaidCount;
            model.DeniedClaimsCount = DeniedCount;
            model.WorkinPClaimsCount = WIPCount;
            model.fileclaim_list = GetFileDataForDashbaord();
            return View(model);
        }
        private List<FileClaimDetails> GetFileDataForDashbaord()
        {
            string sSelectedDate = "";
            string sMenu = "total";
            List<FileClaimDetails> model = new List<FileClaimDetails>();
            if (!string.IsNullOrEmpty(sSelectedDate))
            {
                List<string> data = new List<string>();
                string strClaimStatus = "";
                string adjudication_status = "";
                if (sMenu == "FEC")
                {
                    strClaimStatus = "File Error";
                    data = _entityContext.FileInTakes.Where(o => o.ExtraField2 == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "SC" || sMenu == "total")
                {
                    strClaimStatus = "";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.FileDate.Contains(sSelectedDate)).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "AC")
                {
                    strClaimStatus = "Accepted";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "EC")
                {
                    strClaimStatus = "Rejected";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "Paid")
                {
                    adjudication_status = "Paid";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "Denied")
                {
                    adjudication_status = "Partial Paid";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "WIP")
                {
                    adjudication_status = "Work in Progress";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                if (strClaimStatus != "")
                {
                    foreach (var FileID in data)
                    {
                        if (strClaimStatus == "Accepted" && adjudication_status != "")
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus && o.adjudication_status == adjudication_status).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                    }

                }
                else
                {
                    foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate))
                    {
                        model.Add(new FileClaimDetails
                        {
                            FileStatus = fileInTake.ExtraField2,
                            FileData = BindFileIntakeData(fileInTake),
                            IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                            intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                        });
                    }
                }

            }
            else
            {
                List<string> data = new List<string>();
                string strClaimStatus = "";
                string adjudication_status = "";
                if (sMenu == "FEC")
                {
                    strClaimStatus = "File Error";
                    data = _entityContext.FileInTakes.Where(o => o.ExtraField2 == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "SC" || sMenu == "total")
                {
                    strClaimStatus = "";
                    data = _entityContext.IntakeClaimDatas.OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "AC")
                {
                    strClaimStatus = "Accepted";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "EC")
                {
                    strClaimStatus = "Rejected";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "Paid")
                {
                    adjudication_status = "Paid";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "Denied")
                {
                    adjudication_status = "Partial Paid";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "WIP")
                {
                    adjudication_status = "Work in Progress";
                    data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }

                foreach (var FileID in data)
                {
                    if (strClaimStatus != "")
                    {
                        if (strClaimStatus == "Accepted" && adjudication_status != "")
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus && o.adjudication_status == adjudication_status).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                        {
                            model.Add(new FileClaimDetails
                            {
                                FileStatus = fileInTake.ExtraField2,
                                FileData = BindFileIntakeData(fileInTake),
                                IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                                intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                            });
                        }
                    }

                }

            }

            return model.OrderByDescending(x => x.FileData.FileDate).ToList();
        }
        public ActionResult ClaimCounts(string sDate)
        {
            //sDate = "20190123";
            int totalFile = Convert.ToInt32(_entityContext.SP_GetFileInTakeCount(sDate).FirstOrDefault());
            int SubCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataCount(sDate).FirstOrDefault());
            int AccCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataVarifiedCount(sDate).FirstOrDefault());
            int NotAccCount = SubCount - AccCount;
            int PenCount = 0;
            int RejCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataErrorsCount(sDate).FirstOrDefault());
            int FailedFileCount = _entityContext.FileInTakes.Where(x => x.ExtraField2 == "File Error" && x.FileDate.Contains(sDate)).ToList().Count();
            int PaidCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Paid" && x.FileDate.Contains(sDate)).ToList().Count();
            int DeniedCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Partial Paid" && x.FileDate.Contains(sDate)).ToList().Count();
            //int WIPCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Verified" && x.adjudication_status == "Work in Progress" && x.FileDate.Contains(sDate)).ToList().Count();
            int WIPCount = SubCount - RejCount - PaidCount - DeniedCount;


            Dashboard837ViewModel model = new Dashboard837ViewModel();
            model.FileDate = sDate;
            model.FileCount = totalFile;
            model.SubmittedClaimCount = SubCount;
            model.AcceptedClaimCount = AccCount;
            model.NotAcceptedClaimCount = NotAccCount;
            model.PendingClaimCount = PenCount;
            model.RejectedClaimCount = RejCount;
            //model.EDIFailLoadCount = NotAccCount - PenCount - RejCount;
            model.EDIFailLoadCount = FailedFileCount;
            //
            model.PaidClaimsCount = PaidCount;
            model.DeniedClaimsCount = DeniedCount;
            model.WorkinPClaimsCount = WIPCount;

            return PartialView("_DashboardPartialView", model);
        }
        public ActionResult DashboardChild(string sDate)
        {
            //sDate = "20190123";
            int totalFile = Convert.ToInt32(_entityContext.SP_GetFileInTakeCount(sDate).FirstOrDefault());
            int SubCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataCount(sDate).FirstOrDefault());
            int AccCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataVarifiedCount(sDate).FirstOrDefault());
            int NotAccCount = SubCount - AccCount;
            int PenCount = 0;
            int RejCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataErrorsCount(sDate).FirstOrDefault());

            Dashboard837ViewModel model = new Dashboard837ViewModel();
            model.FileDate = sDate;
            model.FileCount = totalFile;
            model.SubmittedClaimCount = SubCount;
            model.AcceptedClaimCount = AccCount;
            model.NotAcceptedClaimCount = NotAccCount;
            model.PendingClaimCount = PenCount;
            model.RejectedClaimCount = RejCount;
            model.EDIFailLoadCount = NotAccCount - PenCount - RejCount;

            return PartialView("_DashboardChild", model);
        }

        public ActionResult ClaimsDetails()
        {
            return FileClaimDetails("", "total");
        }

        public ActionResult FileClaimDetails(string sSelectedDate, string sMenu)
        {
            using (HiPaaS_website_offshoreEntities entites = new HiPaaS_website_offshoreEntities())
            {
                //List<FileClaimDetails> model = new List<FileClaimDetails>();
                //foreach (FileInTake fileInTake in _entityContext.FileInTakes.OrderByDescending(p => p.FileDate))
                //{
                //    model.Add(new FileClaimDetails
                //    {
                //        FileStatus = fileInTake.ExtraField2,
                //        FileData = BindFileIntakeData(fileInTake),
                //        IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                //    });
                //}
                ViewBag.FileCount = Convert.ToInt32(_entityContext.SP_GetFileInTakeCount(sSelectedDate).FirstOrDefault());
                List<FileClaimDetails> model = new List<FileClaimDetails>();
                if (!string.IsNullOrEmpty(sSelectedDate))
                {
                    List<string> data = new List<string>();
                    string strClaimStatus = "";
                    string adjudication_status = "";
                    if (sMenu == "FEC")
                    {
                        strClaimStatus = "File Error";
                        data = _entityContext.FileInTakes.Where(o => o.ExtraField2 == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "SC" || sMenu == "total")
                    {
                        strClaimStatus = "";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.FileDate.Contains(sSelectedDate)).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "AC")
                    {
                        strClaimStatus = "Accepted";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "EC")
                    {
                        strClaimStatus = "Rejected";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "Paid")
                    {
                        adjudication_status = "Paid";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                        strClaimStatus = "Accepted";
                    }
                    else if (sMenu == "Denied")
                    {
                        adjudication_status = "Partial Paid";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                        strClaimStatus = "Accepted";
                    }
                    else if (sMenu == "WIP")
                    {
                        adjudication_status = "Work in Progress";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                        strClaimStatus = "Accepted";
                    }
                    if (strClaimStatus != "")
                    {
                        foreach (var FileID in data)
                        {
                            if (strClaimStatus == "Accepted" && adjudication_status != "")
                            {
                                foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                                {
                                    model.Add(new FileClaimDetails
                                    {
                                        FileStatus = fileInTake.ExtraField2,
                                        FileData = BindFileIntakeData(fileInTake),
                                        IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus && o.adjudication_status == adjudication_status).Take(10).ToList(),
                                        intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                    });
                                }
                            }
                            else
                            {
                                foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                                {
                                    model.Add(new FileClaimDetails
                                    {
                                        FileStatus = fileInTake.ExtraField2,
                                        FileData = BindFileIntakeData(fileInTake),
                                        IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                        intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                    });
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate))
                        {
                            model.Add(new FileClaimDetails
                            {
                                FileStatus = fileInTake.ExtraField2,
                                FileData = BindFileIntakeData(fileInTake),
                                IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                                intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                            });
                        }
                    }

                }
                else
                {
                    List<string> data = new List<string>();
                    string strClaimStatus = "";
                    string adjudication_status = "";
                    if (sMenu == "FEC")
                    {
                        strClaimStatus = "File Error";
                        data = _entityContext.FileInTakes.Where(o => o.ExtraField2 == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "SC" || sMenu == "total")
                    {
                        strClaimStatus = "";
                        data = _entityContext.IntakeClaimDatas.OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "AC")
                    {
                        strClaimStatus = "Accepted";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "EC")
                    {
                        strClaimStatus = "Rejected";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "Paid")
                    {
                        adjudication_status = "Paid";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                        strClaimStatus = "Accepted";
                    }
                    else if (sMenu == "Denied")
                    {
                        adjudication_status = "Partial Paid";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                        strClaimStatus = "Accepted";
                    }
                    else if (sMenu == "WIP")
                    {
                        adjudication_status = "Work in Progress";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                        strClaimStatus = "Accepted";
                    }

                    foreach (var FileID in data)
                    {
                        if (strClaimStatus != "")
                        {
                            if (strClaimStatus == "Accepted" && adjudication_status != "")
                            {
                                foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                                {
                                    model.Add(new FileClaimDetails
                                    {
                                        FileStatus = fileInTake.ExtraField2,
                                        FileData = BindFileIntakeData(fileInTake),
                                        IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus && o.adjudication_status == adjudication_status).Take(10).ToList(),
                                        intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                    });
                                }
                            }
                            else
                            {
                                foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                                {
                                    model.Add(new FileClaimDetails
                                    {
                                        FileStatus = fileInTake.ExtraField2,
                                        FileData = BindFileIntakeData(fileInTake),
                                        IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                        intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                    });
                                }
                            }
                        }
                        else
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }

                    }

                }

                return View("FileClaimDetails", model.OrderByDescending(x => x.FileData.FileDate));
            }
        }

        [HttpPost]
        public JsonResult UpdateClaimStatus(UpdateClaimStatusViewModel obj)
        {
            Int32 ClaimStatusValue = Convert.ToInt32(obj.ClaimStatus);
            if (ClaimStatusValue != 0)
            {
                var Claim_Status = _entityContext.Claim_Status.Where(x => x.Claim_Status_id == ClaimStatusValue).FirstOrDefault();
                obj.ClaimStatus = Claim_Status.Claim_Status_desc;
            }
            else
            {
                obj.ClaimStatus = "";
            }

            if (_entityContext != null)
            {
                var data = _entityContext.IntakeClaimDatas.Where(x => x.SeqID == obj.ClaimDataSeqID).FirstOrDefault();
                if (data != null)
                {
                    data.ClaimStatus = string.IsNullOrEmpty(obj.ClaimStatus) ? data.ClaimStatus : obj.ClaimStatus;
                    _entityContext.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFileDetailsByDate(string sSelectedDate, string sMenu)
        {
            using (HiPaaS_website_offshoreEntities entites = new HiPaaS_website_offshoreEntities())
            {
                ViewBag.FileCount = Convert.ToInt32(_entityContext.SP_GetFileInTakeCount(sSelectedDate).FirstOrDefault());
                List<FileClaimDetails> model = new List<FileClaimDetails>();
                if (!string.IsNullOrEmpty(sSelectedDate))
                {
                    foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate))
                    {
                        model.Add(new FileClaimDetails
                        {
                            FileStatus = fileInTake.ExtraField2,
                            FileData = BindFileIntakeData(fileInTake),
                            IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                            intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                        });
                    }
                }
                else
                {
                    List<string> data = new List<string>();
                    string strClaimStatus = "";
                    if (sMenu == "FEC")
                    {
                        strClaimStatus = "File Error";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "SC" || sMenu == "total")
                    {
                        strClaimStatus = "";
                        data = _entityContext.IntakeClaimDatas.OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "AC")
                    {
                        strClaimStatus = "Accepted";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "EC")
                    {
                        strClaimStatus = "Rejected";
                        data = _entityContext.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }

                    foreach (var FileID in data)
                    {
                        if (strClaimStatus != "")
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake fileInTake in _entityContext.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                                    intake_claim_count = _entityContext.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }

                    }

                }


                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Details(int SeqID)
        {
            IntakeClaimDetailsNewViewModel intakeClaimData = GetIntakeClaimDetailsNew(SeqID);
            var ClaimStatusList = GetClaimStatusList();
            var ICDCodeList = GetICDCodeList();
            var Claim_Status = _entityContext.Claim_Status.Where(x => x.Claim_Status_desc == intakeClaimData.ClaimStatus).FirstOrDefault();
            if (Claim_Status == null)
            {
                ViewBag.ClaimStatus = new SelectList(ClaimStatusList, "Value", "Text", 0);
            }
            else
            {
                ViewBag.ClaimStatus = new SelectList(ClaimStatusList, "Value", "Text", Claim_Status.Claim_Status_id);
            }
            var ICDCode = _entityContext.Claims_ICD_CODE.Where(x => x.ICD_CODE == intakeClaimData.ICDCode).FirstOrDefault();
            if (ICDCode == null)
            {
                ViewBag.ICDCode = new SelectList(ICDCodeList, "Value", "Text", 0);
            }
            else
            {
                ViewBag.ICDCode = new SelectList(ICDCodeList, "Value", "Text", ICDCode.SeqId);
            }

            return PartialView("Details", intakeClaimData);
        }

        public ActionResult ClaimsDailyAudit(string sSelectedDate)
        {
            ClaimsDailyAuditViewModel model = new ClaimsDailyAuditViewModel();
            ViewBag.FileCount = Convert.ToInt32(_entityContext.SP_GetFileInTakeCount(sSelectedDate).FirstOrDefault());
            int FailedFileCount = _entityContext.FileInTakes.Where(x => x.ExtraField2 == "File Error").ToList().Count();
            int Count999 = Convert.ToInt32(_entityContext.SP_GetFileInTakeCount(sSelectedDate).FirstOrDefault()) - FailedFileCount;
            ViewBag.Count999 = Count999;
            var data = _entityContext.SP_GetClaimDailyAuditCount(sSelectedDate).FirstOrDefault();
            if (data != null)
            {
                model.SubTotal = data.SubTotal;
                model.VeriTotal = data.VeriTotal;
                model.InBizstockTotal = data.InBizstockTotal;
                model.PenTotal = data.PenTotal;
                model.RejTotal = data.RejTotal;
                model.errTotal = data.errTotal;
                model.listitem = _entityContext.SP_GetClaimDailyAudit(sSelectedDate).ToList();
            }
            return View(model);
        }
        public ActionResult ClaimsDailyRejection(string sSelectedDate)
        {
            int RejCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataErrorsCount(sSelectedDate).FirstOrDefault());
            //int RejCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Errors").ToList().Count();
            ViewBag.RejCount = RejCount;
            List<SP_GetRejectedClaims_Result> model = new List<SP_GetRejectedClaims_Result>();
            model = _entityContext.SP_GetRejectedClaims(sSelectedDate).ToList();
            return View(model);
        }
        public ActionResult LCDCovered()
        {
            LCD_Covered_OriginalViewModel pd = new LCD_Covered_OriginalViewModel();
            //pd.Codes = PopulateCodes();
            pd.listitem = _entityContext.SP_GetLCD_Covered_Original().ToList();
            return View(pd);
        }
        public ActionResult ClaimSearch()
        {
            List<SP_SearchClaims_Result> model = new List<SP_SearchClaims_Result>();
            model = _entityContext.SP_SearchClaims().ToList();
            return View(model);
        }
        public ActionResult ResearchQueue()
        {
            return View();
        }
        public ActionResult MatchClaims()
        {
            return View();
        }
        public ActionResult AssignedDetails()
        {
            return View();
        }
        public ActionResult Dashboard835()
        {
            return View();
        }
        private static List<SelectListItem> PopulateCodes()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["Constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = " SELECT ID, CategoryDesc FROM Category";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["CategoryDesc"].ToString(),
                                Value = sdr["ID"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }

            return items;
        }

        public ActionResult LCDSmall()
        {
            List<SP_GetLCD_SmallFileNew_Result> model = new List<SP_GetLCD_SmallFileNew_Result>();
            model = _entityContext.SP_GetLCD_SmallFileNew().ToList();
            return View(model);
        }

        public ActionResult ClaimsInfo()
        {
            return View();
        }

        public ActionResult submitClaim()
        {
            return View();
        }
        //Yogesh code
        public ActionResult Eligibility271Details(string TotalCount, string Status)
        {
            Eligibility270_Details objEligibility270_Details = new Eligibility270_Details();
            objEligibility270_Details.Total_File = TotalCount;
            objEligibility270_Details.Status = Status;
            objEligibility270_Details.objVisitDetailsList = _entityContext.SP_GetDetails270(TotalCount, "276").ToList();
            return View(objEligibility270_Details);
        }
        public ActionResult Eligibility270Details(string TotalCount, string Status)
        {
            Eligibility270_Details objEligibility270_Details = new Eligibility270_Details();
            objEligibility270_Details.Total_File = TotalCount;
            objEligibility270_Details.Status = Status;
            objEligibility270_Details.objVisitDetailsList = _entityContext.SP_GetDetails270(TotalCount, "270").ToList();
            return View(objEligibility270_Details);
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
        public ActionResult TransCount(string TotalCount)
        {
            ProcessSummaryModel1 objProcessSummaryModel1 = new ProcessSummaryModel1();
            var a = _entityContext.SP_GetData_Wise270(TotalCount).ToList();
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
            return PartialView("EligibilityPartialView", objProcessSummaryModel1);
        }
        //end

        public ActionResult Claims276()
        {
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
            return View();
        }

        public ActionResult Claims270Details()
        {
            string TotalCount = "";
            Eligibility270_Details objEligibility270_Details = new Eligibility270_Details();
            objEligibility270_Details.objVisitDetailsList = _entityContext.SP_GetDetails270(TotalCount, "270").ToList();
            return View(objEligibility270_Details);
        }

        public ActionResult DailyAudit270(string ErrorCount, string error_claimcount)
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


        public ActionResult IntakeClaimData(string FileID, string error_claimcount)
        {
            ViewBag.FileID = FileID;
            ViewBag.error_claimcount = error_claimcount;
            return View();
        }

        public ActionResult IntakeClaimDataError(string FileID)
        {
            ViewBag.FileID = FileID;
            return View();
        }

        public ActionResult ClaimLineData(string FileID, string ClaimID)
        {
            ViewBag.FileID = FileID;
            ViewBag.ClaaimID = ClaimID;
            return View();
        }

        #region Details Method

        [HttpGet]
        public ActionResult IntakeClaimDetails(int SeqID)
        {
            IntakeClaimDetailsViewModel intakeClaimData = GetIntakeClaimDetails(SeqID);

            return View(intakeClaimData);
        }

        public ActionResult ClaimLineDataDetails(int SeqID)
        {
            IntakeClaimLineDetailsViewModel intakeClaimLineData = GetIntakeClaimLineDetails(SeqID);
            return View(intakeClaimLineData);
        }

        //Edit claim Details
        [HttpGet]
        public ActionResult IntakeClaimDetailsEdit(int SeqID)
        {
            var data = _entityContext.IntakeClaimDatas.Where(x => x.SeqID == SeqID).FirstOrDefault();
            if (data != null)
            {
                IntakeClaimDetailsViewModel intakeClaimDataEdit = GetIntakeClaimDetails(SeqID);
                TempData["SeqID"] = SeqID;
                TempData.Keep();
                //return View(data);
                var ClaimStatusList = GetClaimStatusList();
                var ICDCodeList = GetICDCodeList();
                var Claim_Status = _entityContext.Claim_Status.Where(x => x.Claim_Status_desc == intakeClaimDataEdit.ClaimStatus).FirstOrDefault();
                if (Claim_Status == null)
                {
                    ViewBag.DepartmentId = new SelectList(ClaimStatusList, "Value", "Text", 0);
                }
                else
                {
                    ViewBag.DepartmentId = new SelectList(ClaimStatusList, "Value", "Text", Claim_Status.Claim_Status_id);
                }
                var ICDCode = _entityContext.Claims_ICD_CODE.Where(x => x.ICD_CODE == intakeClaimDataEdit.ICDCode).FirstOrDefault();
                if (ICDCode == null)
                {
                    ViewBag.ICDCode = new SelectList(ICDCodeList, "Value", "Text", 0);
                }
                else
                {
                    ViewBag.ICDCode = new SelectList(ICDCodeList, "Value", "Text", ICDCode.SeqId);
                }
                return View(intakeClaimDataEdit);
            }

            return View();
        }

        //update claim Details
        [HttpPost]
        public ActionResult IntakeClaimDetailsEdit(IntakeClaimDetailsViewModel Obj)
        {
            Int32 SeqID = (int)TempData["SeqID"];
            if (ModelState.IsValid)
            {
                Int32 ICDCodeValue = Convert.ToInt32(Request.Form["ICDCode"]);
                Int32 ClaimStatusValue = Convert.ToInt32(Request.Form["DepartmentId"]);

                if (ClaimStatusValue != 0)
                {
                    var Claim_Status = _entityContext.Claim_Status.Where(x => x.Claim_Status_id == ClaimStatusValue).FirstOrDefault();
                    Obj.ClaimStatus = Claim_Status.Claim_Status_desc;
                }
                else
                {
                    Obj.ClaimStatus = "";
                }
                if (ICDCodeValue != 0)
                {
                    var ICD_Code = _entityContext.Claims_ICD_CODE.Where(x => x.SeqId == ICDCodeValue).FirstOrDefault();
                    Obj.ICDCode = ICD_Code.ICD_CODE;
                }
                else
                {
                    Obj.ICDCode = "";
                }
                var data = _entityContext.IntakeClaimDatas.Where(x => x.SeqID == SeqID).FirstOrDefault();
                if (data != null)
                {
                    data.ClaimExtNmbr = (Obj.ClaimExtNmbr == null || Obj.ClaimExtNmbr == "") ? data.ClaimExtNmbr : Obj.ClaimExtNmbr;
                    data.ClaimStatus = (Obj.ClaimStatus == null || Obj.ClaimStatus == "") ? data.ClaimStatus : Obj.ClaimStatus;
                    data.HI01 = (Obj.ICDCode == null || Obj.ICDCode == "") ? data.HI01 : Obj.ICDCode;
                    _entityContext.SaveChanges();
                }
            }
            return Redirect(Url.Action("IntakeClaimDetails", "Claims") + "?SeqID=" + SeqID);
            //return RedirectToAction("IntakeClaimDetails", "Claims", new { id = SeqID });
            //return RedirectToAction("Index");
            //return View(Details);

        }


        #endregion

        //VK Dated 27022019
        #region New Page For Claim Data

        public ActionResult IntakeClaimDataDetails(string FileID)
        {
            FileIntakeClaimViewModel intakeClaimData = GetFileIntakeWithClaims(FileID);
            return View(intakeClaimData);
        }
        private static List<SelectListItem> GetClaims(string FileID)
        {
            HiPaaS_website_offshoreEntities _entityContext = new HiPaaS_website_offshoreEntities();

            List<SelectListItem> claimList = (from p in _entityContext.IntakeClaimDatas.AsEnumerable().Where(x => x.FileID == FileID)
                                              select new SelectListItem
                                              {
                                                  Text = p.ClaimID,
                                                  Value = p.SeqID.ToString()
                                              }).ToList();
            //Add Default Item at First Position.
            claimList.Insert(0, new SelectListItem { Text = "--Select Claim--", Value = "" });
            return claimList;
        }

        #endregion

        #endregion

        #region AjaxDataTable methods

        public JsonResult GetFileIntakes()
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



            var data = _entityContext.FileInTakes.OrderByDescending(p => p.FileDate).ToList()
                .Select(x => new FileIntakeListViewModel
                {
                    FileID = x.FileID,
                    FileName = x.FileName,
                    FileStatus = x.ExtraField2,
                    FileDate = DateTime.ParseExact(x.FileDate, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture).ToString("MM/dd/yyy HH:mm tt"),
                    ISA09 = x.SubmitterID,
                    ISA10 = x.GSA02,
                    GSA04 = x.GSA03,
                    GSA05 = x.GSA07,
                    ST01 = x.ST01,
                    ST02 = x.ST02,
                    ST03 = x.ST03,
                    BHT03 = x.BHT03,
                    CreateDateTime = DateTime.ParseExact(x.CreateDateTime, "yyyyMMdd HHmmss.fff", CultureInfo.InvariantCulture).ToString("MM/dd/yyy HH:mm tt"),
                        //Created_Date = x.Created_Date != null ? x.Created_Date.Value.ToString("MM/dd/yyyy") : string.Empty,
                        //Created_Date = DateTime.ParseExact(x.Created_Date, "yyyyMMdd HHmmss.fff", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy"),
                        Created_Date = x.Created_Date,
                    Receiver_N103 = x.Receiver_N103,
                    Submitter_N103 = x.Submitter_N103,
                    ClaimCount = x.ClaimCount == null ? 0 : x.ClaimCount,
                    error_claimcount = x.error_claimcount == null ? 0 : x.error_claimcount

                });

            int totalRecords = data.Count();
            // Filter record count.
            int recFilter = data.Count();

            // Apply pagination.
            data = data.Skip(startRec).Take(pageSize).ToList();

            // Apply Filter
            data = FilterFileIntakeData((searchFileName != null && searchFileName.Length > 0) ? searchFileName[0] : null,
                                            dtFrom, dtTo,
                                            (searchFileStatus != null && searchFileStatus.Length > 0) ? searchFileStatus[0] : null,
                                            (searchSubmitter != null && searchSubmitter.Length > 0) ? searchSubmitter[0] : null,
                                            (searchReceiver != null && searchReceiver.Length > 0) ? searchReceiver[0] : null,
                                            data);

            // Apply Sorting
            data = SortFileInTake(orderBy, columnName, data);

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
        public JsonResult GetClaimLineData(string FileID, string ClaimID)
        {

            JsonResult result = new JsonResult();

            string draw = Request.Form.GetValues("draw")[0];

            string orderBy = Request.Params["order[0][dir]"];
            string columnIndex = Request.Params["order[0][column]"];
            string columnName = Request.Params["columns[" + columnIndex + "][data]"];

            //Get parameters
            // get Start (paging start index) and length (page size for paging)
            var searchSVD01 = Request.Form.GetValues("columns[1][search][value]");
            var searchSVD03 = Request.Form.GetValues("columns[2][search][value]");
            var searchFileDate = Request.Form.GetValues("columns[3][search][value]");
            var searchSBR02 = Request.Form.GetValues("columns[4][search][value]");

            DateTime? dtFrom = null, dtTo = null;

            if (searchFileDate != null && searchFileDate.Length > 0 && !string.IsNullOrEmpty(searchFileDate[0]))
            {
                var split = searchFileDate[0].Split('-');
                dtFrom = !string.IsNullOrEmpty(split[0]) ? (DateTime?)Convert.ToDateTime(split[0]) : null;
                dtTo = !string.IsNullOrEmpty(split[1]) ? (DateTime?)Convert.ToDateTime(split[1]) : null;
            }


            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);


            var data = _entityContext.IntakeClaimLineDatas.Where(x => x.FileID == FileID && x.ClaimID == ClaimID).ToList()
                .Select(claimLineData => new IntakeClaimLineDataListViewModel
                {
                    ClaimLineDataSeqID = claimLineData.SeqID,
                    ServiceDate = claimLineData.ExtraField1 != null ? claimLineData.ExtraField1 : String.Empty,
                    AMT01_PayerPaidAmmount = claimLineData.AMT01_PayerPaidAmmount,
                    AMT01_RemainingPatientLiability = claimLineData.AMT01_RemainingPatientLiability,
                    AMT02_Amount = claimLineData.AMT02_Amount,
                    AMT02_RPAmount = claimLineData.AMT02_RPAmount,
                    BatchID = claimLineData.BatchID,
                    ClaimID = claimLineData.ClaimID,
                    FileID = claimLineData.FileID,
                    LineCheckOrRemittanceDate = claimLineData.LineCheckOrRemittanceDate != null ? DateTime.ParseExact(claimLineData.LineCheckOrRemittanceDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : String.Empty,
                        //RemainingPatientLiability = claimLineData.RemainingPatientLiability,
                        SBR02 = claimLineData.SBR02,
                    SBR03 = claimLineData.SBR03,
                    SBR04 = claimLineData.SBR04,
                    SBR09 = claimLineData.SBR09,
                    ServiceFacilityLocationName = claimLineData.ServiceFacilityLocationName,
                    ServiceFacilityLocation_NM102 = claimLineData.ServiceFacilityLocation_NM102,
                    ServiceFacilityLocation_NM108 = claimLineData.ServiceFacilityLocation_NM108,
                    ServiceFacilityLocation_NM109 = claimLineData.ServiceFacilityLocation_NM109,
                    SVD01 = claimLineData.SVD01,
                    SVD02 = claimLineData.SVD02,
                    SVD03 = claimLineData.SVD03,
                    SVD05 = claimLineData.SVD05,
                    ErrorCode = claimLineData.ErrorCode,
                    ErrorDesc = claimLineData.ClaimLineLevelErrors,
                    AmmountOwed = claimLineData.RemainingPatientLiability,
                    LX = claimLineData.LX,

                    ServiceFacilityLocationAddress = (claimLineData.ServiceFacilityLocationAddress != null ? (claimLineData.ServiceFacilityLocationAddress + ", ") : string.Empty)
                                                       + (claimLineData.ServiceFacilityLocation_City_State_Zip != null ? claimLineData.ServiceFacilityLocation_City_State_Zip : string.Empty)

                });

            int totalRecords = data.Count();
            // Filter record count.
            int recFilter = data.Count();

            // Apply pagination.
            data = data.Skip(startRec).Take(pageSize).ToList();

            // Apply Filter
            data = FilterClaimLineData((searchSVD01 != null && searchSVD01.Length > 0) ? searchSVD01[0] : null,
                                            dtFrom, dtTo,
                                            (searchSVD03 != null && searchSVD03.Length > 0) ? searchSVD03[0] : null,
                                            (searchSBR02 != null && searchSBR02.Length > 0) ? searchSBR02[0] : null,
                                            data);


            // Apply Sorting
            data = SortClaimLineData(orderBy, columnName, data);

            result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
            return result;
        }


        public JsonResult GetIntakeClaimData(string FileID, string error_claimcount)
        {
            JsonResult result = new JsonResult();

            string draw = Request.Form.GetValues("draw")[0];

            //string orderBy = Request.Params["order[0][dir]"];
            string orderBy = "desc";
            string columnIndex = Request.Params["order[0][column]"];
            string columnName = Request.Params["columns[" + columnIndex + "][data]"];

            //Get parameters
            // get Start (paging start index) and length (page size for paging)
            var searchCLaimID = Request.Form.GetValues("columns[1][search][value]");
            var searchclaimStatus = Request.Form.GetValues("columns[2][search][value]");
            var searchAdmissionDate = Request.Form.GetValues("columns[3][search][value]");
            var searchclaimTrackingID = Request.Form.GetValues("columns[4][search][value]");

            DateTime? dtFrom = null, dtTo = null;

            if (searchAdmissionDate != null && searchAdmissionDate.Length > 0 && !string.IsNullOrEmpty(searchAdmissionDate[0]))
            {
                var split = searchAdmissionDate[0].Split('-');
                dtFrom = !string.IsNullOrEmpty(split[0]) ? (DateTime?)Convert.ToDateTime(split[0]) : null;
                dtTo = !string.IsNullOrEmpty(split[1]) ? (DateTime?)Convert.ToDateTime(split[1]) : null;
            }


            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);

            if (error_claimcount == "0")
            {
                var data = _entityContext.IntakeClaimDatas.Where(x => x.FileID == FileID && (x.ErrorCode == null || x.ErrorCode == "")).ToList()
               .Select(x => new IntakeClaimDataListViewModel
               {
                   ClaimDataSeqID = x.SeqID,
                   Adjust = x.Adjust,
                       //AdmissionDate = x.AdmissionDate != null ? x.AdmissionDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                       AdmissionDate = x.AdmissionDate,// != null ? x.AdmissionDate.Value.ToString("MM/dd/yyyy") : string.Empty
                   ClaimCode = x.ClaimCode,
                   ClaimExtNmbr = x.ClaimExtNmbr,
                   ClaimID = x.ClaimID,
                   ClaimStatus = x.ClaimStatus,
                   ClaimTMTrackingID = x.ClaimTMTrackingID,
                   Claim_Amount = x.Claim_Amount,
                   BillingProvider = (x.BillingProviderFirstName == null ? "" : x.BillingProviderFirstName) + " "
                              + (x.BillingProviderLastName == null ? "" : x.BillingProviderLastName),
                   BillingProviderAddress = x.BillingProviderAddress + "," + x.BillingProviderCity_State_Zip.Replace("*", ","),
                   DischargeHour = x.DischargeHour,
                   FileID = x.FileID,
                   InsuranceBalance = x.InsuranceBalance,
                   NetBalance = x.NetBalance,
                   PatientFirstName = x.PatientFirstName,
                   PatientLastName = x.PatientLastName,
                   PatientPaid = x.PatientPaid,
                   SubscriberFirstName = x.SubscriberFirstName,
                   SubscriberLastName = x.SubscriberLastName,
                   AccountNumber = x.Member_Account_Number,

                   Payer = ((x.PayerFirstName == null ? "" : x.PayerFirstName) + " "
                              + (x.PayerLastName == null ? "" : x.PayerLastName)),

                   PayerAddress = (x.PayerAddress != null ? (x.PayerAddress + ",") : string.Empty)
                                  + x.PayerCity_State_Zip != null ? x.PayerCity_State_Zip : string.Empty,
                   ErrorCode = x.ErrorCode == null ? "" : x.ErrorCode,
                   ErrorDesc = x.ErrorDesc == null ? "" : x.ErrorCode,
                       //Field1 = x.Field1,
                       Field2 = x.ClaimLevelErrors,
                   Field3 = x.Field3,
                   Field4 = x.Field4
               });
                int totalRecords = data.Count();
                // Filter record count.
                int recFilter = data.Count();

                // Apply pagination.
                data = data.Skip(startRec).Take(pageSize).ToList();

                // Apply Filter
                data = FilterIntakeClaimData((searchCLaimID != null && searchCLaimID.Length > 0) ? searchCLaimID[0] : null,
                                                dtFrom, dtTo,
                                                (searchclaimStatus != null && searchclaimStatus.Length > 0) ? searchclaimStatus[0] : null,
                                                (searchclaimTrackingID != null && searchclaimTrackingID.Length > 0) ? searchclaimTrackingID[0] : null,
                                                data);

                // Apply Sorting
                data = SortIntakeClaimData(orderBy, columnName, data);

                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
                return result;
            }
            else
            {
                var data = _entityContext.IntakeClaimDatas.Where(x => x.FileID == FileID && (x.ErrorCode != null || x.ErrorCode != "")).ToList()
                .Select(x => new IntakeClaimDataListViewModel
                {
                    ClaimDataSeqID = x.SeqID,
                    Adjust = x.Adjust,
                        //AdmissionDate = x.AdmissionDate != null ? x.AdmissionDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                        AdmissionDate = x.AdmissionDate,// != null ? x.AdmissionDate.Value.ToString("MM/dd/yyyy") : string.Empty
                    ClaimCode = x.ClaimCode,
                    ClaimExtNmbr = x.ClaimExtNmbr,
                    ClaimID = x.ClaimID,
                    ClaimStatus = x.ClaimStatus,
                    ClaimTMTrackingID = x.ClaimTMTrackingID,
                    Claim_Amount = x.Claim_Amount,
                    BillingProvider = (x.BillingProviderFirstName == null ? "" : x.BillingProviderFirstName) + " "
                               + (x.BillingProviderLastName == null ? "" : x.BillingProviderLastName),
                    BillingProviderAddress = x.BillingProviderAddress + "," + x.BillingProviderCity_State_Zip.Replace("*", ","),
                    DischargeHour = x.DischargeHour,
                    FileID = x.FileID,
                    InsuranceBalance = x.InsuranceBalance,
                    NetBalance = x.NetBalance,
                    PatientFirstName = x.PatientFirstName,
                    PatientLastName = x.PatientLastName,
                    PatientPaid = x.PatientPaid,
                    SubscriberFirstName = x.SubscriberFirstName,
                    SubscriberLastName = x.SubscriberLastName,
                    AccountNumber = x.Member_Account_Number,

                    Payer = ((x.PayerFirstName == null ? "" : x.PayerFirstName) + " "
                               + (x.PayerLastName == null ? "" : x.PayerLastName)),

                    PayerAddress = (x.PayerAddress != null ? (x.PayerAddress + ",") : string.Empty)
                                   + x.PayerCity_State_Zip != null ? x.PayerCity_State_Zip : string.Empty,
                    ErrorCode = x.ErrorCode,
                    ErrorDesc = x.ErrorDesc,
                        //Field1 = x.Field1,
                        Field2 = x.ClaimLevelErrors,
                    Field3 = x.Field3,
                    Field4 = x.Field4
                });
                int totalRecords = data.Count();
                // Filter record count.
                int recFilter = data.Count();

                // Apply pagination.
                data = data.Skip(startRec).Take(pageSize).ToList();

                // Apply Filter
                data = FilterIntakeClaimData((searchCLaimID != null && searchCLaimID.Length > 0) ? searchCLaimID[0] : null,
                                                dtFrom, dtTo,
                                                (searchclaimStatus != null && searchclaimStatus.Length > 0) ? searchclaimStatus[0] : null,
                                                (searchclaimTrackingID != null && searchclaimTrackingID.Length > 0) ? searchclaimTrackingID[0] : null,
                                                data);

                // Apply Sorting
                data = SortIntakeClaimData(orderBy, columnName, data);

                result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
                return result;
            }





        }

        public JsonResult GetIntakeClaimDataError(string FileID)
        {
            JsonResult result = new JsonResult();

            string draw = Request.Form.GetValues("draw")[0];

            string orderBy = Request.Params["order[0][dir]"];
            string columnIndex = Request.Params["order[0][column]"];
            string columnName = Request.Params["columns[" + columnIndex + "][data]"];

            //Get parameters
            // get Start (paging start index) and length (page size for paging)
            var searchCLaimID = Request.Form.GetValues("columns[1][search][value]");
            var searchclaimStatus = Request.Form.GetValues("columns[2][search][value]");
            var searchAdmissionDate = Request.Form.GetValues("columns[3][search][value]");
            var searchclaimTrackingID = Request.Form.GetValues("columns[4][search][value]");

            DateTime? dtFrom = null, dtTo = null;

            if (searchAdmissionDate != null && searchAdmissionDate.Length > 0 && !string.IsNullOrEmpty(searchAdmissionDate[0]))
            {
                var split = searchAdmissionDate[0].Split('-');
                dtFrom = !string.IsNullOrEmpty(split[0]) ? (DateTime?)Convert.ToDateTime(split[0]) : null;
                dtTo = !string.IsNullOrEmpty(split[1]) ? (DateTime?)Convert.ToDateTime(split[1]) : null;
            }


            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);


            var data = _entityContext.IntakeClaimDatas.Where(x => x.FileID == FileID && x.ErrorCode == "Not OK").ToList()
               .Select(x => new IntakeClaimDataListViewModel
               {
                   ClaimDataSeqID = x.SeqID,
                   Adjust = x.Adjust,
                       //AdmissionDate = x.AdmissionDate != null ? x.AdmissionDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                       AdmissionDate = x.AdmissionDate,// != null ? x.AdmissionDate.Value.ToString("MM/dd/yyyy") : string.Empty
                   ClaimCode = x.ClaimCode,
                   ClaimExtNmbr = x.ClaimExtNmbr,
                   ClaimID = x.ClaimID,
                   ClaimStatus = x.ClaimStatus,
                   ClaimTMTrackingID = x.ClaimTMTrackingID,
                   Claim_Amount = x.Claim_Amount,
                   BillingProvider = (x.BillingProviderFirstName == null ? "" : x.BillingProviderFirstName) + " "
                              + (x.BillingProviderLastName == null ? "" : x.BillingProviderLastName),
                   BillingProviderAddress = x.BillingProviderAddress + "," + x.BillingProviderCity_State_Zip.Replace("*", ","),
                   DischargeHour = x.DischargeHour,
                   FileID = x.FileID,
                   InsuranceBalance = x.InsuranceBalance,
                   NetBalance = x.NetBalance,
                   PatientFirstName = x.PatientFirstName,
                   PatientLastName = x.PatientLastName,
                   PatientPaid = x.PatientPaid,
                   SubscriberFirstName = x.SubscriberFirstName,
                   SubscriberLastName = x.SubscriberLastName,
                   AccountNumber = x.Member_Account_Number,

                   Payer = ((x.PayerFirstName == null ? "" : x.PayerFirstName) + " "
                              + (x.PayerLastName == null ? "" : x.PayerLastName)),

                   PayerAddress = (x.PayerAddress != null ? (x.PayerAddress + ",") : string.Empty)
                                  + x.PayerCity_State_Zip != null ? x.PayerCity_State_Zip : string.Empty,
                   ErrorCode = x.ErrorCode,
                   ErrorDesc = x.ErrorDesc,
                       //Field1 = x.Field1,
                       Field2 = x.ClaimLevelErrors
               });

            int totalRecords = data.Count();
            // Filter record count.
            int recFilter = data.Count();

            // Apply pagination.
            data = data.Skip(startRec).Take(pageSize).ToList();

            // Apply Filter
            data = FilterIntakeClaimData((searchCLaimID != null && searchCLaimID.Length > 0) ? searchCLaimID[0] : null,
                                            dtFrom, dtTo,
                                            (searchclaimStatus != null && searchclaimStatus.Length > 0) ? searchclaimStatus[0] : null,
                                            (searchclaimTrackingID != null && searchclaimTrackingID.Length > 0) ? searchclaimTrackingID[0] : null,
                                            data);

            // Apply Sorting
            data = SortIntakeClaimData(orderBy, columnName, data);

            result = this.Json(new { draw = Convert.ToInt32(draw), recordsTotal = totalRecords, recordsFiltered = recFilter, data = data }, JsonRequestBehavior.AllowGet);
            return result;
        }

        #endregion

        #region Helper Methods
        //VK Dated 27/02/2019
        private FileIntakeClaimViewModel GetFileIntakeWithClaims(string FileID)
        {
            var fileIntakeData = _entityContext.FileInTakes.Where(x => x.FileID == FileID).FirstOrDefault();

            FileIntakeClaimViewModel FileIntakeClaim = new FileIntakeClaimViewModel();
            FileIntakeClaim.ClaimNoList = GetClaimNoList();
            FileIntakeClaim.FileData = BindFileIntakeData(fileIntakeData);
            return FileIntakeClaim;
        }
        private IntakeClaimDataDetailsViewModel GetIntakeClaimDataDetails(int SeqID)
        {
            var cliamData = _entityContext.IntakeClaimDatas.Where(x => x.SeqID == SeqID).FirstOrDefault();
            var fileIntakeData = _entityContext.FileInTakes.Where(x => x.FileID == cliamData.FileID).FirstOrDefault();
            //new code
            var data = _entityContext.IntakeClaimLineDatas.Where(x => x.FileID == cliamData.FileID && x.ClaimID == cliamData.ClaimID).ToList()
                .Select(claimLineData => new IntakeClaimLineDataListViewModel
                {
                    ClaimLineDataSeqID = claimLineData.SeqID,
                    ServiceDate = claimLineData.ExtraField1 != null ? claimLineData.ExtraField1 : String.Empty,
                    AMT01_PayerPaidAmmount = claimLineData.AMT01_PayerPaidAmmount,
                    AMT01_RemainingPatientLiability = claimLineData.AMT01_RemainingPatientLiability,
                    AMT02_Amount = claimLineData.AMT02_Amount,
                    AMT02_RPAmount = claimLineData.AMT02_RPAmount,
                    BatchID = claimLineData.BatchID,
                    ClaimID = claimLineData.ClaimID,
                    FileID = claimLineData.FileID,
                    LineCheckOrRemittanceDate = claimLineData.LineCheckOrRemittanceDate != null ? DateTime.ParseExact(claimLineData.LineCheckOrRemittanceDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : String.Empty,
                        //RemainingPatientLiability = claimLineData.RemainingPatientLiability,
                        SBR02 = claimLineData.SBR02,
                    SBR03 = claimLineData.SBR03,
                    SBR04 = claimLineData.SBR04,
                    SBR09 = claimLineData.SBR09,
                    ServiceFacilityLocationName = claimLineData.ServiceFacilityLocationName,
                    ServiceFacilityLocation_NM102 = claimLineData.ServiceFacilityLocation_NM102,
                    ServiceFacilityLocation_NM108 = claimLineData.ServiceFacilityLocation_NM108,
                    ServiceFacilityLocation_NM109 = claimLineData.ServiceFacilityLocation_NM109,
                    SVD01 = claimLineData.SVD01,
                    SVD02 = claimLineData.SVD02,
                    SVD03 = claimLineData.SVD03,
                    SVD05 = claimLineData.SVD05,
                    ErrorCode = claimLineData.ErrorCode,
                    ErrorDesc = claimLineData.ClaimLineLevelErrors,
                    AmmountOwed = claimLineData.RemainingPatientLiability,
                    LX = claimLineData.LX,

                    ServiceFacilityLocationAddress = (claimLineData.ServiceFacilityLocationAddress != null ? (claimLineData.ServiceFacilityLocationAddress + ", ") : string.Empty)
                                                       + (claimLineData.ServiceFacilityLocation_City_State_Zip != null ? claimLineData.ServiceFacilityLocation_City_State_Zip : string.Empty)

                });
            IntakeClaimDataDetailsViewModel intakeClaimData = new IntakeClaimDataDetailsViewModel();
            //IntakeClaimDataDetailsViewModel intakeClaimData = new IntakeClaimDataDetailsViewModel
            //{
            //    ClaimDataSeqID = cliamData.SeqID,
            //    Adjust = cliamData.Adjust,
            //    AdmissionDate = cliamData.AdmissionDate != null ? cliamData.AdmissionDate.ToString() : string.Empty,
            //    //AdmissionDate = cliamData.AdmissionDate != null ? DateTime.ParseExact(cliamData.AdmissionDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : string.Empty,
            //    ClaimCode = cliamData.ClaimCode,
            //    ClaimExtNmbr = cliamData.ClaimExtNmbr,
            //    ClaimID = cliamData.ClaimID,
            //    ClaimStatus = cliamData.ClaimStatus,
            //    ClaimTMTrackingID = cliamData.ClaimTMTrackingID,
            //    Claim_Amount = cliamData.Claim_Amount,
            //    BillingProvider = (cliamData.BillingProviderFirstName == null ? "" : cliamData.BillingProviderFirstName) + " "
            //                   + (cliamData.BillingProviderLastName == null ? "" : cliamData.BillingProviderLastName),
            //    BillingProviderAddress = cliamData.BillingProviderAddress + "," + cliamData.BillingProviderCity_State_Zip.Replace("*", ","),
            //    DischargeHour = cliamData.DischargeHour,
            //    FileID = cliamData.FileID,
            //    InsuranceBalance = cliamData.InsuranceBalance,
            //    NetBalance = cliamData.NetBalance,
            //    PatientPaid = cliamData.PatientPaid,
            //    PatientFirstName = cliamData.PatientFirstName,
            //    PatientLastname = cliamData.PatientLastName,
            //    Payer = ((cliamData.PayerFirstName == null ? "" : cliamData.PayerFirstName) + " "
            //                   + (cliamData.PayerLastName == null ? "" : cliamData.PayerLastName)),

            //    PayerAddress = (cliamData.PayerAddress != null ? (cliamData.PayerAddress + ",") : string.Empty)
            //                       + cliamData.PayerCity_State_Zip != null ? cliamData.PayerCity_State_Zip : string.Empty,
            //    ClaimLevelICDErrorFlag = cliamData.ClaimLevelICDErrorFlag == null ? "0" : cliamData.ClaimLevelICDErrorFlag,
            //    ClaimLevelCLMErrorFlag = cliamData.ClaimLevelCLMErrorFlag == null ? "0" : cliamData.ClaimLevelCLMErrorFlag,
            //    ICDCode = cliamData.HI01 == null ? "" : cliamData.HI01
            //};

            intakeClaimData.ClaimNoList = GetClaimNoList();
            intakeClaimData.FileData = BindFileIntakeData(fileIntakeData);
            //intakeClaimData.IntakeClaimLineData = data;
            return intakeClaimData;
        }
        private IntakeClaimDetailsNewViewModel GetIntakeClaimDetailsNew(int SeqID)
        {
            var ClaimStatusList = GetClaimStatusList();
            var cliamData = _entityContext.IntakeClaimDatas.Where(x => x.SeqID == SeqID).FirstOrDefault();
            var fileIntakeData = _entityContext.FileInTakes.Where(x => x.FileID == cliamData.FileID).FirstOrDefault();
            //new code
            var data = _entityContext.IntakeClaimLineDatas.Where(x => x.FileID == cliamData.FileID && x.ClaimID == cliamData.ClaimID).ToList()
                .Select(claimLineData => new IntakeClaimLineDataListViewModel
                {
                    ClaimLineDataSeqID = claimLineData.SeqID,
                    ServiceDate = claimLineData.ExtraField1 != null ? claimLineData.ExtraField1 : String.Empty,
                        //ServiceDate = string.Empty,
                        AMT01_PayerPaidAmmount = claimLineData.AMT01_PayerPaidAmmount,
                    AMT01_RemainingPatientLiability = claimLineData.AMT01_RemainingPatientLiability,
                    AMT02_Amount = claimLineData.AMT02_Amount,
                    AMT02_RPAmount = claimLineData.AMT02_RPAmount,
                    BatchID = claimLineData.BatchID,
                    ClaimID = claimLineData.ClaimID,
                    FileID = claimLineData.FileID,
                    LineCheckOrRemittanceDate = claimLineData.LineCheckOrRemittanceDate != null ? DateTime.ParseExact(claimLineData.LineCheckOrRemittanceDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : String.Empty,
                        //RemainingPatientLiability = claimLineData.RemainingPatientLiability,
                        SBR02 = claimLineData.SBR02,
                    SBR03 = claimLineData.SBR03,
                    SBR04 = claimLineData.SBR04,
                    SBR09 = claimLineData.SBR09,
                    ServiceFacilityLocationName = claimLineData.ServiceFacilityLocationName,
                    ServiceFacilityLocation_NM102 = claimLineData.ServiceFacilityLocation_NM102,
                    ServiceFacilityLocation_NM108 = claimLineData.ServiceFacilityLocation_NM108,
                    ServiceFacilityLocation_NM109 = claimLineData.ServiceFacilityLocation_NM109,
                    SVD01 = claimLineData.SVD01,
                    SVD02 = claimLineData.SVD02,
                    SVD03 = claimLineData.SVD03,
                    SVD05 = claimLineData.SVD05,
                    ErrorCode = claimLineData.ErrorCode,
                    ErrorDesc = claimLineData.ClaimLineLevelErrors,
                    AmmountOwed = claimLineData.RemainingPatientLiability,
                    LX = claimLineData.LX,
                    adjustment = claimLineData.adjustment,
                    type_of_adjustment = claimLineData.type_of_adjustment,
                    RemainigAmt = claimLineData.RemainigAmt,

                    ServiceFacilityLocationAddress = (claimLineData.ServiceFacilityLocationAddress != null ? (claimLineData.ServiceFacilityLocationAddress + ", ") : string.Empty)
                                                       + (claimLineData.ServiceFacilityLocation_City_State_Zip != null ? claimLineData.ServiceFacilityLocation_City_State_Zip : string.Empty)

                });

            IntakeClaimDetailsNewViewModel intakeClaimData = new IntakeClaimDetailsNewViewModel
            {
                ClaimDataSeqID = cliamData.SeqID,
                Adjust = cliamData.Adjust,
                AdmissionDate = cliamData.AdmissionDate != null ? cliamData.AdmissionDate.ToString() : string.Empty,
                //AdmissionDate = cliamData.AdmissionDate != null ? DateTime.ParseExact(cliamData.AdmissionDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : string.Empty,
                ClaimCode = cliamData.ClaimCode,
                ClaimExtNmbr = cliamData.ClaimExtNmbr,
                ClaimID = cliamData.ClaimID,
                ClaimStatus = cliamData.ClaimStatus,
                ClaimTMTrackingID = cliamData.ClaimTMTrackingID,
                Claim_Amount = cliamData.Claim_Amount,
                BillingProvider = (cliamData.BillingProviderFirstName == null ? "" : cliamData.BillingProviderFirstName) + " "
                               + (cliamData.BillingProviderLastName == null ? "" : cliamData.BillingProviderLastName),
                BillingProviderAddress = cliamData.BillingProviderAddress + "," + cliamData.BillingProviderCity_State_Zip.Replace("*", ","),
                DischargeHour = cliamData.DischargeHour,
                FileID = cliamData.FileID,
                InsuranceBalance = cliamData.InsuranceBalance,
                NetBalance = cliamData.NetBalance,
                PatientPaid = cliamData.PatientPaid,
                PatientFirstName = cliamData.PatientFirstName,
                PatientLastname = cliamData.PatientLastName,
                Payer = ((cliamData.PayerFirstName == null ? "" : cliamData.PayerFirstName) + " "
                               + (cliamData.PayerLastName == null ? "" : cliamData.PayerLastName)),

                PayerAddress = (cliamData.PayerAddress != null ? (cliamData.PayerAddress + ",") : string.Empty)
                                   + cliamData.PayerCity_State_Zip != null ? cliamData.PayerCity_State_Zip : string.Empty,
                ClaimLevelICDErrorFlag = cliamData.ClaimLevelICDErrorFlag == null ? "0" : cliamData.ClaimLevelICDErrorFlag,
                ClaimLevelCLMErrorFlag = cliamData.ClaimLevelCLMErrorFlag == null ? "0" : cliamData.ClaimLevelCLMErrorFlag,
                ICDCode = cliamData.HI01 == null ? "" : cliamData.HI01
            };

            intakeClaimData.FileData = BindFileIntakeData(fileIntakeData);
            intakeClaimData.IntakeClaimLineData = data;
            return intakeClaimData;
        }


        //Return Intake Claim Data and Binds to View Models
        private IntakeClaimDetailsViewModel GetIntakeClaimDetails(int SeqID)
        {
            var cliamData = _entityContext.IntakeClaimDatas.Where(x => x.SeqID == SeqID).FirstOrDefault();
            var fileIntakeData = _entityContext.FileInTakes.Where(x => x.FileID == cliamData.FileID).FirstOrDefault();
            //new code
            var data = _entityContext.IntakeClaimLineDatas.Where(x => x.FileID == cliamData.FileID && x.ClaimID == cliamData.ClaimID).ToList()
                .Select(claimLineData => new IntakeClaimLineDataListViewModel
                {
                    ClaimLineDataSeqID = claimLineData.SeqID,
                    ServiceDate = claimLineData.ExtraField1 != null ? claimLineData.ExtraField1 : String.Empty,
                    AMT01_PayerPaidAmmount = claimLineData.AMT01_PayerPaidAmmount,
                    AMT01_RemainingPatientLiability = claimLineData.AMT01_RemainingPatientLiability,
                    AMT02_Amount = claimLineData.AMT02_Amount,
                    AMT02_RPAmount = claimLineData.AMT02_RPAmount,
                    BatchID = claimLineData.BatchID,
                    ClaimID = claimLineData.ClaimID,
                    FileID = claimLineData.FileID,
                    LineCheckOrRemittanceDate = claimLineData.LineCheckOrRemittanceDate != null ? DateTime.ParseExact(claimLineData.LineCheckOrRemittanceDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : String.Empty,
                        //RemainingPatientLiability = claimLineData.RemainingPatientLiability,
                        SBR02 = claimLineData.SBR02,
                    SBR03 = claimLineData.SBR03,
                    SBR04 = claimLineData.SBR04,
                    SBR09 = claimLineData.SBR09,
                    ServiceFacilityLocationName = claimLineData.ServiceFacilityLocationName,
                    ServiceFacilityLocation_NM102 = claimLineData.ServiceFacilityLocation_NM102,
                    ServiceFacilityLocation_NM108 = claimLineData.ServiceFacilityLocation_NM108,
                    ServiceFacilityLocation_NM109 = claimLineData.ServiceFacilityLocation_NM109,
                    SVD01 = claimLineData.SVD01,
                    SVD02 = claimLineData.SVD02,
                    SVD03 = claimLineData.SVD03,
                    SVD05 = claimLineData.SVD05,
                    ErrorCode = claimLineData.ErrorCode,
                    ErrorDesc = claimLineData.ClaimLineLevelErrors,
                    AmmountOwed = claimLineData.RemainingPatientLiability,
                    LX = claimLineData.LX,

                    ServiceFacilityLocationAddress = (claimLineData.ServiceFacilityLocationAddress != null ? (claimLineData.ServiceFacilityLocationAddress + ", ") : string.Empty)
                                                       + (claimLineData.ServiceFacilityLocation_City_State_Zip != null ? claimLineData.ServiceFacilityLocation_City_State_Zip : string.Empty)

                });

            IntakeClaimDetailsViewModel intakeClaimData = new IntakeClaimDetailsViewModel
            {
                ClaimDataSeqID = cliamData.SeqID,
                Adjust = cliamData.Adjust,
                AdmissionDate = cliamData.AdmissionDate != null ? cliamData.AdmissionDate.ToString() : string.Empty,
                //AdmissionDate = cliamData.AdmissionDate != null ? DateTime.ParseExact(cliamData.AdmissionDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : string.Empty,
                ClaimCode = cliamData.ClaimCode,
                ClaimExtNmbr = cliamData.ClaimExtNmbr,
                ClaimID = cliamData.ClaimID,
                ClaimStatus = cliamData.ClaimStatus,
                ClaimTMTrackingID = cliamData.ClaimTMTrackingID,
                Claim_Amount = cliamData.Claim_Amount,
                BillingProvider = (cliamData.BillingProviderFirstName == null ? "" : cliamData.BillingProviderFirstName) + " "
                               + (cliamData.BillingProviderLastName == null ? "" : cliamData.BillingProviderLastName),
                BillingProviderAddress = cliamData.BillingProviderAddress + "," + cliamData.BillingProviderCity_State_Zip.Replace("*", ","),
                DischargeHour = cliamData.DischargeHour,
                FileID = cliamData.FileID,
                InsuranceBalance = cliamData.InsuranceBalance,
                NetBalance = cliamData.NetBalance,
                PatientPaid = cliamData.PatientPaid,
                PatientFirstName = cliamData.PatientFirstName,
                PatientLastname = cliamData.PatientLastName,
                Payer = ((cliamData.PayerFirstName == null ? "" : cliamData.PayerFirstName) + " "
                               + (cliamData.PayerLastName == null ? "" : cliamData.PayerLastName)),

                PayerAddress = (cliamData.PayerAddress != null ? (cliamData.PayerAddress + ",") : string.Empty)
                                   + cliamData.PayerCity_State_Zip != null ? cliamData.PayerCity_State_Zip : string.Empty,
                ClaimLevelICDErrorFlag = cliamData.ClaimLevelICDErrorFlag == null ? "0" : cliamData.ClaimLevelICDErrorFlag,
                ClaimLevelCLMErrorFlag = cliamData.ClaimLevelCLMErrorFlag == null ? "0" : cliamData.ClaimLevelCLMErrorFlag,
                ICDCode = cliamData.HI01 == null ? "" : cliamData.HI01
            };

            intakeClaimData.FileData = BindFileIntakeData(fileIntakeData);
            intakeClaimData.IntakeClaimLineData = data;
            return intakeClaimData;
        }

        private IntakeClaimLineDetailsViewModel GetIntakeClaimLineDetails(int SeqID)
        {
            var claimLineData = _entityContext.IntakeClaimLineDatas.Where(x => x.SeqID == SeqID).FirstOrDefault();
            var cliamData = _entityContext.IntakeClaimDatas.Where(x => x.ClaimID == claimLineData.ClaimID && x.FileID == claimLineData.FileID).FirstOrDefault();
            var fileIntakeData = _entityContext.FileInTakes.Where(x => x.FileID == claimLineData.FileID).FirstOrDefault();

            IntakeClaimLineDetailsViewModel intakeClaimLineData = BindIntakeClaimLineDtailsModel(claimLineData);

            intakeClaimLineData.IntakeClaimData = BindIntakeClaimDtailsModel(cliamData);

            intakeClaimLineData.FileData = BindFileIntakeData(fileIntakeData);

            return intakeClaimLineData;
        }

        private FileIntakeListViewModel BindFileIntakeData(FileInTake fileIntakeData)
        {
            return new FileIntakeListViewModel
            {
                FileID = fileIntakeData.FileID,
                FileName = fileIntakeData.FileName,
                // FileDate = fileIntakeData.FileDate != null ? fileIntakeData.FileDate.ToString() : string.Empty,
                FileDate = DateTime.ParseExact(fileIntakeData.FileDate, "yyyyMMdd HHmmss", CultureInfo.InvariantCulture).ToString("MM/dd/yyy HH:mm tt"),
                ISA09 = fileIntakeData.ISA09,
                ISA10 = fileIntakeData.ISA10,
                GSA04 = fileIntakeData.GSA04,
                GSA05 = fileIntakeData.GSA05,
                ST01 = fileIntakeData.ST01,
                ST02 = fileIntakeData.ST02,
                ST03 = fileIntakeData.ST03,
                BHT03 = fileIntakeData.BHT03,
                CreateDateTime = DateTime.ParseExact(fileIntakeData.CreateDateTime, "yyyyMMdd HHmmss.fff", CultureInfo.InvariantCulture).ToString("MM/dd/yyy HH:mm tt"),
                Created_Date = fileIntakeData.Created_Date != null ? fileIntakeData.Created_Date.ToString() : string.Empty,
                //Created_Date = fileIntakeData.Created_Date != null ? DateTime.ParseExact(fileIntakeData.Created_Date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : string.Empty,
                Receiver_N103 = fileIntakeData.Receiver_N103,
                Submitter_N103 = fileIntakeData.Submitter_N103,

                //Field1 = fileIntakeData.Field1,
                Field2 = fileIntakeData.Field2,
                Field3 = fileIntakeData.Field3,
                Field4 = fileIntakeData.Field4,
                Field5 = fileIntakeData.Field5,
                Field6 = fileIntakeData.Field6
            };
        }

        private IntakeClaimDataListViewModel BindIntakeClaimDtailsModel(IntakeClaimData cliamData)
        {
            return new IntakeClaimDataListViewModel
            {
                ClaimDataSeqID = cliamData.SeqID,
                Adjust = cliamData.Adjust,
                AdmissionDate = cliamData.AdmissionDate != null ? cliamData.AdmissionDate.ToString() : string.Empty,
                //AdmissionDate = cliamData.AdmissionDate != null ? DateTime.ParseExact(cliamData.AdmissionDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : string.Empty,
                ClaimCode = cliamData.ClaimCode,
                ClaimExtNmbr = cliamData.ClaimExtNmbr,
                ClaimID = cliamData.ClaimID,
                ClaimStatus = cliamData.ClaimStatus,
                ClaimTMTrackingID = cliamData.ClaimTMTrackingID,
                Claim_Amount = cliamData.Claim_Amount,
                BillingProvider = (cliamData.BillingProviderFirstName == null ? "" : cliamData.BillingProviderFirstName) + " "
                               + (cliamData.BillingProviderLastName == null ? "" : cliamData.BillingProviderLastName),
                BillingProviderAddress = cliamData.BillingProviderAddress + "," + cliamData.BillingProviderCity_State_Zip.Replace("*", ","),
                DischargeHour = cliamData.DischargeHour,
                FileID = cliamData.FileID,
                InsuranceBalance = cliamData.InsuranceBalance,
                NetBalance = cliamData.NetBalance,
                PatientPaid = cliamData.PatientPaid,
                SubscriberFirstName = cliamData.SubscriberFirstName,
                SubscriberLastName = cliamData.SubscriberLastName,
                AccountNumber = cliamData.Member_Account_Number,

                Payer = ((cliamData.PayerFirstName == null ? "" : cliamData.PayerFirstName) + " "
                               + (cliamData.PayerLastName == null ? "" : cliamData.PayerLastName)),

                PayerAddress = (cliamData.PayerAddress != null ? (cliamData.PayerAddress + ",") : string.Empty)
                                   + cliamData.PayerCity_State_Zip != null ? cliamData.PayerCity_State_Zip : string.Empty,

                ErrorCode = cliamData.ErrorCode,
                ErrorDesc = cliamData.ErrorDesc,
            };
        }

        private IntakeClaimLineDetailsViewModel BindIntakeClaimLineDtailsModel(IntakeClaimLineData claimLineData)
        {
            return new IntakeClaimLineDetailsViewModel
            {
                ClaimLineDataSeqID = claimLineData.SeqID,
                ServiceDate = claimLineData.ExtraField1 != null ? claimLineData.ExtraField1 : String.Empty,
                AMT01_PayerPaidAmmount = claimLineData.AMT01_PayerPaidAmmount,
                AMT01_RemainingPatientLiability = claimLineData.AMT01_RemainingPatientLiability,
                AMT02_Amount = claimLineData.AMT02_Amount,
                AMT02_RPAmount = claimLineData.AMT02_RPAmount,
                BatchID = claimLineData.BatchID,
                ClaimID = claimLineData.ClaimID,
                FileID = claimLineData.FileID,
                LineCheckOrRemittanceDate = claimLineData.LineCheckOrRemittanceDate != null ? DateTime.ParseExact(claimLineData.LineCheckOrRemittanceDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : String.Empty,
                RemainingPatientLiability = claimLineData.RemainingPatientLiability,
                SBR02 = claimLineData.SBR02,
                SBR03 = claimLineData.SBR03,
                SBR04 = claimLineData.SBR04,
                SBR09 = claimLineData.SBR09,
                ServiceFacilityLocationName = claimLineData.ServiceFacilityLocationName,
                ServiceFacilityLocation_NM102 = claimLineData.ServiceFacilityLocation_NM102,
                ServiceFacilityLocation_NM108 = claimLineData.ServiceFacilityLocation_NM108,
                ServiceFacilityLocation_NM109 = claimLineData.ServiceFacilityLocation_NM109,
                SVD01 = claimLineData.SVD01,
                SVD02 = claimLineData.SVD02,
                SVD03 = claimLineData.SVD03,
                SVD05 = claimLineData.SVD05,
                LX = claimLineData.LX,
                AmmountOwed = claimLineData.RemainingPatientLiability,
                Qty = claimLineData.SVD05,
                ServiceFacilityLocationAddress = (claimLineData.ServiceFacilityLocationAddress != null ? (claimLineData.ServiceFacilityLocationAddress + ", ") : string.Empty)
                                                       + (claimLineData.ServiceFacilityLocation_City_State_Zip != null ? claimLineData.ServiceFacilityLocation_City_State_Zip : string.Empty)
            };
        }

        private static IEnumerable<FileIntakeListViewModel> SortFileInTake(string orderBy, string columnName, IEnumerable<FileIntakeListViewModel> data)
        {
            switch (columnName)
            {
                case "FileID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.FileID);
                    else
                        data = data.OrderByDescending(x => x.FileID);
                    break;
                case "FileName":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.FileName);
                    else
                        data = data.OrderByDescending(x => x.FileName);
                    break;
                case "FileStatus":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.FileStatus);
                    else
                        data = data.OrderByDescending(x => x.FileStatus);
                    break;
                case "FileDate":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.FileDate);
                    else
                        data = data.OrderByDescending(x => x.FileDate);
                    break;
                case "ISA09":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ISA09);
                    else
                        data = data.OrderByDescending(x => x.ISA09);
                    break;
                case "ISA10":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ISA10);
                    else
                        data = data.OrderByDescending(x => x.ISA10);
                    break;
                case "GSA04":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.GSA04);
                    else
                        data = data.OrderByDescending(x => x.GSA04);
                    break;
                case "GSA05":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.GSA05);
                    else
                        data = data.OrderByDescending(x => x.GSA05);
                    break;
                case "ST01":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ST01);
                    else
                        data = data.OrderByDescending(x => x.ST01);
                    break;
                case "ST02":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ST02);
                    else
                        data = data.OrderByDescending(x => x.ST02);
                    break;
                case "ST03":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ST03);
                    else
                        data = data.OrderByDescending(x => x.ST03);
                    break;
                case "BHT03":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.BHT03);
                    else
                        data = data.OrderByDescending(x => x.BHT03);
                    break;
                case "CreateDateTime":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.CreateDateTime);
                    else
                        data = data.OrderByDescending(x => x.CreateDateTime);
                    break;
                case "Created_Date":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.Created_Date);
                    else
                        data = data.OrderByDescending(x => x.Created_Date);
                    break;
                case "Receiver_N103":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.Receiver_N103);
                    else
                        data = data.OrderByDescending(x => x.Receiver_N103);
                    break;
                case "Submitter_N103":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.Submitter_N103);
                    else
                        data = data.OrderByDescending(x => x.Submitter_N103);
                    break;
                default:
                    data = data.OrderBy(x => x.FileID);
                    break;
            }
            return data;
        }

        private IEnumerable<FileIntakeListViewModel> FilterFileIntakeData(string fileName, DateTime? dtFrom, DateTime? dtTo,
                                                                            string fileStatus, string submitter, string reciever,
                                                                            IEnumerable<FileIntakeListViewModel> data)
        {
            if (fileName != null && !fileName.Equals(""))
                data = data.Where(x => x.FileName.ToLower().Contains(fileName.ToLower()));

            if (fileStatus != null && !fileStatus.Equals(""))
                data = data.Where(x => x.FileStatus.ToLower().Contains(fileStatus.ToLower()));

            if (submitter != null && !submitter.Equals(""))
                data = data.Where(x => x.Submitter_N103.ToLower().Contains(submitter.ToLower()));

            if (reciever != null && !reciever.Equals(""))
                data = data.Where(x => x.Receiver_N103.ToLower().Contains(reciever.ToLower()));

            if (dtFrom != null)
                data = data.Where(x => Convert.ToDateTime(x.FileDate) >= dtFrom);

            if (dtTo != null)
                data = data.Where(x => Convert.ToDateTime(x.FileDate) <= dtTo);

            return data;
        }


        private IEnumerable<IntakeClaimDataListViewModel> SortIntakeClaimData(string orderBy, string columnName, IEnumerable<IntakeClaimDataListViewModel> data)
        {
            switch (columnName)
            {
                case "FileID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.FileID);
                    else
                        data = data.OrderByDescending(x => x.FileID);
                    break;
                case "ClaimID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ClaimID);
                    else
                        data = data.OrderByDescending(x => x.ClaimID);
                    break;
                case "ClaimExtNmbr":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ClaimExtNmbr);
                    else
                        data = data.OrderByDescending(x => x.ClaimExtNmbr);
                    break;
                case "ClaimStatus":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ClaimStatus);
                    else
                        data = data.OrderByDescending(x => x.ClaimStatus);
                    break;
                case "ClaimTMTrackingID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ClaimTMTrackingID);
                    else
                        data = data.OrderByDescending(x => x.ClaimTMTrackingID);
                    break;
                case "ClaimCode":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ClaimCode);
                    else
                        data = data.OrderByDescending(x => x.ClaimCode);
                    break;
                case "Claim_Amount":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.Claim_Amount);
                    else
                        data = data.OrderByDescending(x => x.Claim_Amount);
                    break;
                case "AdmissionDate":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.AdmissionDate);
                    else
                        data = data.OrderByDescending(x => x.AdmissionDate);
                    break;
                default:
                    data = data.OrderBy(x => x.FileID);
                    break;
            }
            return data;
        }

        private IEnumerable<IntakeClaimDataListViewModel> FilterIntakeClaimData(string searchclaimID, DateTime? dtFrom, DateTime? dtTo, string searchclaimStatus, string searchclaimTrackingID, IEnumerable<IntakeClaimDataListViewModel> data)
        {
            if (searchclaimID != null && !searchclaimID.Equals(""))
                data = data.Where(x => x.ClaimID.ToLower().Contains(searchclaimID.ToLower()));

            if (searchclaimStatus != null && !searchclaimStatus.Equals(""))
                data = data.Where(x => x.ClaimStatus.ToLower().Contains(searchclaimStatus.ToLower()));

            if (searchclaimTrackingID != null && !searchclaimTrackingID.Equals(""))
                data = data.Where(x => x.ClaimTMTrackingID.ToLower().Contains(searchclaimTrackingID.ToLower()));


            if (dtFrom != null)
                data = data.Where(x => x.AdmissionDate != "" ? Convert.ToDateTime(x.AdmissionDate) >= dtFrom : true);

            if (dtTo != null)
                data = data.Where(x => x.AdmissionDate != "" ? Convert.ToDateTime(x.AdmissionDate) <= dtTo : true);

            return data;
        }

        // private IEnumerable<clsView> SortClaimLineData(string orderBy, string columnName, IEnumerable<clsView> data)
        private IEnumerable<IntakeClaimLineDataListViewModel> SortClaimLineData(string orderBy, string columnName, IEnumerable<IntakeClaimLineDataListViewModel> data)
        {
            switch (columnName)
            {
                case "FileID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.FileID);
                    else
                        data = data.OrderByDescending(x => x.FileID);
                    break;
                case "AMT01_PayerPaidAmmount":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.AMT01_PayerPaidAmmount);
                    else
                        data = data.OrderByDescending(x => x.AMT01_PayerPaidAmmount);
                    break;
                case "AMT01_RemainingPatientLiability":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.AMT01_RemainingPatientLiability);
                    else
                        data = data.OrderByDescending(x => x.AMT01_RemainingPatientLiability);
                    break;
                case "AMT02_Amount":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.AMT02_Amount);
                    else
                        data = data.OrderByDescending(x => x.AMT02_Amount);
                    break;
                case "AMT02_RPAmount":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.AMT02_RPAmount);
                    else
                        data = data.OrderByDescending(x => x.AMT02_RPAmount);
                    break;
                case "BatchID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.BatchID);
                    else
                        data = data.OrderByDescending(x => x.BatchID);
                    break;
                case "ClaimID":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.ClaimID);
                    else
                        data = data.OrderByDescending(x => x.ClaimID);
                    break;
                case "CreateDateTime":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.CreateDateTime);
                    else
                        data = data.OrderByDescending(x => x.CreateDateTime);
                    break; ;
                case "LineCheckOrRemittanceDate":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.LineCheckOrRemittanceDate);
                    else
                        data = data.OrderByDescending(x => x.LineCheckOrRemittanceDate);
                    break;
                case "RemainingPatientLiability":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.RemainingPatientLiability);
                    else
                        data = data.OrderByDescending(x => x.RemainingPatientLiability);
                    break;
                case "SBR02":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.SBR02);
                    else
                        data = data.OrderByDescending(x => x.SBR02);
                    break;
                case "SVD01":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.SVD01);
                    else
                        data = data.OrderByDescending(x => x.SVD01);
                    break;
                case "SVD03":
                    if (orderBy.Equals("asc"))
                        data = data.OrderBy(x => x.SVD03);
                    else
                        data = data.OrderByDescending(x => x.SVD03);
                    break;
                default:
                    data = data.OrderBy(x => x.FileID);
                    break;
            }
            return data;
        }

        //private IEnumerable<clsView> FilterClaimLineData(string searchSVD01, DateTime? dtFrom, DateTime? dtTo, string searchSVD03, string searchSBR02, IEnumerable<clsView> data)
        private IEnumerable<IntakeClaimLineDataListViewModel> FilterClaimLineData(string searchSVD01, DateTime? dtFrom, DateTime? dtTo, string searchSVD03, string searchSBR02, IEnumerable<IntakeClaimLineDataListViewModel> data)
        {
            if (searchSVD01 != null && !searchSVD01.Equals(""))
                data = data.Where(x => x.SVD01.ToLower().Contains(searchSVD01.ToLower()));

            if (searchSVD03 != null && !searchSVD03.Equals(""))
                data = data.Where(x => x.SVD03.ToLower().Contains(searchSVD03.ToLower()));

            if (searchSBR02 != null && !searchSBR02.Equals(""))
                data = data.Where(x => x.SBR02.ToLower().Contains(searchSBR02.ToLower()));


            if (dtFrom != null)
                data = data.Where(x => Convert.ToDateTime(x.LineCheckOrRemittanceDate) >= dtFrom);

            if (dtTo != null)
                data = data.Where(x => Convert.ToDateTime(x.LineCheckOrRemittanceDate) <= dtTo);

            return data;
        }

        private IEnumerable<CAResponse> FilterCAResponse(string searchSVD01, DateTime? dtFrom, DateTime? dtTo, string searchSVD03, string searchSBR02, IEnumerable<CAResponse> data1)
        {
            return data1;
        }

        public IEnumerable<SelectListItem> GetClaimStatusList()
        {
            using (var context = new ApplicationDbContext())
            {

                List<SelectListItem> ClaimStatusList = _entityContext.Claim_Status.AsNoTracking()
                    .OrderBy(n => n.Claim_Status_id)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.Claim_Status_id.ToString(),
                            Text = n.Claim_Status_desc
                        }).ToList();
                var first_item = new SelectListItem()
                {
                    Value = 0.ToString(),
                    Text = "--- Select Claim Status ---"
                };
                ClaimStatusList.Insert(0, first_item);
                return new SelectList(ClaimStatusList, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetICDCodeList()
        {
            using (var context = new ApplicationDbContext())
            {
                List<SelectListItem> ICDCodeList = _entityContext.Claims_ICD_CODE.AsNoTracking()
                    .OrderBy(n => n.SeqId)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.SeqId.ToString(),
                            Text = n.ICD_CODE
                        }).ToList();
                var first_item = new SelectListItem()
                {
                    Value = 0.ToString(),
                    Text = "--- Select ICD Code ---"
                };
                ICDCodeList.Insert(0, first_item);
                return new SelectList(ICDCodeList, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetClaimNoList()
        {
            using (var context = new ApplicationDbContext())
            {
                List<SelectListItem> ClaimNoList = _entityContext.IntakeClaimDatas.AsNoTracking().Where(x => x.FileID == "90")
                        //.OrderBy(n => n.SeqId)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.SeqID.ToString(),
                            Text = n.ClaimID
                        }).ToList();
                var first_item = new SelectListItem()
                {
                    Value = 0.ToString(),
                    Text = "-- Select Claim No --"
                };
                ClaimNoList.Insert(0, first_item);
                return new SelectList(ClaimNoList, "Value", "Text");
            }
        }

        #endregion

        public ActionResult X12Standard()
        {
            return View();
        }

        public JsonResult GetErrorType()
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

            var data = _entityContext.ErrorTypes.OrderByDescending(p => p.ID).Where(p => p.Transaction_Type == "270").ToList()

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

    }
}
