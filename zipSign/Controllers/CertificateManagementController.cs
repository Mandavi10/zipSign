using BusinessAccessLayer;
using BusinessLayerModel;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public bool ValidateCertWithPasswordForSigning(string selectedValue, string password)
        {
            bool flag = true;
            string Path = null;
            string connectionString = GlobalMethods.Global.DocSign.ToString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SELECT Path FROM Tbl_CertificateMgt WHERE CertificateId=@CertificateId", connection))
                    {
                        command.Parameters.AddWithValue("@CertificateId", selectedValue);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Path = reader["Path"].ToString();
                            }
                        }
                    }
                }

                if (Path == null || string.IsNullOrEmpty(password))
                {
                    flag = false;
                }
                else
                {
                    X509Certificate2 certificate = new X509Certificate2(Path, password, X509KeyStorageFlags.MachineKeySet);
                    if (certificate != null && certificate.NotAfter >= DateTime.Now && certificate.NotBefore <= DateTime.Now)
                    {
                        string pdfFilePath = "";
                        string destinationPath = "";
                        PdfReader pdfReader = new PdfReader(pdfFilePath);
                        string pfxFilePath = "D:\\DSC\\PFXs\\Class II Organization 2 Year Document Signer Signature-2022.pfx";
                        string pfxPassword = "abc1234";
                        Pkcs12Store pfxKeyStore = new Pkcs12Store(new FileStream(pfxFilePath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());
                        int page = pdfReader.NumberOfPages;
                        for (int i = 1; i <= page; i++)
                        {
                            if (i > 1)
                            {
                                FileStream stremfile = new FileStream(destinationPath, FileMode.Open, FileAccess.Read);
                                pdfReader = new PdfReader(stremfile);
                                System.IO.File.Delete(destinationPath);
                            }
                            FileStream signedPdf = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite);
                            PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0', null, true);
                            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                            signatureAppearance.Reason = "Digital Signature Reason";
                            signatureAppearance.Location = "Kota";
                            signatureAppearance.Acro6Layers = false;
                            float x = 430;
                            float y = 55;
                            signatureAppearance.Acro6Layers = false;
                            signatureAppearance.Layer4Text = PdfSignatureAppearance.questionMark;
                            signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + 150, y + 40), i, null);
                            string alias = pfxKeyStore.Aliases.Cast<string>().FirstOrDefault(entryAlias => pfxKeyStore.IsKeyEntry(entryAlias));
                            ICipherParameters privateKey = pfxKeyStore.GetKey(alias).Key;
                            IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
                            MakeSignature.SignDetached(signatureAppearance, pks, new Org.BouncyCastle.X509.X509Certificate[] { pfxKeyStore.GetCertificate(alias).Certificate }, null, null, null, 0, CryptoStandard.CMS);
                            pdfReader.Close();
                            pdfStamper.Close();
                            flag = true;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public ActionResult ValidateCertWithPasswordForSigning1(string selectedValue, string password)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("CertificateId", selectedValue),
                new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(password))),
                new DataItems("QueryType", "VaildatePassword")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);
            if (statusClass.StatusCode == 1)
            {
                return Json(new { status = "Validated", password });
            }
            else
            {
                return Json(new { status = "Invalid" });
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


        public JsonResult SearchAndShowDataForCertificateForSelection()
        {
            ResultDataForCertificate res1 = new ResultDataForCertificate();

            try
            {
                List<DSCCertificateMgt> result = new List<DSCCertificateMgt>();
                List<DataItems> obj = new List<DataItems>();
                string QueryType = "ShowCertificate";
                obj.Add(new DataItems("QueryType", QueryType));
                statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);

                if (statusClass.DataFetch != null && statusClass.DataFetch.Tables.Count > 0 && statusClass.DataFetch.Tables[0].Rows.Count > 0)
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
                            PasswordType = Convert.ToString(dr["PasswordType"]),
                            // UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                            IsActive1 = Convert.ToString(dr["IsActive"]),
                        });
                    }
                    res1.Table1 = result;
                }
                else
                {
                    res1.Error = "No data found."; // Set an appropriate error message    
                }
            }
            catch (Exception ex)
            {
                res1.Error = "An error occurred: " + ex.Message; // Handle the exception and set an error message
                                                                 // Log the exception for further analysis if needed
            }

            return Json(res1, JsonRequestBehavior.AllowGet);
        }




        public ActionResult SearchCertificateForPasswordPrompt(string CertificateID)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("CertificateId", CertificateID),
                new DataItems("QueryType", "SearchForPasswordPrompt")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_CertificateManagement, obj);
            var PATH = statusClass.DataFetch.Tables[0].Rows[0]["Path"];
            if (statusClass.StatusCode == 1)
            {
                return Json(new { status = "Prompt/Non", PATH });
            }
            else
            {
                return Json(new { status = "Invalid" });
            }
        }
        public ActionResult DSCSign(string selectedValue, string Location, string Reason, string password, string FilePath)
        {
            string UserName = Session["UserName"] as string;
            string timestamp = "";
            string TxnID = "61000" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            int ResponseCode = 0;
            string destinationPath = "";
            string LocalPath = "";
            string DSCCertificateName= Path.GetFileNameWithoutExtension(selectedValue);
            try
            {
                string baseDirectory = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"];
                string pdfFilePath = FilePath;  //"D:\\DSC\\dummy.pdf";
                string fileName = Path.GetFileNameWithoutExtension(pdfFilePath);
                timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                destinationPath = Path.Combine(baseDirectory, $"{fileName}_{timestamp}_signed.pdf");
                LocalPath = $"{fileName}_{timestamp}_signed.pdf";
                PdfReader pdfReader = new PdfReader(Server.MapPath(pdfFilePath));
                string pfxFilePath = selectedValue;  //"D:\\DSC\\PFXs\\Shiv Health.pfx";
                string pfxPassword = password;
                Pkcs12Store pfxKeyStore = new Pkcs12Store(new FileStream(pfxFilePath, FileMode.Open, FileAccess.Read), pfxPassword.ToCharArray());
                int page = pdfReader.NumberOfPages;
                for (int i = 1; i <= page; i++)
                {
                    if (i > 1)
                    {
                        FileStream stremfile = new FileStream(destinationPath, FileMode.Open, FileAccess.Read);
                        pdfReader = new PdfReader(stremfile);
                        // File.Delete(destinationPath);
                    }
                    FileStream signedPdf = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite);
                    PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf, '\0', null, true);
                    PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                    signatureAppearance.Reason = Reason;
                    signatureAppearance.Location = Location;
                    signatureAppearance.Acro6Layers = false;
                    float x = 430;
                    float y = 55;
                    signatureAppearance.Acro6Layers = false;
                    signatureAppearance.Layer4Text = PdfSignatureAppearance.questionMark;
                    signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + 150, y + 40), i, null);
                    string alias = pfxKeyStore.Aliases.Cast<string>().FirstOrDefault(entryAlias => pfxKeyStore.IsKeyEntry(entryAlias));
                    ICipherParameters privateKey = pfxKeyStore.GetKey(alias).Key;
                    IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
                    MakeSignature.SignDetached(signatureAppearance, pks, new Org.BouncyCastle.X509.X509Certificate[] { pfxKeyStore.GetCertificate(alias).Certificate }, null, null, null, 0, CryptoStandard.CMS);
                    pdfReader.Close();
                    pdfStamper.Close();
                }
                ResponseCode = 1;
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("UserName", UserName),
                    new DataItems("DSC_Certificate", DSCCertificateName),
                    new DataItems("ResponseCode", 1),
                    new DataItems("TxnId", TxnID),
                    new DataItems("ErrorMessage", "No_Error"),
                    new DataItems("TIMESTAMP", timestamp),
                    //new DataItems("UniqueSignerID", fileid),
                    //new DataItems("SignedPDFPath", destinationPath),
                    //obj.Add(new DataItems("DocumentUploadId", DocumentId));
                    new DataItems("QueryType", "SaveLogForDSC")
                };
                statusClass = bal.GetFunctionWithResult(pro.SignatureResponseLog, obj);
                return Json(LocalPath, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("UserName", UserName),
                    new DataItems("TimeStamp", timestamp),
                    new DataItems("DSC_Certificate", DSCCertificateName),
                    new DataItems("ResponseCode", 0),
                    new DataItems("TxnId", TxnID),
                    new DataItems("ErrorCode", ResponseCode),
                    new DataItems("ErrorMessage", ex.Message),
                    new DataItems("TIMESTAMP", timestamp),
                    //new DataItems("UniqueSignerID", fileid),
                    //new DataItems("SignedPDFPath", destinationPath),
                    //obj.Add(new DataItems("DocumentUploadId", DocumentId));
                    new DataItems("QueryType", "SaveLogForDSC")
                };
                statusClass = bal.GetFunctionWithResult(pro.SignatureResponseLog, obj);
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
    }
}