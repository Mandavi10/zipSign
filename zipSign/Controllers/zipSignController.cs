using BusinessDataLayer;
using BusinessLayerModel;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class zipSignController : Controller
    {
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        private readonly ProcMaster pro = new ProcMaster();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RolesAndRights()
        {
            return Session["UserId"] == null ? RedirectToAction("Index", "Login") : (ActionResult)View();
        }
        public ActionResult RolesAndRights3()
        {
            return View();
        }

        public ActionResult BillingInformation()
        {
            return View();
        }
        public ActionResult Link_Expired()
        {
            return View();
        }
        public ActionResult RolesAndRights2()
        {
            return View();
        }

        public ActionResult BillingInformationDetails()
        {
            return View();
        }
        public ActionResult RequestSign()
        {
            return View();
        }
        public ActionResult Upload()
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
        public ActionResult SignRequest()
        {
            return View();
        }
        public ActionResult SignatureRequest()
        {
            return View();
        }
        public ActionResult SigningRequest()
        {
            return View();
        }
      
        public ActionResult Received()
        {
            return View();
        }
        public ActionResult Draft()
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
        public ActionResult In_Progress()
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
        public ActionResult Signed()
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
        public ActionResult Rejected()
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
        public ActionResult Expired()
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
        public ActionResult BuyMoreSign()
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
        public ActionResult Billing()
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
        public ActionResult SignUsesHistory()
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
        public ActionResult PurchaseHistory()
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
        public ActionResult BuyEnterprisePack()
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
        public ActionResult Invoice()
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
        public ActionResult ViewDocument()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetDetail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetDetail(string filePath)
        {
            List<DataItems> obj = new List<DataItems>();
            string querySelector = "GetSignedDetails"; // Default query selector for ShowRecord operation
            obj.Add(new DataItems("QueryType", querySelector));
            obj.Add(new DataItems("filePath", filePath));
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            string DocumentID = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["DocumentUploadId"]);
            string Name = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UploadedFileName"]);
            string SignedOn = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignedOn"]);
            return Json(new { DocumentID, Name, SignedOn }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NSDLPage()
        {
            return View();
        }
        public ActionResult AllEntities()
        {
            return View();
        }
        public ActionResult SignatureRequestNew()
        {
            return View();
        }
        #region Testing Coordinates Of pdf without Iframe 
        // Testing Coordinates Of pdf without Iframe
        public ActionResult OpenPdfInCanvas()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveRectangleData(int pageNumber, float pdfX, float pdfY, float pdfWidth, float pdfHeight)
        {
            return Json(new { success = true, message = "Rectangle data saved successfully." });
        }
        #endregion

        [HttpPost]              

        public ActionResult SignInsert(SignMaster objsign, string UserType)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (objsign == null || string.IsNullOrEmpty(objsign.DocumentName) || string.IsNullOrEmpty(objsign.UploadedDoc) || string.IsNullOrEmpty(objsign.filePath))
            {
                var errorResult = new
                {
                    status = "101",
                    message = "Required fields are missing.",
                };
                return Json(errorResult, JsonRequestBehavior.AllowGet);
            }
            _ = new List<SignMaster>();
            List<DataItems> obj = new List<DataItems>();
            string UniqueSignerID = CreateRandomCode(5);
            if (UserType == "Single Signer")
            {
                string UniqueID = "612000" + UniqueSignerID;
                obj.Add(new DataItems("DocumentName", objsign.DocumentName1));
                obj.Add(new DataItems("UploadedDoc", objsign.UploadedDoc));
                obj.Add(new DataItems("UploadedFileName", objsign.DocumentName));
                obj.Add(new DataItems("ReferenceNumber", objsign.ReferenceNumber));
                obj.Add(new DataItems("filePath", objsign.filePath));
                obj.Add(new DataItems("SignerType", UserType));
                obj.Add(new DataItems("UploadedBy", Convert.ToInt32(Session["UserId"])));
                obj.Add(new DataItems("IsSent", 0));
                obj.Add(new DataItems("QuerySelector", "InsertSign"));
                statusClass = bal.PostFunction(pro.Sp_SignUpload, obj);
                int UploadedDocumentId = statusClass.StatusCode;
                GetUserDataForTrail(Convert.ToInt32(Session["UserId"]), out string userName, out string userEmail);
                LogTrail("", "Document Upload", userName, userEmail, UploadedDocumentId, UserType);
                if (statusClass.StatusCode >= 0)
                {
                    List<DataItems> obj1 = new List<DataItems>
                    {
                         new DataItems("SignerName", userName),
                          new DataItems("SignerEmail", userEmail),
                        new DataItems("SignerType", UserType),
                        new DataItems("UniqueSignerID", UniqueID),
                        new DataItems("UploadedDocumentId", UploadedDocumentId),
                        new DataItems("UploadedBy", Convert.ToInt32(Session["UserId"])),
                        new DataItems("QuerySelector", "InsertSigner")
                    };
                    statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
                }
                string SignerID = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerID"]);
                return Json(new { UploadedDocumentId, UserType, UniqueID, SignerID }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                obj.Add(new DataItems("UploadedFileName", objsign.DocumentName));
                obj.Add(new DataItems("UploadedDoc", objsign.UploadedDoc));
                obj.Add(new DataItems("DocumentName", objsign.DocumentName));
                obj.Add(new DataItems("ReferenceNumber", objsign.ReferenceNumber));
                obj.Add(new DataItems("UploadedBy", Convert.ToInt32(Session["UserId"])));
                obj.Add(new DataItems("filePath", objsign.filePath));
                obj.Add(new DataItems("QuerySelector", "InsertSign"));
                statusClass = bal.PostFunction(pro.Sp_SignUpload, obj);
                int UploadedDocumentId = statusClass.StatusCode;
                GetUserDataForTrail(Convert.ToInt32(Session["UserId"]), out string userName, out string userEmail);
                LogTrail("", "Document Upload", userName, userEmail, UploadedDocumentId, UserType);
                if (statusClass.StatusCode >= 0)
                {
                    string SignerExpiry = "";
                    string EmailToSend = "";
                    string SignerID = "";
                    string SignerName = "";
                    string UniqueID = "612000" + UniqueSignerID;
                    int i = 1;
                    foreach (SignerInfo signer in objsign.signerInfos)
                    {
                        if (string.IsNullOrEmpty(signer.Name) || string.IsNullOrEmpty(signer.Email) || string.IsNullOrEmpty(signer.MobileNumber) || string.IsNullOrEmpty(signer.signerType) || !Regex.IsMatch(signer.Email, @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$"))
                        {
                            var errorResult = new
                            {
                                status = "101",
                                message = "Invalid or missing signer information.",
                            };
                            return Json(errorResult, JsonRequestBehavior.AllowGet);
                        }
                        List<DataItems> obj1 = new List<DataItems>
                        {
                            new DataItems("SignerName", signer.Name),
                            new DataItems("UploadedDocumentId", UploadedDocumentId),
                            new DataItems("SignerEmail", signer.Email),
                            new DataItems("SignerMobile", signer.MobileNumber),
                            new DataItems("SignerExpiryDay", signer.ExpireInDays),
                            new DataItems("SignerType", signer.signerType),
                            new DataItems("DocumentExpiryDay", signer.DocumentExpiryDay),
                            new DataItems("IsSent", i),
                            new DataItems("UniqueSignerID", UniqueID),
                           new DataItems("UploadedBy", Convert.ToInt32(Session["UserId"])),
                        new DataItems("QuerySelector", "InsertSigner")
                        };
                        statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
                        EmailToSend = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerEmail"]);
                        SignerID = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerID"]);
                        SignerName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerName"]);
                        SignerExpiry = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerExpiryDay"]);
                        i = 0;
                        //LogTrail(SignerID, "Link Sent", EmailToSend, SignerName, UploadedDocumentId);
                    }
                    return Json(new { UploadedDocumentId, EmailToSend, SignerID, SignerName, UniqueID, SignerExpiry }, JsonRequestBehavior.AllowGet);
                }
                var result1 = new
                {
                    status = "101",
                };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
        }
        public void LogTrail(string SignerID, string description, string signername, string Email, int UploadedDocumentId, string UserType)
        {
            string connectionString = GlobalMethods.Global.DocSign.ToString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO TblSignerDetailTrailLog (UniqueSignerID, Action, UserName, EmailID,UploadedDocumentId,CreatedOn,UserType) " + "VALUES (@UniqueSignerID, @Action, @UserName, @EmailID,@UploadedDocumentId,@CreatedOn,@UserType)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UniqueSignerID", SignerID);
                        command.Parameters.AddWithValue("@Action", description);
                        command.Parameters.AddWithValue("@UserName", signername);
                        command.Parameters.AddWithValue("@EmailID", Email);
                        command.Parameters.AddWithValue("@UploadedDocumentId", UploadedDocumentId);
                        command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        command.Parameters.AddWithValue("@UserType", UserType);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Trail logged successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Trail logging failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }


        public void GetUserDataForTrail(int Id, out string userName, out string userEmail)
        {
            userName = string.Empty;
            userEmail = string.Empty;
            string connectionString = GlobalMethods.Global.DocSign.ToString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT Name, Email FROM TblUserMaster WHERE UserMasterID=@UserMasterID", connection))
                    {
                        command.Parameters.AddWithValue("@UserMasterID", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userName = reader["Name"].ToString();
                                userEmail = reader["Email"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Session["UserId"] == null)
            {
                return Json(new { redirectUrl = Url.Action("Login", "Login") });
            }
            _ = new List<DataItems>();

            HttpPostedFileBase file = Request.Files["HelpSectionImages"];

            if (file != null && file.ContentLength > 0)
            {
                string randomKey = CreateRandomKey();
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                string[] allowedExtensions = new[] { ".pdf" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { status = "101" }, JsonRequestBehavior.AllowGet);
                }
                int maxFileSize = 10 * 1024 * 1024; // 10 MB
                if (file.ContentLength > maxFileSize)
                {
                    return Json(new { status = "File size exceeds the maximum allowed limit." }, JsonRequestBehavior.AllowGet);
                }
                string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"{originalFileName}_{timestamp}_{randomKey}{fileExtension}";
                string localFilePath = "/Uploads/SignUpload/" + fileName;
                string filePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), fileName);
                _ = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), originalFileName + "_temp.pdf");
                try
                {
                    //file.SaveAs(tempFilePath);
                    file.SaveAs(filePath);
                    //using (PdfReader reader = new PdfReader(tempFilePath))
                    //using (PdfStamper stamper = new PdfStamper(reader, new FileStream(filePath, FileMode.Create)))
                    //{
                    //    int pageCount = reader.NumberOfPages;
                    //    stamper.InsertPage(pageCount + 1, PageSize.A4);
                    //}
                    //// Delete the temp file
                    //System.IO.File.Delete(tempFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return Json(new { status = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = filePath, LocalPath = localFilePath, uniquefileName = fileName }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "101" }, JsonRequestBehavior.AllowGet);
        }
        private static int GetPdfPageCount(string pdfPath)
        {
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            {
                return pdfReader.NumberOfPages;
            }

        }
        public ActionResult DeleteFile(string fileName)
        {
            try
            {
                string filePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error deleting file: " + ex.Message;
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
        public FileContentResult DownloadExcelFile()
        {
            List<SignMaster> result = new List<SignMaster>();
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QuerySelector", "ShowData")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new SignMaster
                    {
                        //SignUploadId = Convert.ToInt32(dr["SignUploadId"]),
                        UploadedFileName = Convert.ToString(dr["UploadedFileName"]),
                        DocumentName = Convert.ToString(dr["DocumentName"]),
                        SignStatus = Convert.ToString(dr["SignStatus"]),
                        UploadedOn = Convert.ToString(dr["UploadedOn"]),
                        UploadedBy = Convert.ToString(dr["UploadedBy"]),
                    });
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SignUpload Id,DocumentName,DocumentName,SignStatus,UploadedOn,UploadedBy");
            foreach (SignMaster item in result)
            {
                sb.AppendLine($"{item.DocumentName},{item.DocumentName},{item.SignStatus},{item.UploadedOn},{item.UploadedBy}");
            }
            byte[] fileContents = Encoding.UTF8.GetBytes(sb.ToString());//Converts The File Contents Into Byte Array
            return File(fileContents, "application/vnd.ms-excel", "DemoExcel.csv");
        }

        [HttpPost]
        public JsonResult GetFileData1(string SignUploadId1)
        {
            try
            {
                ResultData res1 = new ResultData();
                SignMaster result = new SignMaster();
                List<DataItems> obj = new List<DataItems>();
                string id = SignUploadId1;
                string fileId1 = AESEncryption.AESEncryptionClass.DecryptAES(SignUploadId1 + "==");
                obj.Add(new DataItems("QuerySelector", "ShowDoc"));
                obj.Add(new DataItems("DocumentUploadId", fileId1));
                statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
                if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                    {

                        result.filePath = Convert.ToString(dr["filePath"]);

                    }
                }
                if (statusClass.StatusCode == 1)
                {
                    var result1 = new
                    {
                        status = statusClass.StatusCode   //PathFound
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result1 = new
                    {
                        status = statusClass.StatusCode, //Updated
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
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
        public JsonResult GetFileData(string SignUploadId1)
        {
            try
            {
                ResultData res1 = new ResultData();
                SignMaster result = new SignMaster();
                List<DataItems> obj = new List<DataItems>();
                string fileId1 = AESEncryption.AESEncryptionClass.DecryptAES(SignUploadId1);
                result.filePath = fileId1;
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


        public static string CreateRandomCode(int CodeLength)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            // Loop Starts to Generate Random Number 
            for (int i = 0; i < CodeLength; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks)); // Used here System DateTime to give more diversification.
                }
                int t = rand.Next(62);
                if (temp != -1 && temp == t)
                {
                    // Recursive Calling of parent function 
                    return CreateRandomCode(CodeLength);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }


        public JsonResult GetSuggestions(string userName)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QueryType", "ShowRecordMatching"),
                new DataItems("SignerName", userName)
            };
            // Assuming bal.GetFunctionWithResult returns some sort of result
            CommonStatus statusClass = bal.GetFunctionWithResult(pro.Sp_SignerMaster, obj);

            List<object> suggestionsFromBusinessLogic = new List<object>();
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    suggestionsFromBusinessLogic.Add(new
                    {
                        Name = Convert.ToString(dr["SignerName"]),
                        Email = Convert.ToString(dr["SignerEmail"]),
                        Mobile = Convert.ToString(dr["SignerMobile"])
                    });
                }
            }
            List<object> suggestionsFromCustomLogic = GetSuggestionsFromCustomLogic(userName);
            return Json(suggestionsFromCustomLogic, JsonRequestBehavior.AllowGet);
        }

        private List<object> GetSuggestionsFromCustomLogic(string userName)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QueryType", "ShowRecordMatching"),
                new DataItems("SignerName", userName)
            };
            CommonStatus statusClass = bal.GetFunctionWithResult(pro.Sp_SignerMaster, obj);

            List<object> suggestions = new List<object>();
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    suggestions.Add(new
                    {
                        Name = Convert.ToString(dr["SignerName"]),
                        Email = Convert.ToString(dr["SignerEmail"]),
                        Mobile = Convert.ToString(dr["SignerMobile"])
                    });
                }
            }
            return suggestions;
        }
        public ActionResult DownloadFile(string FilePath)
        {
            try
            {
                string filePath = Server.MapPath(FilePath);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string originalFileName = Path.GetFileName(filePath);
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + originalFileName);
                    return File(fileBytes, "application/pdf");
                }
                else
                {
                    return HttpNotFound("File not found");
                }
            }
            catch (Exception ex)
            {
                // Log and handle the exception gracefully
                return Content("An error occurred: " + ex.Message);
            }
        }
        public JsonResult SearchData1(pagination objpage)
        {
            ResultData res1 = new ResultData();
            List<SignMaster> result = new List<SignMaster>();
            List<pagination> result2 = new List<pagination>();
            List<DataItems> obj = new List<DataItems>();
            string querySelector = "ShowRecord"; // Default query selector for ShowRecord operation
            if (!string.IsNullOrEmpty(objpage.keyword))
            {
                querySelector = "Search"; // Set query selector for search operation
                obj.Add(new DataItems("UploadedFileName", objpage.keyword));
            }
            int Id = Convert.ToInt32(Session["UserId"]);
            obj.Add(new DataItems("QuerySelector", querySelector));
            obj.Add(new DataItems("PageCount", objpage.pagecount));
            obj.Add(new DataItems("UploadedBy", Id));
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new SignMaster
                    {
                        DocumentUploadId = Convert.ToInt32(dr["DocumentUploadId"]),
                        UploadedFileName = Convert.ToString(dr["UploadedFileName"]),
                        DocumentName = Convert.ToString(dr["DocumentName"]),
                        SignStatus = Convert.ToString(dr["SignStatus"]),
                        UploadedOn = Convert.ToString(dr["UploadedOn"]),
                        UploadedBy = Convert.ToString(dr["UploadedBy"]),

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

        public ActionResult Delete(int fileCode)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("DocumentUploadId", fileCode),
                new DataItems("QuerySelector", "DeleteRecord")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendToSigningRequest(int fileCode)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("DocumentUploadId", fileCode),
                new DataItems("QuerySelector", "ShowData")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);

            string UploadedFileName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UploadedFileName"]);
            string FilePath = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["FilePath"]);
            //string UploadedBy = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UploadedBy"]);
            return Json(UploadedFileName, FilePath, JsonRequestBehavior.AllowGet);
        }



        //public JsonResult SaveRolesAndPermissions(string roleName, string description, string checkboxData)
        //{

        //    try
        //    {
        //        // Deserialize the JSON data
        //        var checkedCheckboxData = JsonConvert.DeserializeObject<List<CheckboxData>>(checkboxData);
        //        var checkedCheckboxData = new List<CheckboxData>();
        //        foreach (var page in pages)
        //        {
        //            // Extract the last segment of the URL as the pageId
        //            string[] segments = new Uri(page.PageUrl).Segments;
        //            int pageId = 0;
        //            if (segments.Length > 0)
        //            {
        //                string lastSegment = segments.Last();
        //                if (int.TryParse(lastSegment, out pageId))
        //                {
        //                    checkedCheckboxData.Add(new CheckboxData
        //                    {
        //                        name = page.PageName,
        //                        read = false, // Set to appropriate value
        //                        write = false, // Set to appropriate value
        //                        link = page.PageUrl,
        //                        pageId = pageId
        //                    });
        //                }
        //            }
        //        }
        //        // Filter out the data where link is not null
        //        var dataToSave = checkedCheckboxData.Where(item => !string.IsNullOrEmpty(item.link)).ToList();

        //        // Connection string - Replace with your actual connection string
        //        string connectionString = GlobalMethods.Global.DocSign.ToString();

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            foreach (var checkbox in dataToSave)
        //            {

        //                List<DataItems> obj = new List<DataItems>
        //        {
        //            new DataItems("PageName", checkbox.name),
        //            new DataItems("Read", checkbox.read),
        //            new DataItems("Write", checkbox.write),
        //            new DataItems("PageLink", checkbox.link),
        //            new DataItems("CreatedBy",Convert.ToInt32(Session["UserId"])),
        //            new DataItems("QueryType", "RolesAndRights")
        //        };
        //                statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
        //            }

        //            return Json(new { success = true, message = "Roles and permissions saved successfully." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}



        private static readonly Dictionary<string, int> PageIdMapping = new Dictionary<string, int>
    {
        { "/zipSign/BuyEnterprisePack", 1 },
        { "/MySigns/BuySignHistory", 2 },
        { "/MySigns/SignTransactionLog", 3 },
        //{ "/MySigns/SignTransactionLog", 4 },
        { "/MySigns/SignPricingMechanism", 5 },
        { "/zipSign/SignUsesHistory", 6 },
        { "/zipSign/PurchaseHistory", 7 },
        //{ "/Masters/DepartmentMaster", 6 },
        //{ "/Masters/DepartmentMaster", 6 },
    };

        [HttpPost]
        public JsonResult SaveRolesAndPermissions(string roleName, string description, string checkboxData)
        {
            try
            {
                // Deserialize the JSON data
                var checkedCheckboxData = JsonConvert.DeserializeObject<List<CheckboxData>>(checkboxData);

                // Assign static IDs based on the mapping


                // Filter out the data where link is not null
                var dataToSave = checkedCheckboxData.Where(item => !string.IsNullOrEmpty(item.link)).ToList();
                foreach (var checkbox in dataToSave)
                {
                    // Use the link to find the static ID from the mapping
                    if (PageIdMapping.TryGetValue(checkbox.link, out int staticPageId))
                    {
                        checkbox.pageId = staticPageId;
                    }
                    else
                    {
                        // Handle the case where the link doesn't match any mapping
                        // You might want to assign a default value or throw an exception
                        checkbox.pageId = 0; // Change this to your default value
                    }
                }

                // Connection string - Replace with your actual connection string
                string connectionString = GlobalMethods.Global.DocSign.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (var checkbox in dataToSave)
                    {
                        List<DataItems> obj = new List<DataItems>
                    {
                        new DataItems("PageID",checkbox.pageId),
                        new DataItems("PageName", checkbox.name),
                        new DataItems("Read", checkbox.read),
                        new DataItems("Write", checkbox.write),
                        new DataItems("PageLink", checkbox.link),
                        new DataItems("CreatedBy", Convert.ToInt32(Session["UserId"])),
                        new DataItems("QueryType", "RolesAndRights")
                    };
                        statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
                    }

                    return Json(new { success = true, message = "Roles and permissions saved successfully." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }





    }
}
