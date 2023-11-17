using BusinessDataLayer;
using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{

    public class UsersController : Controller
    {
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        private readonly ProcMaster pro = new ProcMaster();
        public ActionResult UserCreation()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }
        public ActionResult AllUsers()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult UserInsert(UserInsert1 obj1, string Domain)
        {
            try
            {
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("UserCode", obj1.UserCode),
                    new DataItems("Username", obj1.Username),
                    new DataItems("EmailId", obj1.EmailId),
                    new DataItems("MobileNo", obj1.MobileNo),
                    new DataItems("UserType", obj1.UserType),
                    new DataItems("Department", obj1.Department),
                    new DataItems("Designation", obj1.Designation),
                    new DataItems("CreatedBy", obj1.CreatedBy),
                    new DataItems("CreatedOn", obj1.CreatedOn),
                    new DataItems("ModifyBy", obj1.ModifyBy),
                    new DataItems("ModifyOn", obj1.ModifyOn),
                    new DataItems("Active", obj1.Active),
                    new DataItems("Mobileapp", obj1.Mobileapp),
                    new DataItems("Domain", Domain)
                };

                if (Domain == "SpecificDomain")
                {
                    obj.Add(new DataItems("SpecificDomaincontrol", obj1.SpecificDomaincontrol));
                }
                else
                {
                    obj.Add(new DataItems("SpecificDomaincontrol", "All"));
                }
                obj.Add(new DataItems("QueryType", "InsertUser"));
                statusClass = bal.PostFunction(pro.Sp_UserCreation, obj);
                if (statusClass.StatusCode == 1)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (statusClass.StatusCode == 2)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode, //Duplicate User Code
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (statusClass.StatusCode == 3)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode, //Duplicate Email
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else if (statusClass.StatusCode == 4)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode, //Duplicate Mobile Number
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {

            }
            var result1 = new
            {
                status = "101",
            };
            return Json(result1, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SearchData1(pagination objpage)
        {
            ResultData2 res1 = new ResultData2();
            List<UserInsert1> result = new List<UserInsert1>();
            List<pagination> result2 = new List<pagination>();
            List<DataItems> obj = new List<DataItems>();
            string queryType = "ShowRecord"; // Default query selector for ShowRecord operation
            if (!string.IsNullOrEmpty(objpage.keyword))
            {
                queryType = "Search"; // Set query selector for search operation
                obj.Add(new DataItems("UserName", objpage.keyword));
            }
            obj.Add(new DataItems("QueryType", queryType));
            obj.Add(new DataItems("PageCount", objpage.pagecount));
            statusClass = bal.GetFunctionWithResult(pro.Sp_UserCreation, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new UserInsert1
                    {

                        Userid = Convert.ToInt32(dr["Userid"]),
                        UserCode = Convert.ToString(dr["UserCode"]),
                        Username = Convert.ToString(dr["Username"]),
                        EmailId = Convert.ToString(dr["EmailId"]),
                        MobileNo = Convert.ToString(dr["MobileNo"]),
                        UserType = Convert.ToString(dr["UserType"]),
                        Department = Convert.ToString(dr["Department"]),
                        Designation = Convert.ToString(dr["Designation"]),
                        CreatedBy = Convert.ToString(dr["CreatedBy"]),
                        CreatedOn = Convert.ToString(dr["CreatedOn"]),
                        ModifyBy = Convert.ToString(dr["ModifyBy"]),
                        ModifyOn = Convert.ToString(dr["ModifyOn"]),
                        Active = Convert.ToString(dr["Active"]),
                        Mobileapp = Convert.ToString(dr["Mobileapp"]),
                        SpecificDomaincontrol = Convert.ToString("SpecificDomaincontrol")
                    });
                }
                res1.Table1 = result;
            }
            if (statusClass.DataFetch.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[1].Rows)
                {
                    result2.Add(new pagination
                    {
                        pagecount = Convert.ToInt32(dr["pagecount"]),
                        count = Convert.ToInt32(dr["count"]),
                        page = Convert.ToInt32(dr["page"]),
                        size = Convert.ToInt32(dr["Size"]),
                    });
                }
                res1.Table2 = result2;
            }

            return Json(res1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetUserDetails(string UserCode)
        {
            ResultData2 res1 = new ResultData2();
            List<UserInsert1> result = new List<UserInsert1>();
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QueryType", "ShowRecordLbl"),
                new DataItems("UserCode", UserCode)
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_UserCreation, obj);
            foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
            {
                result.Add(new UserInsert1
                {
                    Userid = Convert.ToInt32(dr["Userid"]),
                    UserCode = Convert.ToString(dr["UserCode"]),
                    Username = Convert.ToString(dr["Username"]),
                    EmailId = Convert.ToString(dr["EmailId"]),
                    MobileNo = Convert.ToString(dr["MobileNo"]),
                    UserType = Convert.ToString(dr["UserType"]),
                    Department = Convert.ToString(dr["Department"]),
                    Designation = Convert.ToString(dr["Designation"]),
                    CreatedBy = Convert.ToString(dr["CreatedBy"]),
                    CreatedOn = Convert.ToString(dr["CreatedOn"]),
                    ModifyBy = Convert.ToString(dr["ModifyBy"]),
                    ModifyOn = Convert.ToString(dr["ModifyOn"]),
                    Active = Convert.ToString(dr["Active"]),
                    Mobileapp = Convert.ToString(dr["Mobileapp"]),
                    SpecificDomaincontrol = Convert.ToString(dr["SpecificDomaincontrol"])
                });
            }
            res1.Table1 = result;
            return Json(res1, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult DeptDdl()
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QuerySELECTor", "DDL")
            };
            statusClass = bal.GetFunction(pro.DepartmentMaster, obj);
            List<SelectListItem> role = new List<SelectListItem>();
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                role.Add(new SelectListItem { Value = "0", Text = "Select" });
                foreach (DataRow item in statusClass.DataFetch.Tables[0].Rows)
                {
                    role.Add(new SelectListItem { Value = item["DepartmentId"].ToString(), Text = item["DepartmentName"].ToString() });
                }
            }
            return Json(role, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ProfileInsert(profile objpro)
        {
            try
            {
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("FirstName", objpro.FirstName),
                    new DataItems("LastName", objpro.LastName),
                    new DataItems("Add1", objpro.Add1),
                    new DataItems("Add2", objpro.Add2),
                    new DataItems("Pin", objpro.Pin),
                    new DataItems("Gender", objpro.Gender),
                    new DataItems("DOB", objpro.DOB),
                    new DataItems("GST", objpro.GST),
                    new DataItems("ComWebURL", objpro.ComWebURL),
                    new DataItems("EDomian", objpro.EDomian),
                    new DataItems("QueryType", "Insert")
                };
                statusClass = bal.PostFunction(pro.SP_UserProfile, obj);
                var result = new
                {
                    status = "200",
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var result1 = new
                {
                    status = "101",
                };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UploadProfilePicture()
        {
            try
            {
                // Get the uploaded file from the request
                HttpPostedFileBase file = Request.Files[0];

                // Generate a unique filename for the profile picture
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Specify the directory where the profile pictures will be stored
                string uploadPath = Server.MapPath("~/Content/images/profiles/");

                // Save the profile picture to the specified directory
                file.SaveAs(Path.Combine(uploadPath, fileName));

                // Return the filename of the saved profile picture
                var result = new
                {
                    status = "200",
                    fileName
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result1 = new
                {
                    status = "101",
                };
                Console.WriteLine(ex);
                return Json(result1, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult UpdateUserDetails(string UserCode)
        {
            try
            {
                ResultData2 res1 = new ResultData2();
                List<UserInsert1> result = new List<UserInsert1>();
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("QueryType", "UpdateStatus"),
                    new DataItems("UserCode", UserCode)
                };
                statusClass = bal.GetFunctionWithResult(pro.Sp_UserCreation, obj);
                if (statusClass.StatusCode == 1)
                {

                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var result1 = new
                {
                    status = "101",
                };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ChangeProfilePicture(HttpPostedFileBase file)
        {
            var newPictureUrl = "/Content/images/new-profile-picture.jpg";

            return Json(new { newProfilePictureUrl = newPictureUrl }, JsonRequestBehavior.AllowGet);
        }

    }
}





