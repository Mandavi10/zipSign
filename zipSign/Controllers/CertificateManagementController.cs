using BusinessAccessLayer;
using BusinessLayerModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class CertificateManagementController : Controller
    {
        private readonly BusinessAccesslayerClass bal = new BusinessAccesslayerClass();
        private CommonStatus statusClass = new CommonStatus();

        //Security objSecurity = new Security();
        private readonly ProcMaster pro = new ProcMaster();
        public ActionResult CertManagement()
        {
            return View();
        }
        public ActionResult AllDocumentSignerCertificate()
        {
            return View();
        }
        public ActionResult UploadDocument()
        {
            HttpPostedFileBase file = Request.Files["HelpSectionImages"];

            if (file != null && file.ContentLength > 0)
            {
                string randomKey = CreateRandomKey();
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                string[] allowedExtensions = new[] { ".p12", ".pfx", "p12", "pfx" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { status = "101" }, JsonRequestBehavior.AllowGet);
                }
                string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"{originalFileName}_{timestamp}{fileExtension}";
                string filePath = Path.Combine(Server.MapPath("~/Content/Uploaded_DSC_Certificates"), fileName);
                try
                {
                    file.SaveAs(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return Json(new { status = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = filePath }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "101" }, JsonRequestBehavior.AllowGet);
        }

        public static string CreateRandomKey()
        {
            string _allowedChars = "09876543212345678901234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random((int)DateTime.Now.Ticks);
            char[] chars = new char[9];
            for (int i = 0; i < 9; i++)
            {
                chars[i] = _allowedChars[randNum.Next(_allowedChars.Length)];
            }
            return new string(chars);
        }


        [HttpPost]
        public bool ValidateCertWithPassword(string certificateforvalidate, string password)
        {
            try
            {
                if (certificateforvalidate == null || string.IsNullOrEmpty(password))
                {
                    return false;
                }
                else
                {
                    X509Certificate2 certificate = new X509Certificate2(certificateforvalidate, password, X509KeyStorageFlags.MachineKeySet);
                    if (certificate != null && certificate.NotAfter >= DateTime.Now && certificate.NotBefore <= DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public class CertificationManagement
        {
            public int Userid { get; set; }
            public string UserCode { get; set; }
            public string Username { get; set; }
            public string EmailId { get; set; }
            public string MobileNo { get; set; }

        }
        [HttpPost]
        public ActionResult SaveCertificate(DSCCertificateMgt DSCCM)
        {
            if (string.IsNullOrEmpty(DSCCM.CertificateName) || string.IsNullOrEmpty(DSCCM.CertificateType) || string.IsNullOrEmpty(DSCCM.Password) || string.IsNullOrEmpty(DSCCM.Path))
            {
                return Json(new { status = "400", message = "All fields are required." }, JsonRequestBehavior.AllowGet);
            }
            List<CertificationManagement> users = JsonConvert.DeserializeObject<List<CertificationManagement>>(DSCCM.Table);
            string EncPassword = AESEncryption.AESEncryptionClass.EncryptAES(DSCCM.Password);

            List<DataItems> obj = new List<DataItems>();
            obj.Add(new DataItems("CertificateName", DSCCM.CertificateName));
            obj.Add(new DataItems("CertificateType", DSCCM.CertificateType));
            obj.Add(new DataItems("Password", EncPassword));
            obj.Add(new DataItems("Path", DSCCM.Path));
            obj.Add(new DataItems("Role", DSCCM.Role));
            obj.Add(new DataItems("PasswordType", DSCCM.PasswordType));
            obj.Add(new DataItems("UploadedBy", "1"));
            obj.Add(new DataItems("QueryType", "UploadCertificate"));

            statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);
            int CertificateId = Convert.ToInt32(statusClass.DataFetch.Tables[0].Rows[0]["CertificateId"]);
            if (statusClass.StatusCode == 1)
            {
                foreach (CertificationManagement user in users)
                {
                    List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("Name", user.Username),
                new DataItems("Email", user.EmailId),
                new DataItems("MobileNo", user.MobileNo),
                new DataItems("Role", DSCCM.Role),
                new DataItems("CertificateId", CertificateId),
                new DataItems("CertificateName", DSCCM.CertificateName),
                new DataItems("CreatedBy","1"),
                new DataItems("QueryType", "DSCCertificateUsers"),
                };
                    statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj1);
                }
            }

            var result1 = new
            {
                status = "201",
            };

            return Json(result1, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult SearchandShowDataForCertificate(pagination objpage)
        {
            ResultDataForCertificate res1 = new ResultDataForCertificate();
            List<DSCCertificateMgt> result = new List<DSCCertificateMgt>();
            List<pagination> result2 = new List<pagination>();
            List<DataItems> obj = new List<DataItems>();
            string QueryType = "ShowCertificate";
            if (!string.IsNullOrEmpty(objpage.keyword))
            {
                QueryType = "SearchforSigned";
                obj.Add(new DataItems("UploadedFileName", objpage.keyword));
            }
            obj.Add(new DataItems("QueryType", QueryType));
            obj.Add(new DataItems("PageCount", objpage.pagecount));
            statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new DSCCertificateMgt
                    {
                        Row = Convert.ToInt32(dr["Row"]),
                        CertificateName = Convert.ToString(dr["CertificateName"]),
                        CertificateType = Convert.ToString(dr["CertificateType"]),
                        UploadedOn = Convert.ToString(dr["UploadedOn"]),
                        UploadedBy = Convert.ToString(dr["UploadedBy"]),
                        // CreatedOn = Convert.ToString(dr["CreatedOn"]),
                        // UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                        // IsActive = Convert.ToString(dr["IsActive"]),
                    });
                }
                res1.Table1 = result;
            }

            if (statusClass.DataFetch.Tables.Count > 1 && statusClass.DataFetch.Tables[1].Rows.Count > 0)
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
        public ActionResult DownloadCertificate(string CerificateID)
        {
            string certificatePath = GetCertificatePath(CerificateID);

            if (!string.IsNullOrEmpty(certificatePath) && System.IO.File.Exists(certificatePath))
            {
                byte[] certificateData = System.IO.File.ReadAllBytes(certificatePath);
                string fileExtension = Path.GetExtension(certificatePath);
                string contentType;
                if (fileExtension.Equals(".p12", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/x-pkcs12";
                }
                else if (fileExtension.Equals(".pfx", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/x-pkcs12";
                }
                else
                {
                    return HttpNotFound("Unsupported file type.");
                }

                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(certificatePath)}");
                Response.BinaryWrite(certificateData);
                Response.Flush();
                Response.End();
            }
            else
            {
                return HttpNotFound("Certificate file not found for the specified department code.");
            }

            return new EmptyResult();
        }

        private string GetCertificatePath(string CerificateID)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("CertificateId", CerificateID),
                new DataItems("QueryType", "DownloadCertificatePath")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);
            string path = statusClass.DataFetch.Tables[0].Rows[0]["Path"].ToString();
            return path;
        }

        public ActionResult DeleteCertificate(string CertificateID)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("CertificateId", CertificateID),
                new DataItems("QueryType", "DeleteCertificate")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);
            var result1 = new
            {
                status = "201",
            };
            return RedirectToAction("AllDocumentSignerCertificate", "CertificateManagement");
        }

        [HttpPost]
        public JsonResult SearchDataForUsers(pagination objpage)
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
    }
}