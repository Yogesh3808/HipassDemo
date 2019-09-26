using ClaimInfo.DataModel;
using ClaimInfo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Controllers
{
    public class ClaimPaymentController : Controller
    {
        HiPaaS_website_offshoreEntities _entityContext;
        public ClaimPaymentController()
        {
            _entityContext = new HiPaaS_website_offshoreEntities();
        }
        // GET: ClaimPayment
        public ActionResult Index()
        {
            int SubCount = _entityContext.IntakeClaimData835.ToList().Count();
            int AccCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Accepted").ToList().Count();
            int NotAccCount = SubCount - AccCount;
            int PenCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Pending").ToList().Count();
            int RejCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Errors").ToList().Count();
            int FailedFileCount = _entityContext.FileInTake835.Where(x => x.ExtraField2 == "File Error").ToList().Count();

            Dashboard835ViewModel model = new Dashboard835ViewModel();
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
        private List<FileClaimDetails835> GetFileDataForDashbaord()
        {
            string sSelectedDate = "";
            string sMenu = "total";
            List<FileClaimDetails835> model = new List<FileClaimDetails835>();
            if (!string.IsNullOrEmpty(sSelectedDate))
            {
                List<string> data = new List<string>();
                string strClaimStatus = "";
                string adjudication_status = "";
                if (sMenu == "FEC")
                {
                    strClaimStatus = "File Error";
                    data = _entityContext.FileInTake835.Where(o => o.ExtraField2 == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "SC" || sMenu == "total")
                {
                    strClaimStatus = "";
                    data = _entityContext.IntakeClaimData835.Where(o => o.FileDate.Contains(sSelectedDate)).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "AC")
                {
                    strClaimStatus = "Accepted";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "EC")
                {
                    strClaimStatus = "Errors";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == strClaimStatus && o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "Paid")
                {
                    adjudication_status = "Paid";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == "Accepted" &&  o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "Denied")
                {
                    adjudication_status = "Partial Paid";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == "Accepted" &&  o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "WIP")
                {
                    adjudication_status = "Work in Progress";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == "Accepted" &&  o.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                if (strClaimStatus != "")
                {
                    foreach (var FileID in data)
                    {
                        if (strClaimStatus == "Accepted" && adjudication_status != "")
                        {
                            foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails835
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus ).Take(10).ToList(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails835
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                });
                            }
                        }
                    }

                }
                else
                {
                    foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate))
                    {
                        model.Add(new FileClaimDetails835
                        {
                            FileStatus = fileInTake.ExtraField2,
                            FileData = BindFileIntakeData(fileInTake),
                            IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
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
                    data = _entityContext.FileInTake835.Where(o => o.ExtraField2 == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "SC" || sMenu == "total")
                {
                    strClaimStatus = "";
                    data = _entityContext.IntakeClaimData835.OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "AC")
                {
                    strClaimStatus = "Accepted";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "EC")
                {
                    strClaimStatus = "Errors";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                }
                else if (sMenu == "Paid")
                {
                    adjudication_status = "Paid";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == "Accepted").OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "Denied")
                {
                    adjudication_status = "Partial Paid";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == "Accepted").OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }
                else if (sMenu == "WIP")
                {
                    adjudication_status = "Work in Progress";
                    data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == "Accepted").OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    strClaimStatus = "Accepted";
                }

                foreach (var FileID in data)
                {
                    if (strClaimStatus != "")
                    {
                        if (strClaimStatus == "Accepted" && adjudication_status != "")
                        {
                            foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails835
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus ).Take(10).ToList(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails835
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                        {
                            model.Add(new FileClaimDetails835
                            {
                                FileStatus = fileInTake.ExtraField2,
                                FileData = BindFileIntakeData(fileInTake),
                                IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
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
            int totalFile = Convert.ToInt32(_entityContext.SP_GetFileInTake835Count(sDate).FirstOrDefault());
            int SubCount = _entityContext.IntakeClaimData835.Where(o => o.FileDate.Contains(sDate)).ToList().Count();
            int AccCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Accepted" && x.FileDate.Contains(sDate)).ToList().Count();
            int NotAccCount = SubCount - AccCount;
            int PenCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Pending").ToList().Count();
            int RejCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Errors" && x.FileDate.Contains(sDate)).ToList().Count();
            int FailedFileCount = _entityContext.FileInTake835.Where(x => x.ExtraField2 == "File Error" && x.FileDate.Contains(sDate)).ToList().Count();

            Dashboard835ViewModel model = new Dashboard835ViewModel();
            model.FileDate = sDate;
            model.FileCount = totalFile;
            model.SubmittedClaimCount = SubCount;
            model.AcceptedClaimCount = AccCount;
            model.NotAcceptedClaimCount = NotAccCount;
            model.PendingClaimCount = PenCount;
            model.RejectedClaimCount = RejCount;
            //model.EDIFailLoadCount = NotAccCount - PenCount - RejCount;
            model.EDIFailLoadCount = FailedFileCount;

            return PartialView("_Dashboard835", model);
        }
        public ActionResult DashboardChild(string sDate)
        {
            //sDate = "20190123";
            int totalFile = Convert.ToInt32(_entityContext.SP_GetFileInTake835Count(sDate).FirstOrDefault());
            int SubCount = _entityContext.IntakeClaimData835.ToList().Count();
            int AccCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Accepted").ToList().Count();
            int NotAccCount = SubCount - AccCount;
            int PenCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Pending").ToList().Count();
            int RejCount = _entityContext.IntakeClaimData835.Where(x => x.ClaimStatus == "Errors").ToList().Count();
            int FailedFileCount = _entityContext.FileInTake835.Where(x => x.ExtraField2 == "File Error").ToList().Count();

            Dashboard835ViewModel model = new Dashboard835ViewModel();
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

            return PartialView("_Details", intakeClaimData);
        }
        [HttpPost]
        public JsonResult UpdateClaimStatus(UpdateClaimStatusViewModel obj)
        {
            Int32 ClaimStatusValue = Convert.ToInt32(obj.ClaimStatus);
            if (ClaimStatusValue != 0)
            {
                var Claim_Status = _entityContext.Claim_Status.Where(x => x.Claim_Status_id == ClaimStatusValue).FirstOrDefault();
                obj.ClaimStatus = Claim_Status.Claim_Status_desc;
                obj.ClaimStatus = "Accepted";
            }
            else
            {
                obj.ClaimStatus = "";
            }
            Int32 ICDCodeValue = Convert.ToInt32(obj.ICDCode);
            if (ICDCodeValue != 0)
            {
                var ICDCode = _entityContext.Claims_ICD_CODE.Where(x => x.SeqId == ICDCodeValue).FirstOrDefault();
                obj.ICDCode = ICDCode.ICD_CODE;
            }
            else
            {
                obj.ICDCode = "";
            }



            if (_entityContext != null)
            {
                var data = _entityContext.IntakeClaimData835.Where(x => x.SeqID == obj.ClaimDataSeqID).FirstOrDefault();
                if (data != null)
                {
                    data.ClaimStatus = string.IsNullOrEmpty(obj.ClaimStatus) ? data.ClaimStatus : obj.ClaimStatus;
                    data.HI01 = string.IsNullOrEmpty(obj.ICDCode) ? data.HI01 : obj.ICDCode;
                    data.ClaimExtNmbr = string.IsNullOrEmpty(obj.ClaimExtNmbr) ? string.Empty : obj.ClaimExtNmbr;
                    data.ClaimStatus = "Accepted";
                    data.ClaimLevelErrors = string.Empty;
                    _entityContext.SaveChanges();

                    if (data.FileID != null && !string.IsNullOrEmpty(data.FileID))
                    {
                        var fileData = _entityContext.FileInTake835.Where(x => x.FileID == data.FileID).FirstOrDefault();
                        if (fileData != null)
                        {
                            fileData.ExtraField2 = "Verified";
                            _entityContext.SaveChanges();
                        }
                        else
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }

                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details835()
        {
            return Claimspayment835Details("", "total");
        }

        public ActionResult Claimspayment835Details(string sSelectedDate, string sMenu)
        {
            using (HiPaaS_website_offshoreEntities entites = new HiPaaS_website_offshoreEntities())
            {
                ViewBag.FileCount = Convert.ToInt32(_entityContext.SP_GetFileInTake835Count(sSelectedDate).FirstOrDefault());
                List<FileClaimDetails835> model = new List<FileClaimDetails835>();
                if (!string.IsNullOrEmpty(sSelectedDate))
                {
                    foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileDate.Contains(sSelectedDate)).OrderByDescending(p => p.FileDate))
                    {
                        model.Add(new FileClaimDetails835
                        {
                            FileStatus = fileInTake.ExtraField2,
                            FileData = BindFileIntakeData(fileInTake),
                            IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
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
                        data = _entityContext.FileInTake835.Where(o => o.ExtraField2 == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "SC" || sMenu == "total")
                    {
                        strClaimStatus = "";
                        data = _entityContext.IntakeClaimData835.OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "AC")
                    {
                        strClaimStatus = "Accepted";
                        data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }
                    else if (sMenu == "EC")
                    {
                        strClaimStatus = "Errors";
                        data = _entityContext.IntakeClaimData835.Where(o => o.ClaimStatus == strClaimStatus).OrderByDescending(p => p.FileDate).Select(o => o.FileID).Distinct().ToList();
                    }

                    foreach (var FileID in data)
                    {
                        if (strClaimStatus != "")
                        {
                            foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails835
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID && o.ClaimStatus == strClaimStatus).Take(10).ToList(),
                                });
                            }
                        }
                        else
                        {
                            foreach (FileInTake835 fileInTake in _entityContext.FileInTake835.Where(x => x.FileID == FileID))
                            {
                                model.Add(new FileClaimDetails835
                                {
                                    FileStatus = fileInTake.ExtraField2,
                                    FileData = BindFileIntakeData(fileInTake),
                                    IntakeClaimDataList = _entityContext.IntakeClaimData835.Where(o => o.FileID == fileInTake.FileID).Take(10).ToList(),
                                });
                            }
                        }

                    }

                }

                return View("Claimspayment835Details", model);
            }
        }
        private FileIntakeListViewModel BindFileIntakeData(FileInTake835 fileIntakeData)
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
        private IntakeClaimDetailsNewViewModel GetIntakeClaimDetailsNew(int SeqID)
        {
            //var ClaimStatusList = GetClaimStatusList();
            var cliamData = _entityContext.IntakeClaimData835.Where(x => x.SeqID == SeqID).FirstOrDefault();
            var fileIntakeData = _entityContext.FileInTake835.Where(x => x.FileID == cliamData.FileID).FirstOrDefault();
            //new code
            var data = _entityContext.IntakeClaimLineData835.Where(x => x.FileID == cliamData.FileID && x.ClaimID == cliamData.ClaimID).ToList()
                .Select(claimLineData => new IntakeClaimLineDataListViewModel
                {
                    ClaimLineDataSeqID = claimLineData.SeqID,
                    //ServiceDate = claimLineData.ExtraField1 != null ? DateTime.ParseExact(claimLineData.ExtraField1, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyy") : String.Empty,
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
                ICDCode = cliamData.HI01 == null ? "" : cliamData.HI01,
                Field3 = cliamData.Field3 != null ? cliamData.Field3 : "0.00",
                Field4 = cliamData.Field4 != null ? cliamData.Field4 : "0.00",
            };

            intakeClaimData.FileData = BindFileIntakeData(fileIntakeData);
            intakeClaimData.IntakeClaimLineData = data;
            return intakeClaimData;
        }
    }
}