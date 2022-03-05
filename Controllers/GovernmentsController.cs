using ExamsManagement.Auth;
using ExamsManagement.dataTable;
using ExamsManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExamsManagement.Controllers
{
    [CustomAuthenticationFilter]
    public class GovernmentsController : Controller
    {
        // GET: Governments
        ministryEducationEntities db = new ministryEducationEntities();
        // GET: Users
        public ActionResult Index(DatatableParam param)
        {
            if(Request.IsAjaxRequest())
            {
                var data = db.egovs.Select( g => new { g.Id, g.egovname }).OrderByDescending(o => o.Id);

                var displayResult = data.Skip(param.iDisplayStart)
                    .Take(10).ToList();
                var totalRecords = data.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = displayResult
                }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        [HttpPost]
        public ActionResult saveGovernment(egov gov)
        {
            if (gov.Id == 0)
                db.egovs.Add(gov);
            else
            {
                egov oldGov = db.egovs.Find(gov.Id);
                oldGov.egovname = gov.egovname;

                db.Entry(oldGov).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageGovernments(int Id)
        {
            egov info = new egov();
            if(Id!=0)
            info = db.egovs.Find(Id);

            return PartialView("_editGovernments", info);
        }

        public ActionResult deleteGovernments(int Id)
        {
            egov info = new egov();
            info = db.egovs.Find(Id);
            db.egovs.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
