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
    public class CoursesController : Controller
    {
        ministryEducationEntities db = new ministryEducationEntities();
        // GET: Users
        public ActionResult Index(DatatableParam param, int? id = 0, int? seatNo = 0)
        {
            var eexmaster = db.eexmmasters.Find(id);
            if(eexmaster != null)
            {
                var eeduppx0e = db.eeduppx0.Where(s => s.eedupp0 == eexmaster.eeduppx0).Select(s => new { id = s.id }).FirstOrDefault().id;
                var eyear = db.edducyears.Where(s => s.iyear == eexmaster.eyear).Select(y => new { id = y.id }).FirstOrDefault().id;
                var eedulictype = db.eeduecerts.Where(s => s.Master == eexmaster.eedulictype).Select(s => new { id = s.id }).FirstOrDefault().id;
                ViewBag.fileName = eexmaster.edocref;

                ViewBag.eexmaster_id = id;
                ViewBag.seatNo = seatNo;
             
                ViewBag.eeduppx0 = eeduppx0e;
                ViewBag.eyear = eyear;
                ViewBag.eedulictype = eedulictype;
            }
         
      
            if (Request.IsAjaxRequest())
            {
                
                var data =
                       from courses in db.courses
                       join eeduecert in db.eeduecerts on courses.shada_id equals eeduecert.id into certJoin
                       join edducyears in db.edducyears on courses.year_id equals edducyears.id into yearJoin
                       join eeduppx0 in db.eeduppx0 on courses.sho3ba_id equals eeduppx0.id into sho3baJoin
                       from cert in certJoin.DefaultIfEmpty()
                       from year in yearJoin.DefaultIfEmpty()
                       from sho3ba in sho3baJoin.DefaultIfEmpty()
                       select new { id = courses.id, is_main = courses.is_main, cert = cert.Master, year= year.iyear, sho3ba = sho3ba.eedupp0, course_name = courses.name, min_grade= courses.min_grade, max_grade = courses.max_grade};
                if (id != 0)
                {
                
                    data = data.Where(e => e.year == eexmaster.eyear&&e.sho3ba==eexmaster.eeduppx0&&e.cert==eexmaster.eedulictype);
                }   
                
                data = data.OrderByDescending(o => o.id);
                var displayResult = data.Skip(param.iDisplayStart)
                    .Take(10).ToList();
                var totalRecords = data.Count();
           
                ViewBag.year = db.edducyears.Select(c => new { year_id = c.id, year = c.iyear }).ToList();
                ViewBag.cert = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();
                ViewBag.sho3ba = db.eeduppx0.Select(c => new { sho3ba_id = c.id, sho3ba_name = c.eedupp0 }).ToList();

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
        public ActionResult saveCourse(cours course)
        {
            if(course.id == 0)
                db.courses.Add(course);
            else
            {
                cours oldCourse = db.courses.Find(course.id);
                oldCourse.year_id = course.year_id;
                oldCourse.shada_id = course.shada_id;
                oldCourse.sho3ba_id = course.sho3ba_id;
                oldCourse.name = course.name;
                oldCourse.min_grade = course.min_grade;
                oldCourse.max_grade = course.max_grade;
                oldCourse.is_main = course.is_main;
                db.Entry(oldCourse).State=System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult saveCourseWithGrade(cours course/*, float grade*/)
        {
            db.courses.Add(course);
            //grade grade1 = new grade();

            //db.grades.Add();

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult manageCourseWithGrades(int? id = 0, int? eexmasterId = 0)
        {
            cours info = new cours();
            //if (eexmasterId == 0 && id != 0)
            //    info = db.courses.Find(id);
            //else
            if (id == 0 && eexmasterId != 0)
            {
                eexmmaster eexmmaster = db.eexmmasters.Find(eexmasterId);
                var eyear = db.edducyears.Where(s => s.iyear == eexmmaster.eyear).Select(y => new { id = y.id }).FirstOrDefault().id;
                var eedulictype = db.eeduecerts.Where(s => s.Master == eexmmaster.eedulictype).Select(s => new { id = s.id }).FirstOrDefault().id;
                var eeduppx0e = db.eeduppx0.Where(s => s.eedupp0 == eexmmaster.eeduppx0 && s.cert_id == eedulictype).Select(s => new { id = s.id }).FirstOrDefault().id;

                info.year_id = eyear;
                info.sho3ba_id = eeduppx0e;
                info.shada_id = eedulictype;

            }

            ViewBag.year = db.edducyears.Select(c => new { year_id = c.id, year = c.iyear }).ToList();
            ViewBag.cert = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();
            ViewBag.sho3ba = db.eeduppx0.Select(c => new { sho3ba_id = c.id, sho3ba_name = c.eedupp0 }).ToList();

            return PartialView("_manageCourseWithGrades", info);
        }

        public ActionResult manageCourse(int? id=0,int? eexmasterId=0)
        {
            cours info = new cours();
            if(eexmasterId== 0 && id != 0)
                info = db.courses.Find(id);
            else if(id==0 && eexmasterId != 0)
            {
                eexmmaster eexmmaster = db.eexmmasters.Find(eexmasterId);
                var eeduppx0e = db.eeduppx0.Where(s => s.eedupp0 == eexmmaster.eeduppx0).Select(s => new { id = s.id }).FirstOrDefault().id;
                var eyear = db.edducyears.Where(s => s.iyear == eexmmaster.eyear).Select(y => new { id = y.id }).FirstOrDefault().id;
                var eedulictype = db.eeduecerts.Where(s => s.Master == eexmmaster.eedulictype).Select(s => new { id = s.id }).FirstOrDefault().id;
                info.year_id = eyear;
                info.sho3ba_id = eeduppx0e;
                info.shada_id = eedulictype;
            }

            ViewBag.year = db.edducyears.Select(c => new { year_id = c.id, year = c.iyear }).ToList();
            ViewBag.cert = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();
            ViewBag.sho3ba = db.eeduppx0.Select(c => new { sho3ba_id = c.id, sho3ba_name = c.eedupp0 }).ToList();

            return PartialView("_manageCourse", info);
        }
        
        public ActionResult deleteCourse(int id)
        {
            cours info = new cours();
            info = db.courses.Find(id);
            db.courses.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}