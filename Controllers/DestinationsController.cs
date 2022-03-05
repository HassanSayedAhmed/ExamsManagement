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
    public class DestinationsController : Controller
    {
        // GET: Certificates
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if (Request.IsAjaxRequest())
            {
                var data = db.destinations.Select(s => new { s.id, s.destination1 }).OrderByDescending(o => o.id);

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
        public ActionResult saveDestination(destination destination)
        {
            if (destination.id == 0)
                db.destinations.Add(destination);
            else
            {
                destination olddestination = db.destinations.Find(destination.id);
                olddestination.destination1 = destination.destination1;

                db.Entry(olddestination).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();
            var destinations = db.destinations.Select(s => new { s.id, s.destination1 }).ToList();

            var JsonResult = new { icon = "success", title = "تم الحفظ بنجاح...", text = "يمكنك الأن اختيار هذه الجهه.", destinationID = destination.id, destinationNameResult = destination.destination1, allDestinations = destinations };

            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageDestination(int id)
        {
            destination info = new destination();
            if (id != 0)
                info = db.destinations.Find(id);

            return PartialView("_manageDestination", info);
        }

        public ActionResult deleteDestination(int id)
        {
            destination info = new destination();
            info = db.destinations.Find(id);
            db.destinations.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}