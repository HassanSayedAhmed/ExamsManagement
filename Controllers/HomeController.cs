using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExamsManagement.Auth;
using ExamsManagement.dataTable;
using ExamsManagement.Models;
using ExamsManagement.ViewModels;
using Rotativa;
using System.Configuration;
using System.Globalization;

namespace ExamsManagement.Controllers
{
    [CustomAuthenticationFilter]
    public class HomeController : Controller
    {
        ministryEducationEntities db = new ministryEducationEntities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserSecuirtyTable userSecuirtyTable)
        {
            UserSecuirtyTable obj = db.UserSecuirtyTables.Where(e => e.IUser_Name == userSecuirtyTable.IUser_Name && e.IUser_Password == userSecuirtyTable.IUser_Password && e.IUser_Enabled == true).FirstOrDefault();
            if (obj != null)
            {
                Session["username"] = obj.IUser_Name;
                Session["id"] = obj.id;
                Session["userType"] = obj.IUser_Type;
               
                return RedirectToAction("Documents");
                

            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult searchFiles(string fileName, int? pageNumber)
        {
            var query = db.eexmmasters;
            List<eexmmaster> studentInfo;
            if (!String.IsNullOrWhiteSpace(fileName) && pageNumber != null)
            {
                studentInfo = db.eexmmasters.Where(s => s.edocref == fileName && (pageNumber >= s.estudentfrom && pageNumber <= s.estudentto)).ToList();
            }
            else
            {
                studentInfo = query.Take(10).ToList();
            }

            return View(studentInfo);
        }
        public ActionResult Counter()
        {
            string cs = ConfigurationManager.ConnectionStrings["ministryEducationEntitiesADO"].ConnectionString;
            SqlConnection sql = new SqlConnection(cs);
            sql.Open();
            //SqlCommand comm = new SqlCommand("SELECT count(distinct( edocref)) as docCount FROM eexmmaster  where edocref is not null", sql);
            //SqlDataReader reader = comm.ExecuteReader();
            //if (reader.Read())
            //{
            //    ViewBag.edocrefCount = reader["docCount"];
            //}

            SqlCommand comm = new SqlCommand("SELECT MAX(epage) as pages  FROM eexmmaster where edocref is not null and epage is not null group by edocref", sql);
            SqlDataReader reader = comm.ExecuteReader();
            List<int> pages = new List<int>();
            while (reader.Read())
            {
                pages.Add(Convert.ToInt32(reader["pages"]));
            }
            ViewBag.epagesCount = pages.Sum();
            reader.Close();
            sql.Close();
            return View();
        }

        public ActionResult Documents()
        {
            
            ViewBag.gov = db.egovs.Select(s => new { s.Id, s.egovname }).ToList();
            ViewBag.section = db.eedusections.Select(s => new { s.id, s.eedusection1 }).ToList();
            ViewBag.school = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();

            ViewBag.level = new List<SelectListItem>
            {
            new SelectListItem() {Text = " الأول", Value = " الأول"},
            new SelectListItem() {Text = "الثانى", Value = "الثانى"}
            };

            ViewBag.year = db.edducyears.Select(s => new { s.id, s.iyear }).ToList();
            ViewBag.sho3ba = db.eeduppx0.Select(s => new { s.id, s.eedupp0 }).ToList();
            ViewBag.cert = db.eeduecerts.Select(s => new { s.id, s.Master }).ToList();
            ViewBag.control = db.eeducons.Select(s => new { s.id, s.educon }).ToList();
            ViewBag.users = db.UserSecuirtyTables.Select(s => new { s.id, s.IUser_Name }).ToList();
            return View();
        }
        
        public ActionResult showSho3ba(string cert)
        {
           
            if (cert != null)
            {
                int shada_id = db.eeduecerts.Where(c => c.Master == cert).FirstOrDefault().id;
                if(shada_id > 0)
                {
                    var data = db.eeduppx0.Where(c => c.cert_id == shada_id).Select(s => new { s.eedupp0 }).ToList();
                    return Json(new { data = data }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { data = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getName(string id_no14)
        {
            string data = db.students.Where(c => c.id_no14 == id_no14).FirstOrDefault() != null ? db.students.Where(c => c.id_no14 == id_no14).FirstOrDefault().name : null;

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getYears(string cert)
        {
            var data = db.eexmmasters.Where(c => c.eedulictype == cert && c.eyear != null && c.eyear != "" && c.eyear.Length == 4 && c.eyear != "0000").Select(s => new DocumentViewModel { eyear = s.eyear.Trim() }).Distinct().OrderBy(c => c.eyear).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getSchools(string year, string cert)
        {
            var data = db.eexmmasters.Where(c => c.eyear == year && (cert == null || c.eedulictype == cert) && c.eschool != null && c.eschool != "").Select(s => new DocumentViewModel { eschool = s.eschool.Trim() }).Distinct().OrderBy(c => c.eschool).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult showSho3baInCourses(int cert)
        {
            var data = db.eeduppx0.Where(c => c.cert_id == cert).Select(s => new { s.id,s.eedupp0 }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult getDocuments(DatatableParam param)
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
            var gov = Request.Form.GetValues("columns[0][search][value]")[0];
            var section = Request.Form.GetValues("columns[1][search][value]")[0];
            var school = Request.Form.GetValues("columns[2][search][value]")[0];
            var level = Request.Form.GetValues("columns[3][search][value]")[0];
            var year = Request.Form.GetValues("columns[4][search][value]")[0];
            var sho3ba = Request.Form.GetValues("columns[5][search][value]")[0];
            var cert = Request.Form.GetValues("columns[6][search][value]")[0];
            var control = Request.Form.GetValues("columns[7][search][value]")[0];
            var seatNo = Request.Form.GetValues("columns[8][search][value]")[0];
            var id_no14 = Request.Form.GetValues("columns[9][search][value]")[0];

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            //IQueryable<eexmmaster> employees = db.eexmmasters;
            var employees = (from e in db.eexmmasters
                             //join s in db.students on e.id equals s.eexmaster_id into certJoin
                             //from std in certJoin.DefaultIfEmpty()
                             select new indexViewModel
                             {
                                 id = e.id,
                                 eschool = e.eschool,
                                 edocref = e.edocref,
                                 eegov = e.eegov,
                                 eyear = e.eyear,
                                 estatus = e.estatus,
                                 elevel = e.elevel,
                                 eedusection = e.eedusection,
                                 eeduppx0 = e.eeduppx0,
                                 eedulictype = e.eedulictype,
                                 epage = e.epage,
                                 eeCount = e.eeCount,
                                 estudentfrom = e.estudentfrom,
                                 estudentto = e.estudentto,
                                 //studentName = std.name,
                                 //id_no14 = std.id_no14,
                                 //seat_number = std.seat_number
                             });

            if (!string.IsNullOrEmpty(searchValue))
            {
                employees = employees.Where(x => x.eegov.ToLower().Contains(searchValue.ToLower())
                                              || x.eschool.ToLower().Contains(searchValue.ToLower())
                                              || x.estatus.ToLower().Contains(searchValue.ToLower())
                                              || x.eyear.ToString().Contains(searchValue.ToLower())
                                              || x.edocref.ToString().Contains(searchValue.ToLower()));
            }
            if (!string.IsNullOrEmpty(gov))
            {
                employees = employees.Where(x => x.eegov == gov);
            }
            if (!string.IsNullOrEmpty(section))
            {
                employees = employees.Where(x => x.eedusection == section);
            }
            if (!string.IsNullOrEmpty(school))
            {
                employees = employees.Where(x => x.eschool == school);
            }
            if (!string.IsNullOrEmpty(level))
            {
                employees = employees.Where(x => x.elevel == level);
            }
            if (!string.IsNullOrEmpty(year))
            {
                employees = employees.Where(x => x.eyear == year);
            }
            if (!string.IsNullOrEmpty(sho3ba))
            {
                employees = employees.Where(x => x.eeduppx0 == sho3ba);
            }
            if (!string.IsNullOrEmpty(cert))
            {
                employees = employees.Where(x => x.eedulictype == cert);
            }
            if (!string.IsNullOrEmpty(control))
            {
                employees = employees.Where(x => x.eeCount == control);
            }
            if (!string.IsNullOrEmpty(seatNo))
            {
                int seat = int.Parse(seatNo);
                employees = employees.Where(x => x.estudentfrom <= seat && x.estudentto >= seat);
            }
            //if (!string.IsNullOrEmpty(param.studentName))
            //{
            //    employees = employees.Where(x => x.studentName.Contains(param.studentName));
            //}
            if (!string.IsNullOrEmpty(id_no14))
            {
                student student = db.students.Where(s => s.id_no14 == id_no14).FirstOrDefault();
                if(student!=null)
                    employees = employees.Where(x => x.id == student.eexmaster_id);
                else
                    employees = employees.Where(x => x.id == -1);
            }
            var data = employees.OrderByDescending(r => r.id);
            var sortColumnIndex = Convert.ToInt32(HttpContext.Request.QueryString["iSortCol_0"]);
            var displayResult = data.Skip(skip)
                     .Take(pageSize).ToList();
            //List<DocumentViewModel> Documents = new List<DocumentViewModel>();
            //Documents = AutoMapper.Mapper.Map<List<indexViewModel>, List<DocumentViewModel>>(displayResult);
            var totalRecords = data.Count();
            return Json(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = displayResult

            }, JsonRequestBehavior.AllowGet);
          

        }


       public ActionResult uploadFiles(string fileName,int? id=0)
        {
            eexmmaster Document = new eexmmaster();
            if (id != 0)
            { 
                Document = db.eexmmasters.Find(id);
                ViewBag.fileName = Document.edocref;
                ViewBag.pageNo = Document.epage;
                ViewBag.isEdit = 1;
            }
            else if (!String.IsNullOrEmpty(fileName))
            {
                ViewBag.fileName = fileName;
                ViewBag.pageNo = 1;
                ViewBag.isEdit = 0;

            }

            //.fileName = Document.edocref;
            ViewBag.gov = db.egovs.Select(s => new { s.Id, s.egovname }).ToList();
            ViewBag.section = db.eedusections.Select(s => new { s.id, s.eedusection1 }).ToList();
            ViewBag.school = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();
            //ViewBag.Forcert sho3ba = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();

            ViewBag.level = new List<SelectListItem>
            {
            new SelectListItem() {Text = "الأول", Value = " الأول"},
            new SelectListItem() {Text = "الثانى", Value = "الثانى"}
            };
            ViewBag.status = new List<SelectListItem>
            {
            new SelectListItem() {Text = "ناجح", Value = "ناجح"},
            new SelectListItem() {Text = "راسب", Value = "راسب"}
            };
            ViewBag.year = db.edducyears.Select(s => new { s.id, s.iyear }).ToList();
            ViewBag.sho3ba = db.eeduppx0.Select(s => new { s.id, s.eedupp0 }).ToList();
            ViewBag.cert = db.eeduecerts.Select(s => new { s.id, s.Master }).ToList();
            ViewBag.control = db.eeducons.Select(s => new { s.id, s.educon }).ToList();
            
            return View(Document);
        }

        public ActionResult addStudentMissing(int? id = 0)
        {
            eexmmaster Document = new eexmmaster();
            if (id != 0)
            {
                Document = db.eexmmasters.Find(id);
                //ViewBag.fileName = Document.edocref;
                //ViewBag.pageNo = Document.epage;
                ViewBag.isEdit = 1;
            }
            else if (id > 0)
            {
                //ViewBag.fileName = fileName;
                //ViewBag.pageNo = 1;
                ViewBag.isEdit = 0;

            }

            //.fileName = Document.edocref;
            ViewBag.gov = db.egovs.Select(s => new { s.Id, s.egovname }).ToList();
            ViewBag.section = db.eedusections.Select(s => new { s.id, s.eedusection1 }).ToList();
            ViewBag.school = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();
            //ViewBag.Forcert sho3ba = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();

            ViewBag.level = new List<SelectListItem>
            {
            new SelectListItem() {Text = "الأول", Value = " الأول"},
            new SelectListItem() {Text = "الثانى", Value = "الثانى"}
            };
            ViewBag.status = new List<SelectListItem>
            {
            new SelectListItem() {Text = "ناجح", Value = "ناجح"},
            new SelectListItem() {Text = "راسب", Value = "راسب"}
            };
            ViewBag.year = db.edducyears.Select(s => new { s.id, s.iyear }).ToList();
            ViewBag.sho3ba = db.eeduppx0.Select(s => new { s.id, s.eedupp0 }).ToList();
            ViewBag.cert = db.eeduecerts.Select(s => new { s.id, s.Master }).ToList();
            ViewBag.control = db.eeducons.Select(s => new { s.id, s.educon }).ToList();

            return View(Document);
        }


        public ActionResult uploadFilesDataEntry(string fileName, int? id = 0)
        {
            eexmmaster Document = new eexmmaster();
            if (id != 0)
            {
                Document = db.eexmmasters.Find(id);
                ViewBag.fileName = Document.edocref;
                ViewBag.pageNo = Document.epage;
                ViewBag.isEdit = 1;
            }
            else if (!String.IsNullOrEmpty(fileName))
            {
                ViewBag.fileName = fileName;
                ViewBag.pageNo = 1;
                ViewBag.isEdit = 0;

            }

            //.fileName = Document.edocref;
            ViewBag.gov = db.egovs.Select(s => new { s.Id, s.egovname }).ToList();
            ViewBag.section = db.eedusections.Select(s => new { s.id, s.eedusection1 }).ToList();
            ViewBag.school = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();
            //ViewBag.Forcert sho3ba = db.eeduecerts.Select(c => new { cert_id = c.id, cert_name = c.Master }).ToList();

            ViewBag.level = new List<SelectListItem>
            {
            new SelectListItem() {Text = "الأول", Value = " الأول"},
            new SelectListItem() {Text = "الثانى", Value = "الثانى"}
            };
            ViewBag.status = new List<SelectListItem>
            {
            new SelectListItem() {Text = "ناجح", Value = "ناجح"},
            new SelectListItem() {Text = "راسب", Value = "راسب"}
            };
            ViewBag.year = db.edducyears.Select(s => new { s.id, s.iyear }).ToList();
            ViewBag.sho3ba = db.eeduppx0.Select(s => new { s.id, s.eedupp0 }).ToList();
            ViewBag.cert = db.eeduecerts.Select(s => new { s.id, s.Master }).ToList();
            ViewBag.control = db.eeducons.Select(s => new { s.id, s.educon }).ToList();

            return View(Document);
        }


        public JsonResult calcPages(int UserID, DateTime IUserDateTimeFrom, DateTime IUserDateTimeTo)
        {
            
            string cs = ConfigurationManager.ConnectionStrings["ministryEducationEntitiesADO"].ConnectionString;
            SqlConnection sql = new SqlConnection(cs);
            sql.Open();
            //SqlCommand comm = new SqlCommand("SELECT count(distinct( edocref)) as docCount FROM eexmmaster  where edocref is not null", sql);
            //SqlDataReader reader = comm.ExecuteReader();
            //if (reader.Read())
            //{
            //    ViewBag.edocrefCount = reader["docCount"];
            //}
            var query = "SELECT epage as pages  FROM eexmmaster where edocref is not null and epage is not null";
            if (UserID > 0)
            {
                query += " and UserID = "+ UserID;
            }
            if (IUserDateTimeFrom != null)
            {
                //var date = IUserDateTime.ToString("dd/MM/yyyy").Replace('-', '/');
                //var date = IUserDateTime.Year + '-' + IUserDateTime.Month + '-' + IUserDateTime.Day;
                //var date =Convert.ToInt32( IUserDateTime.Year.ToString()+ IUserDateTime.Month.ToString() + IUserDateTime.Day.ToString());
                var date = IUserDateTimeFrom.Year.ToString() + '-' + IUserDateTimeFrom.Month.ToString() + '-' + IUserDateTimeFrom.Day.ToString();

                //query += "eexmmaster.IUserDateTime = '" + date + "'";
                query += " and CAST( eexmmaster.IUserDateTime AS DATE) >= '" + date + "'";
            }
            if (IUserDateTimeTo != null)
            {
                //var date = IUserDateTime.ToString("dd/MM/yyyy").Replace('-', '/');
                //var date = IUserDateTime.Year + '-' + IUserDateTime.Month + '-' + IUserDateTime.Day;
                //var date =Convert.ToInt32( IUserDateTime.Year.ToString()+ IUserDateTime.Month.ToString() + IUserDateTime.Day.ToString());
                var date = IUserDateTimeTo.Year.ToString() + '-' + IUserDateTimeTo.Month.ToString() + '-' + IUserDateTimeTo.Day.ToString();

                //query += "eexmmaster.IUserDateTime = '" + date + "'";
                query += " and CAST( eexmmaster.IUserDateTime AS DATE) <= '" + date + "'";
            }
            SqlCommand comm = new SqlCommand(query, sql);
            SqlDataReader reader = comm.ExecuteReader();
            List<int> pages = new List<int>();
            int counter = 0;
            while (reader.Read())
            {
                counter++;
                //pages.Add(Convert.ToInt32(reader["pages"]));
            }
            //ViewBag.epagesCount = pages.Sum();
            reader.Close();
            sql.Close();
            return Json(new
            {
                data = counter
            }, JsonRequestBehavior.AllowGet);
       }

        [HttpPost]
        public JsonResult saveDftarRow(eexmmaster eexmmaster)
        {
            bool refreshPage = false;
            if (eexmmaster.id==0)
            { 
                eexmmaster.erowref = "page" + DateTime.Now.ToString("ddMMyyyyhhmmssttt");
                eexmmaster.IUserDateTime = DateTime.Now;
                eexmmaster.UserID = Convert.ToInt32(Session["id"].ToString());
                
                eexmmaster.DocThere = true;
                db.eexmmasters.Add(eexmmaster);
                db.SaveChanges();
                return Json(new
                {
                    id = eexmmaster.id,
                    data = eexmmaster.edocref,
                    refreshPage = refreshPage
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                eexmmaster oldRow = db.eexmmasters.Where(s => s.id == eexmmaster.id).FirstOrDefault();
                eexmmaster.IUserDateTime = oldRow.IUserDateTime;
                eexmmaster.DocThere = oldRow.DocThere;
                eexmmaster.UserID = oldRow.UserID;
                eexmmaster.erowref = oldRow.erowref;
                if (oldRow != null)
                {
                    db.Entry(oldRow).State = System.Data.Entity.EntityState.Detached;
                }
                
                if (eexmmaster.edocref != oldRow.edocref)
                    refreshPage = true;
                db.Entry(eexmmaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json(new
                {
                    id = eexmmaster.id,
                    data = eexmmaster.edocref,
                    refreshPage= refreshPage
                }, JsonRequestBehavior.AllowGet);
            }
          
        }



        public JsonResult showDftar(DatatableParam param)
        {
            
              var data = db.eexmmasters.Where(e => (param.edocref == null) && (e.id == param.daftarRowID || param.daftarRowID == 0)).Select(e=>new pageViewModel
              { 
                  id=e.id,
                  estudentfrom=(int)e.estudentfrom,
                  estudentto=(int)e.estudentfrom,
                  epage=(int)e.epage,
                  eschool=e.eschool

              }).OrderByDescending(o => o.id);
          
            var displayResult = data.Skip(param.iDisplayStart)
                .Take(5).ToList();
            var totalRecords = data.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult showMissingStudents(DatatableParam param)
        {

            var data = db.eexmmasters.Where(e => (param.edocref == null) && (e.id == param.daftarRowID || param.daftarRowID == 0)).Select(e => new pageViewModel
            {
                id = e.id,
                estudentfrom = (int)e.estudentfrom,
                estudentto = (int)e.estudentfrom,
                epage = (int)e.epage,
                eschool = e.eschool

            }).OrderByDescending(o => o.id);

            var displayResult = data.Skip(param.iDisplayStart)
                .Take(5).ToList();
            var totalRecords = data.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult showDftarDataEntry(DatatableParam param)
        {

          
            List<pageViewModel> data = new List<pageViewModel>();
            if (param.daftarRowID != 0)
            {
                pageViewModel firstRaw = db.eexmmasters.Where(e => e.id == param.daftarRowID).Select(e=>new pageViewModel
                {
                    id = e.id,
                    estudentfrom = (int)e.estudentfrom,
                    estudentto = (int)e.estudentfrom,
                    epage = (int)e.epage,
                    eschool = e.eschool
                }).FirstOrDefault();
                data.Add(firstRaw);
                List<pageViewModel> queryData2 = db.eexmmasters.Where(e => e.id != param.daftarRowID && e.edocref == firstRaw.edocref).Select(e => new pageViewModel
                {
                    id = e.id,
                    estudentfrom = e.estudentfrom,
                    estudentto = e.estudentfrom,
                    epage = e.epage,
                    eschool = e.eschool
                }).ToList() ;
                data.AddRange(queryData2);
            }
            else
            { 
                data.AddRange(db.eexmmasters.Where(e => (e.edocref == param.edocref || param.edocref == null) && (e.id == param.daftarRowID || param.daftarRowID == 0)).Select(e => new pageViewModel
                {
                    id = e.id,
                    estudentfrom = (int)e.estudentfrom,
                    estudentto = (int)e.estudentfrom,
                    epage = (int)e.epage,
                    eschool = e.eschool
                }));
            }
            
            var displayResult = data.OrderByDescending(o => o.id).Skip(param.iDisplayStart)
                .Take(5).ToList();
            var totalRecords = data.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult displayPdf(string fileName, int page)
        {

            ViewBag.file = fileName;
            ViewBag.page = page.ToString();
            return View();
        }
        public ActionResult Details(int Id)
        {
            eexmmaster info = new eexmmaster();
            info = db.eexmmasters.Find(Id);
            ViewBag.Username = db.UserSecuirtyTables.Where(s => s.id == info.UserID).Select(s => s.IUser_Name).FirstOrDefault();
            return PartialView("_Details", info);
        }

        public ActionResult editPage(int Id)
        {
            eexmmaster info = new eexmmaster();
            info = db.eexmmasters.Find(Id);
            ViewBag.Username = db.UserSecuirtyTables.Where(s => s.id == info.UserID).Select(s => s.IUser_Name).FirstOrDefault();
            ViewBag.school = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();
            return PartialView("_editPage", info);
        }
        public ActionResult deletePage(int Id)
        {
            eexmmaster info = new eexmmaster();
            info = db.eexmmasters.Find(Id);
            var edocref = info.edocref;
            db.eexmmasters.Remove(info);
            db.SaveChanges();
            ViewBag.Username = db.UserSecuirtyTables.Where(s => s.id == info.UserID).Select(s => s.IUser_Name).FirstOrDefault();
            ViewBag.school = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();
            return Json(new
            {
                data = edocref
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult deleteRequest(int Id)
        {
            request info = new request();
            info = db.requests.Find(Id);
            var std = db.students.Find(info.student_id);
            var eexmasterID = std.eexmaster_id;
            var seatNumber = std.seat_number;
            db.requests.Remove(info);
            db.SaveChanges();
            return Json(new
            {
                eexmasterID = eexmasterID,
                seatNumber = seatNumber
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult saveDaftar(HttpPostedFileBase file)
        {
            string fileName = "";
            if (file != null)
            {
                fileName = "doc" + DateTime.Now.ToString("ddMMyyyyhhmmssttt");
                var InputFileName = Path.GetFileName(file.FileName);
                var ServerSavePath = Path.Combine(Server.MapPath("~/Scripts/web/") + fileName+".pdf");
                file.SaveAs(ServerSavePath);
            }
            return RedirectToAction("uploadFiles","Home",new { fileName=fileName});
        }

        [HttpPost]
        public ActionResult uploadAttachment()
        {
            string fileName = "";
            if (Request.Files.Count > 0)
            {
                string savedPath = "";
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    fileName = "attachment_" + DateTime.Now.ToString("ddMMyyyyhhmmssttt");
                    var InputFileName = Path.GetFileName(file.FileName);
                    var extention = InputFileName.Split('.')[InputFileName.Split('.').Length - 1];
                    var ServerSavePath = Path.Combine(Server.MapPath("~/Attachments/") + fileName + "." + extention);
                    file.SaveAs(ServerSavePath);
                    savedPath = "/Attachments/" + fileName + "." + extention;
                }
                return Json(new { data = savedPath }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { data = 0}, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Documents", "Home", new { fileName = fileName });
        }
        [HttpPost]
        public ActionResult saveAttachmentRow(attachment attachment)
        {
        
            student std = db.students.Where(s => s.eexmaster_id == attachment.eexmaster_id && s.seat_number == attachment.student_seat_number).FirstOrDefault();
            if (std != null)
            {
                std.attachment_file = attachment.fileData;
                std.attachment_description = attachment.description;
                db.Entry(std).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                student student = new student();
                student.seat_number = attachment.student_seat_number;
                student.eexmaster_id = attachment.eexmaster_id;
                student.attachment_description = attachment.description;
                student.attachment_file = attachment.fileData;

                db.students.Add(student);
            }
                
            db.SaveChanges();
            
            return Json(new { data = attachment.student_seat_number }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult showUploadedAttachment(int eexmaster_id, int seat_number)
        {
            var student = db.students.Where(s => s.seat_number == seat_number && s.eexmaster_id == eexmaster_id).FirstOrDefault();
            string data = "";
            string description = "";
            if (student != null)
            {
                data = student.attachment_file;
                description = student.attachment_description;
            }

            return Json(new { data = data , description = description }, JsonRequestBehavior.AllowGet);
        }
            
        [HttpPost]
        public JsonResult saveSchool(string schoolName)
        {
            int count = db.eschools.Where(s => s.eschool1 == schoolName).Count();
        
            if (count == 0)
            {
                eschool school = new eschool();
                school.eschool1 = schoolName;
                db.eschools.Add(school);
                db.SaveChanges();
                var schools = db.eschools.Select(s => new { s.id, s.eschool1 }).ToList();
              
                var JsonResult = new { icon = "success", title = "تم الحفظ بنجاح...", text = "يمكنك الأن اختيار هذه المدرسة." ,schoolNameResult = schoolName, allSchools= schools };
                return Json(JsonResult, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var JsonResult = new { icon = "error", title = "فشل الحفظ...", text = ".هذه المدرسة موجوده بالفعل"};
                return Json(JsonResult, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult checkIfExist(int? id, int? seatNo)
        {
            var eexmaster = db.eexmmasters.Find(id);
            if (eexmaster.eeduppx0 == null || eexmaster.eyear == null || eexmaster.eedulictype == null || eexmaster.eedusection == null || eexmaster.elevel == null) {
                return RedirectToAction("uploadFiles", new { id = id});
            }
            eeduecert eedulictype = db.eeduecerts.Where(s => s.Master == eexmaster.eedulictype).FirstOrDefault();
            if (eedulictype == null)
            {
                eedulictype = new eeduecert();
                eedulictype.Master = eexmaster.eedulictype;
                db.eeduecerts.Add(eedulictype);
                db.SaveChanges();
            }
            eeduppx0 eeduppx0 = db.eeduppx0.Where(s => s.eedupp0 == eexmaster.eeduppx0 && s.cert_id == eedulictype.id).FirstOrDefault();
            if (eeduppx0 == null)
            {
                eeduppx0 = new eeduppx0();
                eeduppx0.cert_id = eedulictype.id;
                eeduppx0.eedupp0 = eexmaster.eeduppx0;
                db.eeduppx0.Add(eeduppx0);
                db.SaveChanges();
            }
          
            var eyear = db.edducyears.Where(s => s.iyear == eexmaster.eyear).Select(y => new { id = y.id }).FirstOrDefault();

            var courses = db.courses.Where(c => c.sho3ba_id == eeduppx0.id && c.year_id == eyear.id && c.shada_id == eedulictype.id).FirstOrDefault();
            //if(courses == null)
            //{
            //    return RedirectToAction("Index", "Courses", new { id = id, seatNo = seatNo });
            //}

            return RedirectToAction("StudentGrade", new { id = id, seatNo = seatNo });
        }

        public ActionResult studentGrade(DatatableParam param,int id, int seatNo, int? stdID=0)
        {
            ViewBag.StudentId = seatNo;
            ViewBag.id = id;
            eexmmaster Document = new eexmmaster();
            if (id != 0)
            {
                Document = db.eexmmasters.Find(id);
                ViewBag.fileName = Document.edocref;
                ViewBag.pageNo = Document.epage;
            }
            
            var studentGrades  = new grade();
            var studentExist = db.students.Where(g => g.seat_number == seatNo && g.eexmaster_id == id).FirstOrDefault();
            if(stdID != 0)
              studentGrades = db.grades.Where(g => g.student_id == stdID).FirstOrDefault();
            var eexmaster = db.eexmmasters.Find(id);

            var eyear = db.edducyears.Where(s => s.iyear == eexmaster.eyear).Select(y => new { id = y.id }).FirstOrDefault();
            var eedulictype = db.eeduecerts.Where(s => s.Master == eexmaster.eedulictype).Select(s => new { id = s.id }).FirstOrDefault();
            var eeduppx0 = db.eeduppx0.Where(s => s.eedupp0 == eexmaster.eeduppx0 && s.cert_id == eedulictype.id).Select(s => new { id = s.id }).FirstOrDefault();

            var courses = db.courses.Where(c => c.sho3ba_id == eeduppx0.id && c.year_id == eyear.id && c.shada_id == eedulictype.id).ToList();
            ViewBag.hasCourses = 1;
            if (courses.Count == 0)
            {
                ViewBag.hasCourses = 0;
            }
            else { 
                if (stdID == 0 && studentExist == null)
                {
                    var student = new student();
                    student.seat_number = seatNo;
                    student.eexmaster_id = id;
                    db.students.Add(student);
                    db.SaveChanges();
                    ViewBag.student_id = student.id;
                    foreach (cours cours in courses)
                    {
                        var grade = new grade();
                        grade.course_id = cours.id;
                        grade.student_id = student.id;
                        grade.grade1 = 0;
                        db.grades.Add(grade);
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (cours cours in courses)
                    {
                        var std = db.students.Where(s => s.eexmaster_id == id && s.seat_number == seatNo).FirstOrDefault();
                        var grade = db.grades.Where(c => c.course_id == cours.id && c.student_id == studentExist.id).FirstOrDefault();
                        if(grade == null)
                        {
                            var std_grade = new grade();
                            std_grade.course_id = cours.id;
                            std_grade.student_id = std.id;
                            std_grade.grade1 = 0;
                            db.grades.Add(std_grade);
                        }
                    }
                    db.SaveChanges();
                }
            }
            if (Request.IsAjaxRequest())
            { 
                var data = (from g in db.grades
                              join c in db.courses on g.course_id equals c.id
                              join s in db.students on g.student_id equals s.id
                              select new
                              {
                                  g.id,
                                  g.student_id,
                                  s.seat_number,
                                  s.eexmaster_id,
                                  c.max_grade,
                                  c.min_grade,
                                  c.name,
                                  g.grade1,
                                  c.is_main
                              }).Where(s => s.seat_number == seatNo && s.eexmaster_id == id).OrderByDescending(r => r.is_main);
                var displayResult = data.Skip(param.iDisplayStart)
                    .Take(10).ToList();
                var totalRecords = data.Count();

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = data
                }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public ActionResult manageGrades(int id)
        {
            var info = db.grades.Find(id);
            ViewBag.courseName = db.courses.Where(c => c.id == info.course_id).Select(c => c.name).FirstOrDefault();
            return PartialView("_manageGrades", info);
        }
        [HttpPost]
        public ActionResult saveGrade(grade grade)
        {
            var info = db.grades.Find(grade.id);
            info.grade1 = grade.grade1;
            db.Entry(info).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            var gradesCompleted = db.grades.Where(g => g.student_id == grade.student_id && g.grade1 == 0).FirstOrDefault();
            var showNext = false;
            if (gradesCompleted == null)
                showNext = true;
            var JsonResult = new { data = "Done", showNext = showNext };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);

        }
        public ActionResult printTotal(int id, int seatNo)
        {
            var eexmaster = db.eexmmasters.Find(id);
            if (eexmaster.eeduppx0 == null || eexmaster.eyear == null || eexmaster.eedulictype == null || eexmaster.eedusection == null || eexmaster.elevel == null)
            {
                return RedirectToAction("uploadFiles", new { id = id });
            }

            ViewBag.StudentId = seatNo;
            ViewBag.id = id;
            ViewBag.destinations = db.destinations.Select(s => new { s.id, s.destination1 }).ToList();

            var student = db.students.Where(s => s.eexmaster_id == id && s.seat_number == seatNo).FirstOrDefault();
            if(student != null)
            {
                ViewBag.student_name = student.name;
                ViewBag.student_id_no14 = student.id_no14;
                //ViewBag.StudentId = student.id;
                var grade = db.requests.Where(g => g.student_id == student.id && g.type == 1).OrderByDescending(r => r.id).FirstOrDefault();
                if(grade != null)
                {
                    ViewBag.max_grade = grade.max_grade;
                    ViewBag.total_grade = grade.total_grade;
                    ViewBag.registration_number = grade.registrationNumber;
                    ViewBag.amount = grade.amount;
                }
            }
            eexmmaster Document = new eexmmaster();
          
            Document = db.eexmmasters.Find(id);
            
            ViewBag.fileName = Document.edocref;
            ViewBag.pageNo = Document.epage;
            ViewBag.cert = Document.eedulictype;
            ViewBag.sho3ba = Document.eeduppx0;
            ViewBag.school = Document.eschool;
            ViewBag.year = Document.eyear;
            ViewBag.section = Document.eedusection;
                
            return View(Document);
        }
        [HttpPost]
        public ActionResult printTotal(int? Id, int? seat_number, string name, int? count, int? destination_id, string id_no14, DateTime? date,float? max_grade , float? total_grade , string registration_number , float? amount,string transfer_number)
        {
            var eexmasterRow = db.eexmmasters.Find(Id);
            var student = db.students.Where(s => s.eexmaster_id == Id && s.seat_number == seat_number).FirstOrDefault();
            if (student == null)
            { 
                student = new student();
                student.name = name;
                student.id_no14 = id_no14;
                student.seat_number = seat_number;
                student.eexmaster_id = Id;
                db.students.Add(student);
                db.SaveChanges();
            }
            request req = new request();
            req.student_id = student.id;
            req.destination_id = destination_id;
            req.count = count;
            req.date = date;
            req.max_grade = max_grade;
            req.total_grade = total_grade;
            req.registrationNumber = registration_number;
            req.amount = amount;
            req.transfer_number = transfer_number;
            req.type = 1;
            db.requests.Add(req);
            db.SaveChanges();
            var JsonResult = new { icon = "success", title = "تم الحفظ بنجاح...", text = ".تم حفظ الطلب بنجاح", eexmasterID = Id, seatNumber = student.seat_number, requestID = req.id };
            
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult makeRequest(int id,int seatNo)
        {
            eexmmaster Document = db.eexmmasters.Find(id);

            ViewBag.StudentId = seatNo;
            ViewBag.eegov = Document.eegov;
            ViewBag.cert = Document.eedulictype;
            ViewBag.sho3ba = Document.eeduppx0;
            ViewBag.school = Document.eschool;
            ViewBag.year = Document.eyear;
            ViewBag.StudentId = seatNo;
            ViewBag.id = id;
            ViewBag.fileName = Document.edocref;
            ViewBag.pageNo = Document.epage;
            ViewBag.destinations = db.destinations.Select(s => new { s.id, s.destination1 }).ToList();

            var student = db.students.Where(s => s.eexmaster_id == id && s.seat_number == seatNo).FirstOrDefault();
            var request = db.requests.Where(r => r.student_id == student.id && r.type == 2).OrderByDescending(r => r.id).FirstOrDefault();
            if(request != null)
            ViewBag.amount = request.amount;
            return View(student);
        }
        [HttpPost]
        public ActionResult makeRequest(int? Id,int? seat_number,string name,int? count, int? destination_id, string id_no14, DateTime? date, float? amount, string registration_number, string transfer_number, float? total_grade)
        {
            
            var eexmasterRow = db.eexmmasters.Find(Id);
            var oldStudent = db.students.Where(s=> s.eexmaster_id == Id && s.seat_number == seat_number).FirstOrDefault();
            oldStudent.name = name;
            oldStudent.id_no14 = id_no14;
            db.Entry(oldStudent).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            request req = new request();
            req.student_id = oldStudent.id;
            req.destination_id = destination_id;
            req.count = count;
            req.date = date;
            req.registrationNumber = registration_number;
            req.amount = amount;
            req.transfer_number = transfer_number;
            req.type = 2;
            req.total_grade = total_grade;
            db.requests.Add(req);
            db.SaveChanges();
            var JsonResult = new { icon = "success", title = "تم الحفظ بنجاح...", text = ".تم حفظ الطلب بنجاح", eexmasterID = Id, seatNumber = oldStudent.seat_number , requestID = req.id };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult showRequests(DatatableParam param)
        {
            //student student = db.students.Find(param.studentID);
            var result = (from r in db.requests
                join s in db.students on r.student_id equals s.id
                join e in db.eexmmasters on s.eexmaster_id equals e.id
                join de in db.destinations on r.destination_id equals de.id into des
                from d in des.DefaultIfEmpty()
                select new RequestsViewModel
                {
                    seat_number=s.seat_number,
                    name=s.name,
                    eschool=e.eschool,
                    count=r.count,
                    destination1=d.destination1,
                    date=r.date.ToString(),
                    student_id=r.student_id,
                    eexmaster_id=s.eexmaster_id,
                    id=r.id,
                    transfer_number=r.transfer_number,
                    type=r.type
                }).Where(s => s.seat_number == param.seatNumber && s.type == 2).OrderByDescending(r => r.date);
            

            var displayResult = result.Skip(param.iDisplayStart)
                .Take(param.iDisplayLength).ToList();
            var totalRecords = result.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }
       
        public ActionResult showTotalPrint(DatatableParam param)
        {
            //student student = db.students.Find(param.studentID);
            var result = (from r in db.requests
                          join s in db.students on r.student_id equals s.id
                          join e in db.eexmmasters on s.eexmaster_id equals e.id
                          join de in db.destinations on r.destination_id equals de.id into des
                          from d in des.DefaultIfEmpty()
                          select new RequestsViewModel
                          {
                              seat_number=s.seat_number,
                              name=s.name,
                              eexmaster_id=s.eexmaster_id,
                              eebook=e.eebook,
                              eschool=e.eschool,
                              count=r.count,
                              destination1=d.destination1,
                              date=r.date.ToString(),
                              student_id=r.student_id,
                              id=r.id,
                              type=r.type
                          }).Where(e => e.eexmaster_id == param.eexmasterID && e.seat_number==param.seatNumber && e.type == 1).OrderByDescending(r => r.date);

            var displayResult = result.Skip(param.iDisplayStart)
                .Take(param.iDisplayLength).ToList();
            var totalRecords = result.Count();

            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = displayResult
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult successStatement(int eexmaster_id, int student_id, int request_id)
        {
            var eexmaster = db.eexmmasters.Find(eexmaster_id);
            var student = db.students.Find(student_id);
            var request = db.requests.Find(request_id);
            var destination = db.destinations.Find(request.destination_id);
            successStatement data = new successStatement();
            data.date = request.date.Value.Day.ToString() + "/" + request.date.Value.Month.ToString() + "/" + request.date.Value.Year.ToString();
            data.shahada = eexmaster.eedulictype;
            data.sho3ba = eexmaster.eeduppx0;
            data.eldoor = eexmaster.elevel;
            data.year = eexmaster.eyear;
            data.seat_id = student.seat_number;
            data.student_name = student.name;
            data.school_name = eexmaster.eschool;
            data.section = eexmaster.eedusection;
            data.student_total_grades = request.total_grade;
            data.max_grades = request.max_grade;
            data.to_destination = destination != null ? destination.destination1 : "";
            data.price = request.amount;
            data.transfer_number = request.transfer_number;
            data.unique_registration = request.registrationNumber;

            return new ViewAsPdf(data);
        }
        public ActionResult getSeat(string studentName)
        {
            var seat = db.students.Where(s => s.name.Contains(studentName)).FirstOrDefault().seat_number;
            return Json(new { data = seat }, JsonRequestBehavior.AllowGet);
        }
      

        public ActionResult gradesInDetails(int eexmaster_id, int student_id, int request_id)
        {
            var eexmaster = db.eexmmasters.Find(eexmaster_id);
            var student = db.students.Find(student_id);
            var request = db.requests.Find(request_id);
            var destination = db.destinations.Find(request.destination_id);

            var eeduppx0 = db.eeduppx0.Where(s => s.eedupp0 == eexmaster.eeduppx0).Select(s => new { id = s.id }).FirstOrDefault();
            var eyear = db.edducyears.Where(s => s.iyear == eexmaster.eyear).Select(y => new { id = y.id }).FirstOrDefault();
            var eedulictype = db.eeduecerts.Where(s => s.Master == eexmaster.eedulictype).Select(s => new { id = s.id }).FirstOrDefault();

            var calcMaxGrade = db.courses.Where(c => c.sho3ba_id == eeduppx0.id && c.year_id == eyear.id && c.shada_id == eedulictype.id && c.is_main == true).Sum(c => c.max_grade);
            var calcMinGrade = db.courses.Where(c => c.sho3ba_id == eeduppx0.id && c.year_id == eyear.id && c.shada_id == eedulictype.id && c.is_main == true).Sum(c => c.min_grade);
            //var calcGrade = db.grades.Where(g => g.student_id == student_id && ).Sum(s => s.grade1);
            
            var calcGrade = (from g in db.grades
                         join c in db.courses on g.course_id equals c.id
                         select new coursesWithGrades
                         {
                             grade1 = g.grade1,
                             is_main = c.is_main,
                             student_id = g.student_id
                         }).Where(e => e.student_id == student_id && e.is_main == true).Sum(s => s.grade1);

            var gradeMain = (from g in db.grades
                                      join c in db.courses on g.course_id equals c.id
                                      select new coursesWithGrades
                                      {
                                          max_grade = c.max_grade,
                                          min_grade = c.min_grade,
                                          course_name = c.name,
                                          grade1 = g.grade1,
                                          is_main = c.is_main,
                                          student_id = g.student_id
                                      }).Where(e => e.student_id == student_id && e.is_main == true);

            var gradeOptional = (from g in db.grades
                             join c in db.courses on g.course_id equals c.id
                             select new coursesWithGrades
                             {
                                 max_grade = c.max_grade,
                                 min_grade = c.min_grade,
                                 course_name = c.name,
                                 grade1 = g.grade1,
                                 is_main = c.is_main,
                                 student_id = g.student_id
                             }).Where(e => e.student_id == student_id && e.is_main == false);

            gradesInDetails data = new gradesInDetails();
            data.seat_id = student.seat_number;
            data.date = request.date != null ?request.date.Value.Day.ToString() + "/" + request.date.Value.Month.ToString() + "/" + request.date.Value.Year.ToString() : "";
            data.student_name = student.name;
            data.school_name = eexmaster.eschool;
            data.section = eexmaster.eedusection;
            data.to_destination = destination != null ? destination.destination1 : "";
            data.price = request.amount;
            data.unique_registration = request.registrationNumber;
            data.coursesWithGradesMain = gradeMain.ToList();
            data.coursesWithGradesOptional = gradeOptional.ToList();
            data.dor = eexmaster.estatus;
            data.year = eexmaster.eyear;
            data.max_grade = calcMaxGrade;
            data.min_grade = calcMinGrade;
            data.total_grade = calcGrade;
            data.transfer_number = request.transfer_number;
            data.eldoor = eexmaster.elevel;
            return new ViewAsPdf(data);
    }

}


}