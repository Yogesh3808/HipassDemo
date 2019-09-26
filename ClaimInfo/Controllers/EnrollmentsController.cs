using ClaimInfo.DataModel;
using ClaimInfo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Controllers
{
    public class EnrollmentsController : Controller
    {
        HiPaaS_website_offshoreEntities _entities = new HiPaaS_website_offshoreEntities();
        // GET: Enrollments
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