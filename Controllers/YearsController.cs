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
    public class YearsController : Controller
    {
        // GET: Years
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if (Request.IsAjaxRequest())
            {
                var data = db.edducyears.Select(y => new { y.id, y.iyear }).OrderByDescending(o => o.iyear);

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
        public ActionResult saveYear(edducyear year)
        {
            if (year.id == 0)
                db.edducyears.Add(year);
            else
            {
                edducyear oldYear = db.edducyears.Find(year.id);
                oldYear.iyear = year.iyear;

                db.Entry(oldYear).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageYear(int id)
        {
            edducyear info = new edducyear();
            if (id != 0)
                info = db.edducyears.Find(id);

            return PartialView("_manageYear", info);
        }

        public ActionResult deleteYear(int id)
        {
            edducyear info = new edducyear();
            info = db.edducyears.Find(id);
            db.edducyears.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}