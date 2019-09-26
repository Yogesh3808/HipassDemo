using ClaimInfo.DataModel;
using ClaimInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Controllers
{
    public class Daily834InboundController : Controller
    {
        HiPaaS_website_offshoreEntities _entityContext;
        public Daily834InboundController()
        {
            _entityContext = new HiPaaS_website_offshoreEntities();
        }
        public ActionResult Index()
        {
            Dailt834Model ObjDailt834Model = new Dailt834Model();
            int TotalFile = _entityContext.MemberInfo_M1.ToList().Count();
            var List = _entityContext.SP_834DailyDashboardCount().ToList();
            ObjDailt834Model.TotalFile =List[0].total_file.ToString();
            ObjDailt834Model.TotalEnrollment = List[0].Total_enrollment.ToString();
            ObjDailt834Model.Additional = List[0].addition.ToString();
            ObjDailt834Model.Change = List[0].Change.ToString();
            ObjDailt834Model.Term = List[0].term.ToString();
            ObjDailt834Model.ErrorCount = List[0].Error.ToString();
            ObjDailt834Model.objVisitDetailsList = _entityContext.SP_Daily834headerData().ToList();
           
            return View(ObjDailt834Model);
            
        }

        public ActionResult FileEnrollmentDetails1(string sMenu)
        {
            Dailt834Model ObjDailt834Model = new Dailt834Model();
            ObjDailt834Model.objFileEnrollmentDetails = _entityContext.SP_834Filecountwisedetails(sMenu).ToList();
           for (int i = 0; i < ObjDailt834Model.objFileEnrollmentDetails.Count; i++)
            {
                ObjDailt834Model.objFile834Details = _entityContext.SP_834FileDetails(sMenu).ToList();
            }           

            return View("FileEnrollmentDetails", ObjDailt834Model);

        }

        public ActionResult FileEnrollmentDetails()
        {
            Dailt834Model ObjDailt834Model = new Dailt834Model();
            ObjDailt834Model.objFileEnrollmentDetails = _entityContext.SP_834Filecountwisedetails("Total").ToList();
            for (int i = 0; i < ObjDailt834Model.objFileEnrollmentDetails.Count; i++)
            {
                ObjDailt834Model.objFile834Details = _entityContext.SP_834FileDetails("Total").ToList();
            }

            return View("FileEnrollmentDetails", ObjDailt834Model);
        }
        public ActionResult Details(int SeqID, string SubscriberNo)
        {
            Daily834LineData ObjDaily834LineData = new Daily834LineData();
            var data = _entityContext.SP_834FileHeaderDetails(SeqID.ToString(), SubscriberNo, 1).ToList();
            ObjDaily834LineData.Obj834LineData = _entityContext.SP_834FileHeaderDetails(SeqID.ToString(), SubscriberNo, 2).ToList();
            if (data != null && data.Count >0)
            {
                ObjDaily834LineData.FileName = data[0].FileName;
                ObjDaily834LineData.sender = data[0].sender;
                ObjDaily834LineData.Receiver = data[0].receiver;
                ObjDaily834LineData.MemberFName = data[0].MemberFName;
                ObjDaily834LineData.MemberLName = data[0].MemberLName;

                ObjDaily834LineData.Telephone = data[0].Telephone;
                ObjDaily834LineData.StreetAddress = data[0].StreetAddress;
                ObjDaily834LineData.City = data[0].City;


                ObjDaily834LineData.State = data[0].State;
                ObjDaily834LineData.PostalCode = data[0].PostalCode;
                ObjDaily834LineData.Enrollment_type = data[0].Enrollment_type;

                ObjDaily834LineData.dob = data[0].dob.ToString();
                ObjDaily834LineData.gender = data[0].gender;

            }
            else { Daily834LineData a = new Daily834LineData(); }
   
            return PartialView("Details", ObjDaily834LineData);
        }
    }
}