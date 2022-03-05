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
    public class ControlsController : Controller
    {
        // GET: Controls
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if (Request.IsAjaxRequest())
            {
                var data = db.eeducons.Select(c => new { c.id, c.educon } ).OrderByDescending(o => o.id);

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
        public ActionResult saveControl(eeducon control)
        {
            if (control.id == 0)
                db.eeducons.Add(control);
            else
            {
                eeducon oldControl = db.eeducons.Find(control.id);
                oldControl.educon = control.educon;

                db.Entry(oldControl).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageControl(int id)
        {
            eeducon info = new eeducon();
            if (id != 0)
                info = db.eeducons.Find(id);

            return PartialView("_manageControl", info);
        }
     
        public ActionResult deleteControl(int id)
        {
            eeducon info = new eeducon();
            info = db.eeducons.Find(id);
            db.eeducons.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}