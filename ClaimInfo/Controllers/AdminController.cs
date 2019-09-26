using ClaimInfo.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClaimInfo.Controllers
{
    public class AdminController : Controller
    {
        HiPaaS_website_offshoreEntities _entityContext;
        public AdminController()
        {
            _entityContext = new HiPaaS_website_offshoreEntities();
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ICDCode()
        {
            List<SP_GetClaims_ICD_CODE_Result> data = new List<SP_GetClaims_ICD_CODE_Result>();
            data = _entityContext.SP_GetClaims_ICD_CODE().ToList();
            return View(data);
        }
        public ActionResult TredingPartner()
        {
            return View();
        }
        public ActionResult Comapnion()
        {
            List<SP_GetCompanion_Guide_Result> model = new List<SP_GetCompanion_Guide_Result>();
            model = _entityContext.SP_GetCompanion_Guide().ToList();
            return View(model);
        }
        public ActionResult Comapnion270()
        {
            List<SP_GetCompanion_Guide_270_Result> model = new List<SP_GetCompanion_Guide_270_Result>();
            model = _entityContext.SP_GetCompanion_Guide_270().ToList();
            return View(model);
        }
        public ActionResult ListTredingPartner()
        {
            return View();
        }

        public ActionResult AddNewClient()
        {
            List<SP_GetCompanion_Guide_Result> model = new List<SP_GetCompanion_Guide_Result>();
            model = _entityContext.SP_GetCompanion_Guide().ToList();
            return View(model);
        }

    }
}