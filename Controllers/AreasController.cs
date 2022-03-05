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
    public class AreasController : Controller
    {
        // GET: Areas
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if(Request.IsAjaxRequest())
            {
                var data = db.eedusections.Select(a => new { a.id, a.eedusection1 } ).OrderByDescending(o => o.id);

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
        public ActionResult saveArea(eedusection section)
        {
            if (section.id == 0)
                db.eedusections.Add(section);
            else
            {
                eedusection oldSection = db.eedusections.Find(section.id);
                oldSection.eedusection1 = section.eedusection1;

                db.Entry(oldSection).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult showAreas(DatatableParam param)
        {
            var data = db.eedusections.OrderByDescending(o => o.id);

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

        public ActionResult manageArea(int id)
        {
            eedusection info = new eedusection();
            if (id != 0)
                info = db.eedusections.Find(id);

            return PartialView("_manageArea", info);
        }

        public ActionResult deleteArea(int id)
        {
            eedusection info = new eedusection();
            info = db.eedusections.Find(id);
            db.eedusections.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}