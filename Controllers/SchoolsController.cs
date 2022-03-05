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
    public class SchoolsController : Controller
    {
        // GET: Schools
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if (Request.IsAjaxRequest())
            {
                var data = db.eschools.Select(s => new { s.id, s.eschool1 }).OrderByDescending(o => o.id);

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
        public ActionResult saveSchool(eschool school)
        {
            if (school.id == 0)
                db.eschools.Add(school);
            else
            {
                eschool oldSchool = db.eschools.Find(school.id);
                oldSchool.eschool1 = school.eschool1;

                db.Entry(oldSchool).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageSchool(int id)
        {
            eschool info = new eschool();
            if (id != 0)
                info = db.eschools.Find(id);

            return PartialView("_manageSchool", info);
        }

        public ActionResult deleteSchool(int id)
        {
            eschool info = new eschool();
            info = db.eschools.Find(id);
            db.eschools.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}