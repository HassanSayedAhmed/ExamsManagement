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
    public class CertificatesController : Controller
    {
        // GET: Certificates
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Index(DatatableParam param)
        {
            if (Request.IsAjaxRequest())
            {
                var data = db.eeduecerts.Select(s => new { s.id, s.Master }).OrderByDescending(o => o.id);

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
        public ActionResult saveCertificate(eeduecert certificate)
        {
            if (certificate.id == 0)
                db.eeduecerts.Add(certificate);
            else
            {
                eeduecert oldcertification = db.eeduecerts.Find(certificate.id);
                oldcertification.Master = certificate.Master;

                db.Entry(oldcertification).State = System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult manageCertificate(int id)
        {
            eeduecert info = new eeduecert();
            if (id != 0)
                info = db.eeduecerts.Find(id);

            return PartialView("_manageCertificate", info);
        }

        public ActionResult deleteCertificate(int id)
        {
            eeduecert info = new eeduecert();
            info = db.eeduecerts.Find(id);
            db.eeduecerts.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}