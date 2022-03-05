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
    public class UsersController : Controller
    {
        ministryEducationEntities db = new ministryEducationEntities();
        // GET: Users
        public ActionResult Index(DatatableParam param)
        {
            if(Request.IsAjaxRequest())
            {
                var data = db.UserSecuirtyTables.Select(s => new { s.id, s.IUser_Name, s.IUser_Enabled, s.IUser_Login, s.IUser_Password, s.IUser_Type }).OrderByDescending(o => o.id);

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
        public ActionResult saveUser(UserSecuirtyTable user)
        {
            user.IUser_Login = user.IUser_Name;
            if(user.id == 0)
                db.UserSecuirtyTables.Add(user);
            else
            {
                UserSecuirtyTable oldUser = db.UserSecuirtyTables.Find(user.id);
                oldUser.IUser_Name = user.IUser_Name;
                oldUser.IUser_Login = user.IUser_Name;
                oldUser.IUser_Password = user.IUser_Password;
                oldUser.IUser_Enabled = user.IUser_Enabled;
                oldUser.IUser_Type = user.IUser_Type;
                if (user.IUser_Enabled == null)
                    user.IUser_Enabled = false;
                db.Entry(oldUser).State=System.Data.Entity.EntityState.Modified;
            }

            db.SaveChanges();

            var JsonResult = new { data = "Done" };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult manageUser(int Id)
        {
            UserSecuirtyTable info = new UserSecuirtyTable();
            if(Id>0)
                info = db.UserSecuirtyTables.Find(Id);
        
            return PartialView("_editUser", info);
        }
        public ActionResult deleteUser(int Id)
        {
            UserSecuirtyTable info = new UserSecuirtyTable();
            info = db.UserSecuirtyTables.Find(Id);
            db.UserSecuirtyTables.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                data = "Done"
            }, JsonRequestBehavior.AllowGet);
        }

    }
}