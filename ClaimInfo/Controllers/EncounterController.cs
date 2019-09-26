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
    public class EncounterController : Controller
    {
        // GET: Encounter

        HiPaaS_website_offshoreEntities _entityContext = new HiPaaS_website_offshoreEntities();
       
        public ActionResult Index()
        {
            int SubCount = _entityContext.IntakeClaimData835.ToList().Count();
            int AccCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Accepted").ToList().Count();
            int NotAccCount = SubCount - AccCount;
            int PenCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Pending").ToList().Count();
            int RejCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Rejected").ToList().Count();
            int FailedFileCount = _entityContext.FileInTake835.Where(x => x.ExtraField2 == "File Error").ToList().Count();

            Dashboard837ViewModel model = new Dashboard837ViewModel();
            model.FileCount = _entityContext.FileInTake835.ToList().Count();
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

        public ActionResult ClaimsDailyRejection(string sSelectedDate)
        {
            int RejCount = Convert.ToInt32(_entityContext.SP_GetIntakeClaimDataErrorsCount(sSelectedDate).FirstOrDefault());
            //int RejCount = _entityContext.IntakeClaimDatas.Where(x => x.ClaimStatus == "Errors").ToList().Count();
            ViewBag.RejCount = RejCount;
            List<SP_GetRejectedClaims_Result> model = new List<SP_GetRejectedClaims_Result>();
            model = _entityContext.SP_GetRejectedClaims(sSelectedDate).ToList();
            return View(model);
        }

        public ActionResult Claims270Details()
        {
            string TotalCount = "";
            Eligibility270_Details objEligibility270_Details = new Eligibility270_Details();
            objEligibility270_Details.objVisitDetailsList = _entityContext.SP_GetDetails270(TotalCount, "270").ToList();
            return View(objEligibility270_Details);
        }

        public ActionResult ResearchQueue()
        {
            return View();
        }
        public ActionResult MatchClaims()
        {
            return View();
        }
    }
}