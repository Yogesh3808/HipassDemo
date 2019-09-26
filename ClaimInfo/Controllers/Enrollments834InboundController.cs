using ClaimInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClaimInfo.DataModel;
using System.Globalization;

namespace ClaimInfo.Controllers
{
    public class Enrollments834InboundController : Controller
    {
        HiPaaS_website_offshoreEntities _entities=new HiPaaS_website_offshoreEntities();
        // GET: Enrollments834Inbound

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Payment()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Dashboard834()
        {
            int SubCount = _entities.IntakeClaimDatas.ToList().Count();
            int AccCount = _entities.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted").ToList().Count();
            int NotAccCount = SubCount - AccCount;
            int PenCount = _entities.IntakeClaimDatas.Where(x => x.ClaimStatus == "Pending").ToList().Count();
            int RejCount = _entities.IntakeClaimDatas.Where(x => x.ClaimStatus == "Rejected").ToList().Count();
            int FailedFileCount = _entities.FileInTakes.Where(x => x.ExtraField2 == "File Error").ToList().Count();
            int PaidCount = _entities.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Paid").ToList().Count();
            int DeniedCount = _entities.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Denied").ToList().Count();
            int WIPCount = _entities.IntakeClaimDatas.Where(x => x.ClaimStatus == "Accepted" && x.adjudication_status == "Work in Progress").ToList().Count();
            //int PaidCount = 0;
            //int DeniedCount = 0;
            //int WIPCount = 0;

            Dashboard837ViewModel model = new Dashboard837ViewModel();
            model.FileCount = _entities.FileInTakes.ToList().Count();
            model.SubmittedClaimCount = SubCount;
            model.AcceptedClaimCount = AccCount;
            model.NotAcceptedClaimCount = NotAccCount;
            model.PendingClaimCount = PenCount;
            model.RejectedClaimCount = RejCount;
            //model.EDIFailLoadCount = NotAccCount - PenCount - RejCount;
            model.EDIFailLoadCount = FailedFileCount;
            model.PendingClaimError = _entities.SP_GetPendingClaimError().ToList();
            model.RejectedClaimError = _entities.SP_GetRejectedClaimError().ToList();
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
                    data = _entities.FileInTakes.Where(o => o.ExtraField2 == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "SC" || sMenu == "total")
                {
                    strClaimStatus = "";
                    data = _entities.IntakeClaimDatas.Where(o => o.FileDate.Contains(sSelectedDate)).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "AC")
                {
                    strClaimStatus = "Accepted";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "EC")
                {
                    strClaimStatus = "Rejected";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "Paid")
                {
                    adjudication_status = "Paid";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "Denied")
                {
                    adjudication_status = "Partial Paid";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "WIP")
                {
                    adjudication_status = "Work in Progress";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                if (strClaimStatus != "")
                {
                    foreach (var FileID in data)
                    {
                        if (strClaimStatus == "Accepted" && adjudication_status != "")
                        {
                            foreach (FileInTake fileInTake in _entities.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus && o.adjudication_status == adjudication_status).Take(10).ToList(),
                                    intake_claim_count = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake fileInTake in _entities.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                    intake_claim_count = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                    }

                }
                else
                {
                    foreach (FileInTake fileInTake in _entities.FileInTakes.Where(x => x.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate))
                    {
                        model.Add(new FileClaimDetails
                        {
                            FileStatus = fileInTake.ExtraField2,
                            FileData = BindFileIntakeData(fileInTake),
                            IntakeClaimDataList = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                            intake_claim_count = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
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
                    data = _entities.FileInTakes.Where(o => o.ExtraField2 == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "SC" || sMenu == "total")
                {
                    strClaimStatus = "";
                    data = _entities.IntakeClaimDatas.OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "AC")
                {
                    strClaimStatus = "Accepted";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "EC")
                {
                    strClaimStatus = "Rejected";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "Paid")
                {
                    adjudication_status = "Paid";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "Denied")
                {
                    adjudication_status = "Partial Paid";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "WIP")
                {
                    adjudication_status = "Work in Progress";
                    data = _entities.IntakeClaimDatas.Where(o => o.ClaimStatus == "Accepted" && o.adjudication_status == adjudication_status).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }

                foreach (var FileID in data)
                {
                    if (strClaimStatus != "")
                    {
                        if (strClaimStatus == "Accepted" && adjudication_status != "")
                        {
                            foreach (FileInTake fileInTake in _entities.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus && o.adjudication_status == adjudication_status).Take(10).ToList(),
                                    intake_claim_count = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake fileInTake in _entities.FileInTakes.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                    intake_claim_count = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (FileInTake fileInTake in _entities.FileInTakes.Where(x => x.FileID == FileID))
                        {
                            model.Add(new FileClaimDetails
                            {
                                FileStatus = fileInTake.ExtraField2,
                                FileData = BindFileIntakeData(fileInTake),
                                IntakeClaimDataList = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                                intake_claim_count = _entities.IntakeClaimDatas.Where(o => o.FileID == fileInTake.FileID).ToList().Count(),
                            });
                        }
                    }

                }

            }

            return model.OrderByDescending(x => x.FileData.FileDate).ToList();
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
        public ActionResult Audit()
        {
            enrollmentDetails834 objenrollmentDetails384 = new enrollmentDetails834();
            objenrollmentDetails384.objVisitDetailsList = _entities.SP_GetenrollmentDetails834().ToList();
            return View(objenrollmentDetails384);
        }
        public ActionResult Errors()
        {
            enrollmentDetails834 objenrollmentDetails384 = new enrollmentDetails834();
            objenrollmentDetails384.objVisitDetailsList = _entities.SP_GetenrollmentDetails834().ToList();
            return View(objenrollmentDetails384);
        }
        public ActionResult Outbound()
        {
            enrollmentDetails834 objenrollmentDetails384 = new enrollmentDetails834();
            objenrollmentDetails384.objVisitDetailsList = _entities.SP_GetenrollmentDetails834().ToList();
            return View(objenrollmentDetails384);
        }
    }
}