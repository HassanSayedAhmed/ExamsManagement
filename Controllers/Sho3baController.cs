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
    public class Sho3baController : Controller
    {
        // GET: Areas
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if (Request.IsAjaxRequest())
            {
                    var data =
                       from eeduppx0 in db.eeduppx0
                       join eeduecert in db.eeduecerts on eeduppx0.cert_id equals eeduecert.id into certJoin
                       from cert in certJoin.DefaultIfEmpty()
                       select new { id = eeduppx0.id, eedupp0 = eeduppx0.eedupp0, cert_id = cert.Master };
                    ViewBag.cert = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();

                    data = data.OrderByDescending(o => o.id);

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
        public ActionResult saveSho3ba(eeduppx0 sho3ba)
        {
            if (sho3ba.id == 0)
                db.eeduppx0.Add(sho3ba);
            else
            {
                eeduppx0 oldSho3ba = db.eeduppx0.Find(sho3ba.id);
                oldSho3ba.eedupp0 = sho3ba.eedupp0;
                oldSho3ba.cert_id = sho3ba.cert_id;
                db.Entry(oldSho3ba).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data="Done", sho3ba = sho3ba };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageSho3ba(int id)
        {
            eeduppx0 info = new eeduppx0();
            if (id != 0)
                info = db.eeduppx0.Find(id);

            ViewBag.cert = db.eeduecerts.Select(c =>new { cert_id = c.id, cert_name= c.Master }).ToList();

            return PartialView("_manageSho3ba", info);
        }

        public ActionResult deleteSho3ba(int id)
        {
            eeduppx0 info = new eeduppx0();
            info = db.eeduppx0.Find(id);
            db.eeduppx0.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}