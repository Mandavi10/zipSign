using BusinessDataLayer;
using BusinessLayerModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class zipSignController : Controller
    {
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        private ProcMaster pro = new ProcMaster();
        //Security objSecurity = new Security();


        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RequestSign()
        {
            return View();
        }
        public ActionResult Upload()
        {
            return View();
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
            return View();
        }
        public ActionResult In_Progress()
        {
            return View();
        }
        public ActionResult Signed()
        {
            return View();
        }
        public ActionResult Rejected()
        {
            return View();
        }
        public ActionResult Expired()
        {
            return View();
        }
        public ActionResult BuyMoreSign()
        {
            return View();
        }
        public ActionResult Billing()
        {
            return View();
        }
        public ActionResult SignUsesHistory()
        {
            return View();
        }
        public ActionResult PurchaseHistory()
        {
            return View();
        }
        public ActionResult BuyEnterprisePack()
        {
            return View();
        }
        public ActionResult Invoice()
        {
            return View();
        }
        public ActionResult ViewDocument()
        {
            return View();
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

        //public ActionResult NSDLPage()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult SignInsert(SignMaster objsign, string UserType)
        {
            List<SignMaster> result = new List<SignMaster>();
            List<DataItems> obj = new List<DataItems>();
            string UniqueSignerID = CreateRandomCode(5);


            if (UserType == "Single Signer")
            {
                string UniqueID = "612000" + UniqueSignerID;
                // Add common document details
                obj.Add(new DataItems("DocumentName", objsign.DocumentName));
                obj.Add(new DataItems("UploadedDoc", objsign.UploadedDoc));
                obj.Add(new DataItems("UploadedFileName", objsign.DocumentName));
                obj.Add(new DataItems("ReferenceNumber", objsign.ReferenceNumber));
                obj.Add(new DataItems("filePath", objsign.filePath));
                obj.Add(new DataItems("SignerType", UserType));
                obj.Add(new DataItems("IsSent", 0));
                obj.Add(new DataItems("QuerySelector", "InsertSign"));
                statusClass = bal.PostFunction(pro.Sp_SignUpload, obj);
                int UploadedDocumentId = statusClass.StatusCode;
                if (statusClass.StatusCode >= 0)
                {
                    List<DataItems> obj1 = new List<DataItems>
                    {
                        new DataItems("SignerType", UserType),
                        new DataItems("UniqueSignerID", UniqueID),
                        new DataItems("UploadedDocumentId", UploadedDocumentId),
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
                obj.Add(new DataItems("filePath", objsign.filePath));
                obj.Add(new DataItems("QuerySelector", "InsertSign"));
                statusClass = bal.PostFunction(pro.Sp_SignUpload, obj);
                int UploadedDocumentId = statusClass.StatusCode;

                if (statusClass.StatusCode >= 0)
                {
                    string EmailToSend = "";
                    string SignerID = "";
                    string SignerName = "";
                    string UniqueID = "612000" + UniqueSignerID;
                    int i = 1;
                    foreach (SignerInfo signer in objsign.signerInfos)
                    {
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
                            new DataItems("QuerySelector", "InsertSigner")
                        };
                        statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
                        EmailToSend = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerEmail"]);
                        SignerID = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerID"]);
                        SignerName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerName"]);
                        i = 0;
                    }
                    return Json(new { UploadedDocumentId, EmailToSend, SignerID, SignerName, UniqueID }, JsonRequestBehavior.AllowGet);
                }
                var result1 = new
                {
                    status = "101",
                };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            List<DataItems> obj = new List<DataItems>();

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
                string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"{originalFileName}_{timestamp}_{randomKey}{fileExtension}";
                string localFilePath = "/Uploads/SignUpload/" + fileName;
                string filePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), fileName);
                string tempFilePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), originalFileName + "_temp.pdf");
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
            // Retrieve your data and populate the 'result' list
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

        //[HttpPost]
        //public ActionResult ConvertToPdf()
        //{
        //    var file = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];

        //    // Replace "inputWordFilePath" and "outputPdfFilePath" with the actual file paths.
        //    string inputWordFilePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), file.FileName);
        //    string outputPdfFilePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"));

        //    try
        //    {
        //        // Create a new PDF file using FileStream.
        //        using (var outputStream = new FileStream(outputPdfFilePath, FileMode.Create))
        //        {
        //            using (var wordDocument = WordprocessingDocument.Open(inputWordFilePath, false))
        //            {
        //                // Create an instance of WordprocessingDocument and open the Word document.

        //                // Create an instance of the PDF part.
        //                var pdfPart = wordDocument.MainDocumentPart.ConvertToPdf(outputStream);

        //                // Close the PDF part and the Word document.
        //                pdfPart.PdfDocument.Close();
        //            }
        //        }

        //        ViewBag.Message = "File converted successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "Error converting the file: " + ex.Message;
        //    }

        //    return View();
        //}


        //public JsonResult GetData1(SignerMaster objsigner)
        //{
        //    List<SignerMaster> result = new List<SignerMaster>();
        //    List<DataItems> obj = new List<DataItems>();
        //    obj.Add(new DataItems("QueryType", "ShowRecord"));
        //    statusClass = bal.GetFunctionWithResult(pro.Sp_SignerMaster, obj);
        //    if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
        //        {
        //            result.Add(new SignerMaster
        //            {
        //                SignerName = Convert.ToString(dr["SignerName"]),
        //                SignerEmail = Convert.ToString(dr["SignerEmail"]),
        //                SignerMobile = Convert.ToString(dr["SignerMobile"]),

        //            });
        //        }
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    var result1 = new
        //    {
        //        status = "201",
        //    };
        //    return Json(result1, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult UploadFiles(SignMaster objsign)
        //{
        //    System.Web.HttpPostedFile file = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];

        //    if (file != null && file.ContentLength > 0)
        //    {
        //        string image = CreateRandomKey();
        //        string fileExtension = Path.GetExtension(file.FileName).ToLower();
        //        string[] allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };

        //        if (!allowedExtensions.Contains(fileExtension))
        //        {
        //            return Json(new { status = "101" }, JsonRequestBehavior.AllowGet);
        //        }

        //        string fileName = image + fileExtension;
        //        string filePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), fileName);

        //        if (fileExtension == ".pdf")
        //        {
        //            file.SaveAs(filePath);
        //        }
        //        else if (fileExtension == ".doc" || fileExtension == ".docx")
        //        {
        //            string pdfFilePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), image + ".pdf");
        //            //ConvertWordToPdf(file.InputStream, pdfFilePath);
        //            filePath = pdfFilePath;
        //        }
        //        else
        //        {
        //            string pdfFilePath = Path.Combine(Server.MapPath("~/Uploads/SignUpload"), image + ".pdf");
        //            //ConvertImageToPdf(file.InputStream, pdfFilePath);
        //            filePath = pdfFilePath;
        //        }
        //        //objsign.filePath = filePath;

        //        return Json(new { status = filePath }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { status = "101" }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult ProxyExternalContent()
        {
            using (HttpClient client = new HttpClient())
            {
                string externalUrl = "https://pregw.esign.egov-nsdl.com/nsdl-esp/authenticate/esign-doc/";
                string content = client.GetStringAsync(externalUrl).Result;
                return Content(content, "text/html");
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
            obj.Add(new DataItems("QuerySelector", querySelector));
            obj.Add(new DataItems("PageCount", objpage.pagecount));
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
                        // CreatedOn = Convert.ToString(dr["CreatedOn"]),
                        //UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                        //IsActive = Convert.ToString(dr["IsActive"]),
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

    }
}





