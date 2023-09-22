using BusinessDataLayer;
using BusinessLayerModel;
using iTextSharp.text.pdf;
using Pkcs7pdf_Multiple_EsignService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace zipSign.Controllers
{
    public class NSDLController : Controller
    {
        private string redirectUrl = "";

        private readonly ProcMaster pro = new ProcMaster();
        private readonly NSDLGetSet nsdlget = new NSDLGetSet();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        public ActionResult NSDLPageAction(string xmlData)
        {
            ViewBag.XmlData = xmlData;
            return View();
        }
        public ActionResult PDFSignature(AuthViewModel objModel)
        {
            string TraceNumber = "612000" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            string documentid = objModel.UploadedDocumentId;
            _ = $"{System.Configuration.ConfigurationManager.AppSettings["ConsumePath"]}{objModel.File}";
            string baseDirectory = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"];
            string filePath = objModel.File.Replace('/', '\\').TrimStart('\\'); // Replace forward slashes with backslashes
            string pdfPath = Path.Combine(baseDirectory, filePath);
            string jarPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\JAR Files\\Runnable_eSign2.1_multiple_LogFile.jar";
            string txtFilePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt";
            int pageCount = GetPdfPageCount(pdfPath);
            int Coordinates = objModel.Coordinates;
            string ekycId = "";// Aadhar number token /UID ID 72 digit its optional field
            string aspId = "ASPYSPLMUMTEST223";
            string authMode = "1";
            _ = objModel.Fileid;
            string resp_url = $"http://localhost:50460/NSDL/Page_Load?filePathfromUpload={HttpUtility.UrlEncode(objModel.File)}";
            //string resp_url = $"https://uataadharsign.zipsign.in/NSDL/Page_Load?filePathfromUpload={HttpUtility.UrlEncode(objModel.File)}";
            string certificatePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\DSC_.p12\\YoekiDSC1.p12";
            string certificatePassward = "Creative0786!@#";
            string tickImagePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content/images/signbg.png";
            int serverTime = 15;
            string alias = "te-6b59446f-6d6d-416f-8624-858d73c61fd8";
            string nameToShowOnSignatureStamp = "Aadhar_E-sign"; /*objModel.SignerName;*/
            string locationToShowOnSignatureStamp = "";
            string reasonForSign = "";
            string pdfPassword = "";
            string txn = TraceNumber;
            string jrebinpath = "";
            string responsesigtype = "";
            int log_err = 1;
            string CoordinatesPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt";
            try
            {
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("TxnNo", txn),
                    new DataItems("SignerID", objModel.SignerID),
                    new DataItems("UploadedDocumentId", objModel.UploadedDocumentId),
                    new DataItems("QuerySelector", "UpdateTxnNo")
                };
                statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
                if (statusClass.StatusCode == 1)
                {
                    Coordinates = Convert.ToInt32(statusClass.DataFetch.Tables[0].Rows[0]["CoordinatesUpdate"]);
                }
                CreatePageSequenceFile(txtFilePath, pageCount, Coordinates, documentid, pdfPath, TraceNumber);

                PKCS7PDFMultiEsign req_resp = new PKCS7PDFMultiEsign();
                string req = req_resp.GenerateRequestXml(jarPath, ekycId, pdfPath, aspId, authMode, resp_url, certificatePath, certificatePassward, tickImagePath, serverTime, alias, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, txn, responsesigtype, CoordinatesPath, jrebinpath, log_err);
                string base_folder_path = Path.GetDirectoryName(pdfPath);
                string file_withoutExtn = Path.GetFileNameWithoutExtension(pdfPath);
                string request = file_withoutExtn + "_eSignRequestXml.txt";
                while (!System.IO.File.Exists(base_folder_path + "\\" + request))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                string xml_get = null;
                using (StreamReader sr = new StreamReader(base_folder_path + "\\" + request))
                {
                    xml_get = sr.ReadToEnd();
                }
                NameValueCollection collections = new NameValueCollection
                {
                    { "msg", xml_get }
                };
                string remoteUrl = "https://pregw.esign.egov-nsdl.com/nsdl-esp/authenticate/esign-doc/";
                string html = "<html><head><style>";
                html += "body {";
                html += "    margin: -10px 0 0 0;";
                html += "    padding: 0;";
                html += "    display: flex;";
                html += "    justify-content: center;";
                html += "    align-items: center;";
                html += "    height: 100vh;";
                html += "    background-color: #f0f0f0;";
                html += "}";
                html += ".loader-container {";
                html += "    display: flex;";
                html += "    justify-content: center;";
                html += "    align-items: center;";
                html += "    height: 100%;";
                html += "    margin-top: -10px;";
                html += "}";
                html += ".loader111 {";
                html += "    position: absolute;";
                html += "    border: 5px solid #f3f3f3;";
                html += "    border-radius: 50%;";
                html += "    border-top: 5px solid #3474b5;";
                html += "    width: 60px !important;";
                html += "    height: 60px !important;";
                html += "    -webkit-animation: spin 1.5s linear infinite !important;";
                html += "    animation: spin 1.5s linear infinite !important;";
                html += "}";
                html += "@-webkit-keyframes spin {";
                html += "    0% { -webkit-transform: rotate(0deg); }";
                html += "    100% { -webkit-transform: rotate(360deg); }";
                html += "}";
                html += "@keyframes spin {";
                html += "    0% { transform: rotate(0deg); }";
                html += "    100% { transform: rotate(360deg); }";
                html += "}";
                html += "</style>";
                html += "</head>";
                html += "<body onload='document.forms[0].submit()'>";
                html += string.Format("<form name='PostForm' method='POST' action='{0}' enctype='multipart/form-data'>", remoteUrl);
                foreach (string key in collections.Keys)
                {
                    html += string.Format("<input name='{0}' type='text' value='{1}'>", key, collections[key]);
                }
                html += "<div class='loader-container'>";
                html += "    <div class='loader111'></div>";
                html += "</div>";
                html += "</form>";
                html += "</body></html>";
                Response.Clear();
                Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1");
                Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
                Response.Charset = "ISO-8859-1";
                Response.Write(html);
                Response.End();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return View();
        }

        private static int GetPdfPageCount(string pdfPath)
        {
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            {
                return pdfReader.NumberOfPages;
            }

        }
        public JsonResult CreatePageSequenceFile(string txtFilePath, int pageCount, int Coordinates, string documentid, string pdfPath, string TraceNumber)
        {
            using (StreamWriter writer = new StreamWriter(txtFilePath))
            {
                if (Coordinates >= 0)
                {
                    for (int page = 1; page <= pageCount; page++)
                    {
                        int x;
                        //string fileExtn = Path.GetExtension(pdfPath);
                        if (pdfPath.Contains("_signedFinal.pdf"))
                        {
                            x = Coordinates + 140;
                            writer.Write($"{page}-{x},10,45,130;");
                        }
                        else
                        {
                            x = 10;
                            writer.Write($"{page}-{x},10,45,130;");
                        }
                        List<DataItems> obj = new List<DataItems>
                        {
                            new DataItems("CoordinatesUpdate", x),
                            new DataItems("UploadedDocumentId", documentid),
                            new DataItems("TxnNo", TraceNumber),
                            new DataItems("QuerySelector", "CoordinatesUpdate")
                        };
                        statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
                    }
                }
                return Json(new { }, JsonRequestBehavior.AllowGet);
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
        public ActionResult Page_Load(string filePathfromUpload)
        {
            _ = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + filePathfromUpload;
            string baseDirectory = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"];
            string filePath = filePathfromUpload.Replace('/', '\\').TrimStart('\\'); // Replace forward slashes with backslashes
            string pdfReadServerPath = Path.Combine(baseDirectory, filePath);

            string jarPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\JAR Files\\Runnable_eSign2.1_multiple_LogFile.jar";
            string tickImagePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content/images/signbg.png";
            int serverTime = 15;
            string nameToShowOnSignatureStamp = "Aadhar_E-sign";
            string locationToShowOnSignatureStamp = "";
            string reasonForSign = "";
            string pdfPassword = "";
            int log_err = 1;
            string pdfFolder = Path.GetDirectoryName(pdfReadServerPath);
            string PdfName = Path.GetFileNameWithoutExtension(pdfReadServerPath);

            string CoordinatesPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt";

            string jrebinpath = "";
            string outputFinalPdfPath = "" /*@"D:\Project\ZipSign_New\zipSign\zipSign\NSDL_Request_Response\NSDL_Final_SignedPDF\"*/;
            try
            {
                string responseXml = Request.Unvalidated["msg"].ToString();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseXml);
                XmlElement EsignResponse = xmlDoc.DocumentElement;
                string responsexmlPath = pdfFolder + "\\" + PdfName + "_responseXML.txt";
                PKCS7PDFMultiEsign response = new PKCS7PDFMultiEsign();
                if (EsignResponse.Attributes != null && EsignResponse.Attributes["status"].Value != "1")
                {
                    string req_flnm = pdfFolder + "\\" + PdfName + "_responseXML.txt";
                    string unsignedReqXmlpath = req_flnm;
                    string errorDetails = "errCode: " + EsignResponse.Attributes["errCode"].Value + " & Error Message: " + EsignResponse.Attributes["errMsg"].Value;
                    response.WriteLog("errCode: " + EsignResponse.Attributes["errCode"].Value + " & Error Message: " + EsignResponse.Attributes["errMsg"].Value, 1, pdfFolder);
                    string Responsexml = Path.Combine(pdfFolder + "\\" + PdfName + "_ERROR.txt");
                    System.IO.File.WriteAllText(unsignedReqXmlpath, responseXml);
                    System.IO.File.WriteAllText(Responsexml, errorDetails);
                    string LogFilePath = Responsexml;
                    string content = System.IO.File.ReadAllText(LogFilePath);
                    Match match = Regex.Match(content, @"errCode: ([^\s]+) & Error Message: (.+)");
                    if (match.Success)
                    {
                        string errorCode = match.Groups[1].Value;
                        string errorMessage = match.Groups[2].Value;
                        GetCertificateFromRequestWithCancel(unsignedReqXmlpath, errorCode, errorMessage);
                    }
                    if (System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt"))
                    {
                        System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt", string.Empty);
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_encryptTemp.pdf"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_encryptTemp.pdf");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignLog.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignLog.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_encryptTempSigned.pdf"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_encryptTempSigned.pdf");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignResponseXmllog.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignResponseXmllog.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_calTimeStamp.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_calTimeStamp.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_documentIds.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_documentIds.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_ERROR.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_ERROR.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignRequestXmlmethodentry.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignRequestXmlmethodentry.txt");
                    }
                    string sourceFilePatheSignRequestXml = Path.Combine(pdfFolder + "\\" + PdfName + "_eSignRequestXml.txt");
                    string sourceFilePathresponseXML = Path.Combine(pdfFolder, PdfName + "_responseXML.txt");
                    string destinationFolderPathres = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "NSDL_Request_Response\\Response\\";
                    string destinationFolderPathreq = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "NSDL_Request_Response\\Request\\";
                    if (System.IO.File.Exists(sourceFilePatheSignRequestXml))
                    {
                        string destinationFilePath_RequestXml = Path.Combine(destinationFolderPathreq, PdfName + "_eSignRequestXml.txt");
                        if (!System.IO.File.Exists(destinationFilePath_RequestXml))
                        {
                            System.IO.File.Move(sourceFilePatheSignRequestXml, destinationFilePath_RequestXml);
                        }
                    }
                    if (System.IO.File.Exists(sourceFilePathresponseXML))
                    {
                        string destinationFilePath_ResponeXml = Path.Combine(destinationFolderPathres, PdfName + "_responseXML.txt");
                        if (!System.IO.File.Exists(destinationFilePath_ResponeXml))
                        {
                            System.IO.File.Move(sourceFilePathresponseXML, destinationFilePath_ResponeXml);
                        }
                        else
                        {
                            Console.WriteLine("Destination file already exists. Rename or remove the existing file.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Source file does not exist.");
                    }
                    ViewBag.HasError = true;
                    return View();
                }
                else
                {
                    System.IO.File.WriteAllText(responsexmlPath, responseXml);
                    string rtn = response.SignDocument(pdfReadServerPath, jarPath, tickImagePath, responsexmlPath, serverTime, nameToShowOnSignatureStamp, locationToShowOnSignatureStamp, reasonForSign, pdfPassword, outputFinalPdfPath, CoordinatesPath, jrebinpath, log_err);
                }
                string signedPdfPath = "";
                if (outputFinalPdfPath == "")
                {
                    signedPdfPath = pdfFolder + "\\" + PdfName + "_signedFinal.pdf";
                }
                else
                {
                    signedPdfPath = outputFinalPdfPath + "\\" + PdfName + "_signedFinal.pdf";

                }

                string FilePath = "/Uploads/SignUpload/" + PdfName + "_signedFinal.pdf";

                while (!System.IO.File.Exists(signedPdfPath))
                {
                    // System.Threading.Thread.Sleep(1000);

                    if (System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt"))
                    {
                        System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt", string.Empty);
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_encryptTemp.pdf"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_encryptTemp.pdf");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignLog.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignLog.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_encryptTempSigned.pdf"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_encryptTempSigned.pdf");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignResponseXmllog.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignResponseXmllog.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_calTimeStamp.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_calTimeStamp.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_documentIds.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_documentIds.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_ERROR.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_ERROR.txt");
                    }
                    if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignRequestXmlmethodentry.txt"))
                    {
                        System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignRequestXmlmethodentry.txt");
                    }
                    string sourceFilePatheSignRequestXml = Path.Combine(signedPdfPath + "_eSignRequestXml.txt");
                    string sourceFilePathresponseXML = Path.Combine(signedPdfPath + "_responseXML.txt");
                    string destinationFolderPathres = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "\\NSDL_Request_Response\\Response\\";
                    string destinationFolderPathreq = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "\\NSDL_Request_Response\\Request\\";
                    if (System.IO.File.Exists(sourceFilePatheSignRequestXml))
                    {
                        string destinationFilePath_RequestXml = Path.Combine(destinationFolderPathreq, PdfName + "_eSignRequestXml.txt");
                        //string destinationFilePath_responseXML = Path.Combine(destinationFolderPathres, file_withoutExtn + "_responseXML.txt");
                        if (!System.IO.File.Exists(destinationFilePath_RequestXml))
                        {
                            System.IO.File.Move(sourceFilePatheSignRequestXml, destinationFilePath_RequestXml);
                        }
                    }
                    if (System.IO.File.Exists(sourceFilePathresponseXML))
                    {
                        string destinationFilePath_ResponeXml = Path.Combine(destinationFolderPathres, PdfName + "_responseXML.txt");
                        if (!System.IO.File.Exists(destinationFilePath_ResponeXml))
                        {
                            System.IO.File.Move(sourceFilePathresponseXML, destinationFilePath_ResponeXml);
                        }
                        else
                        {
                            Console.WriteLine("Destination file already exists. Rename or remove the existing file.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Source file does not exist.");
                    }
                }

                GetCertificateFromResponse(responsexmlPath, FilePath);

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            try
            {
                if (System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt"))
                {
                    System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Coordinatesfile.txt", string.Empty);
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_encryptTemp.pdf"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_encryptTemp.pdf");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignLog.txt"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignLog.txt");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_encryptTempSigned.pdf"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_encryptTempSigned.pdf");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignResponseXmllog.txt"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignResponseXmllog.txt");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_calTimeStamp.txt"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_calTimeStamp.txt");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_documentIds.txt"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_documentIds.txt");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_ERROR.txt"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_ERROR.txt");
                }
                if (System.IO.File.Exists(pdfFolder + "\\" + PdfName + "_eSignRequestXmlmethodentry.txt"))
                {
                    System.IO.File.Delete(pdfFolder + "\\" + PdfName + "_eSignRequestXmlmethodentry.txt");
                }
                string sourceFilePatheSignRequestXml = Path.Combine(pdfFolder + "\\" + PdfName + "_eSignRequestXml.txt");
                string sourceFilePathresponseXML = Path.Combine(pdfFolder, PdfName + "_responseXML.txt");

                string destinationFolderPathres = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "NSDL_Request_Response/Response/";
                string destinationFolderPathreq = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "NSDL_Request_Response/Request/";

                if (System.IO.File.Exists(sourceFilePatheSignRequestXml))
                {
                    string destinationFilePath_RequestXml = Path.Combine(destinationFolderPathreq, PdfName + "_eSignRequestXml.txt");
                    //string destinationFilePath_responseXML = Path.Combine(destinationFolderPathres, file_withoutExtn + "_responseXML.txt");
                    if (!System.IO.File.Exists(destinationFilePath_RequestXml))
                    {
                        System.IO.File.Move(sourceFilePatheSignRequestXml, destinationFilePath_RequestXml);
                    }
                }
                if (System.IO.File.Exists(sourceFilePathresponseXML))
                {
                    string destinationFilePath_ResponeXml = Path.Combine(destinationFolderPathres, PdfName + "_responseXML.txt");
                    if (!System.IO.File.Exists(destinationFilePath_ResponeXml))
                    {
                        System.IO.File.Move(sourceFilePathresponseXML, destinationFilePath_ResponeXml);
                    }
                    else
                    {
                        Console.WriteLine("Destination file already exists. Rename or remove the existing file.");
                    }
                }
                else
                {
                    Console.WriteLine("Source file does not exist.");
                }
            }
            catch (Exception ex)
            {
                PKCS7PDFMultiEsign req_resp = new PKCS7PDFMultiEsign();

                req_resp.WriteLog("file delete error: " + ex.Message, 1, pdfFolder);
            }
            //string timeStamp = "";
            ViewBag.RedirectUrl = redirectUrl;
            //return Redirect(redirectUrl);
            return View();
        }


        public string GetCertificateFromResponse(string xmlFilePath, string FilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            string temp = "";

            XmlNodeList elemList = doc.GetElementsByTagName("Signatures");
            for (int i = 0; i < elemList.Count; i++)
            {
                temp = elemList[i].InnerText;
            }
            string base64EncodedString = temp;// "SGVsbG8gV29ybGQh"; // Replace this with your actual Base64-encoded string
            // Convert the Base64-encoded string to bytes
            byte[] bytes = Convert.FromBase64String(base64EncodedString);
            // Convert the bytes back to the original string
            string originalString = System.Text.Encoding.UTF8.GetString(bytes);
            string searchPattern = "\u0003\u0013";
            string searchPatternForAadhar = "\f\u0013\u0004";
            int startIndexForAadhar = originalString.IndexOf(searchPatternForAadhar);
            int startIndex = originalString.IndexOf(searchPattern);
            string UserName = "";
            string timeStamp = "";
            string ResponseCode = "";
            string txnid = "";
            string errCode = "";
            string errMsg = "";
            string Status = "";
            string AadharNumber = "";
            if (startIndexForAadhar != -1)
            {
                startIndexForAadhar += searchPattern.Length;
                string endIndexforaadhar = "�\u0001";
                int endIndex = originalString.IndexOf(endIndexforaadhar, startIndexForAadhar);

                if (endIndex != -1)
                {
                    string extractedData = originalString.Substring(startIndexForAadhar, endIndex - startIndexForAadhar);
                    Console.WriteLine("Extracted Data: " + extractedData);

                    // Remove any non-numeric characters from the extracted data
                    string numericData = new string(extractedData.Where(char.IsDigit).ToArray());
                    string processedString = "";
                    if (extractedData.EndsWith("0"))
                    {
                        processedString = numericData.Substring(0, numericData.Length - 1);
                    }
                    AadharNumber = "XXXXXXXX" + processedString;
                    Console.WriteLine("Aadhar Number: " + processedString);
                }
                else
                {
                    Console.WriteLine("End pattern not found.");
                }
            }
            else
            {
                Console.WriteLine("Start pattern not found.");
            }
            if (startIndex != -1)
            {
                startIndex += searchPattern.Length;
                string endIndex1 = "\u00110";
                int endIndex = originalString.IndexOf(endIndex1, startIndex);

                if (endIndex != -1)
                {
                    string extractedData = originalString.Substring(startIndex, endIndex - startIndex);
                    Console.WriteLine("Extracted Data: " + extractedData);
                    string input = extractedData;
                    string processedString = "";
                    if (extractedData.EndsWith("1"))
                    {
                        processedString = extractedData.Substring(0, extractedData.Length - 1);
                    }
                    input = processedString;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (char.IsUpper(input[i]))
                        {
                            UserName = input.Substring(i);
                            break;
                        }
                    }
                    //string processedString1 = RemoveLowercaseAndUnicode(input);
                    Console.WriteLine("Original string: " + UserName);
                    //Console.WriteLine("Processed string: " + processedString1);
                }
                else
                {
                    Console.WriteLine("End pattern not found.");
                }
            }
            XmlNodeList esignRespNodes = doc.GetElementsByTagName("EsignResp");
            if (esignRespNodes.Count > 0)
            {
                XmlElement esignRespElement = (XmlElement)esignRespNodes[0];
                timeStamp = esignRespElement.GetAttribute("ts");
                ResponseCode = esignRespElement.GetAttribute("resCode");
                txnid = esignRespElement.GetAttribute("txn");
                errCode = esignRespElement.GetAttribute("errCode");
                errMsg = esignRespElement.GetAttribute("errMsg");
                Status = esignRespElement.GetAttribute("status");
            }
            else
            {
                Console.WriteLine("Start pattern not found.");
            }

            if (Status == "1")
            {
                string Email = "";
                string fileid = "";
                string SignerName = "";
                int SignerID = 0;
                string SignerAadhaar = "";
                string UploadedDocumentId = "";
                string TxnId = "";
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("UserName", UserName),
                    new DataItems("TimeStamp", timeStamp),
                    new DataItems("AadhaarNo", AadharNumber),
                    new DataItems("ResponseCode", ResponseCode),
                    new DataItems("TxnId", txnid),
                    new DataItems("ErrorCode", errCode),
                    new DataItems("ErrorMessage", errMsg),
                    new DataItems("Status", Status),
                    new DataItems("TxnNo", txnid),
                    new DataItems("UniqueSignerID", fileid),
                    new DataItems("SignedPDFPath", FilePath),
                    //obj.Add(new DataItems("DocumentUploadId", DocumentId));
                    new DataItems("QueryType", "SaveLog")
                };
                statusClass = bal.GetFunctionWithResult(pro.SignatureResponseLog, obj);
                if (statusClass.StatusCode == 1)
                {
                    string TimeStamp = "";
                    if (statusClass.DataFetch.Tables.Count > 0)
                    {
                        DataTable table = statusClass.DataFetch.Tables[0]; // Assuming you want to check the first table

                        if (table.Rows.Count == 0)
                        {
                            //redirectUrl = FilePath;
                            SignerID = Convert.ToInt32(statusClass.DataFetch.Tables[1].Rows[0]["SignerID"]);
                            SignerName = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["SignerName"]);
                            Email = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["SignerEmail"]);
                            UploadedDocumentId = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["UploadedDocumentId"]);
                            SendEmailAfterSuccess(Email, SignerName, SignerID, FilePath, UploadedDocumentId);
                            //redirectUrl = FilePath + "&TxnId=" + TxnId + "&Date=" + TimeStamp;
                        }
                        else
                        {
                            SignerID = Convert.ToInt32(statusClass.DataFetch.Tables[0].Rows[0]["SignerID"]);
                            SignerName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerName"]);
                            Email = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerEmail"]);
                            UploadedDocumentId = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UploadedDocumentId"]);
                            TxnId = Convert.ToString(statusClass.DataFetch.Tables[2].Rows[0]["TxnId"]);
                            TimeStamp = Convert.ToString(statusClass.DataFetch.Tables[2].Rows[0]["TimeStamp"]);
                            //SignerName = Convert.ToString(statusClass.DataFetch.Tables[2].Rows[0]["UserName"]);
                            SignerAadhaar = Convert.ToString(statusClass.DataFetch.Tables[2].Rows[0]["AadhaarNo"]);
                            int SignerID1 = Convert.ToInt32(statusClass.DataFetch.Tables[1].Rows[0]["SignerID"]);
                            string SignerName1 = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["SignerName"]);
                            string Email1 = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["SignerEmail"]);
                            string UploadedDocumentId1 = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["UploadedDocumentId"]);
                            string TxnId1 = Convert.ToString(statusClass.DataFetch.Tables[2].Rows[0]["TxnId"]);
                            //string TimeStamp1 = Convert.ToString(statusClass.DataFetch.Tables[].Rows[0]["TimeStamp"]);
                            // string SignerName2 = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["UserName"]);
                            string SignerAadhaar1 = Convert.ToString(statusClass.DataFetch.Tables[2].Rows[0]["AadhaarNo"]);
                            redirectUrl = FilePath + "&TxnId=" + TxnId + "&Date=" + TimeStamp;
                            SendVerifyLinkByEmail(Email, fileid, SignerName, SignerID, FilePath, UploadedDocumentId);
                            SendEmailAfterSuccess(Email1, SignerName1, SignerID1, FilePath, UploadedDocumentId1);
                        }
                    }
                }
                else
                {
                    SignerName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UserName"]);
                    SignerAadhaar = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["AadhaarNo"]);
                    TxnId = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["TxnId"]);
                    string TimeStamp = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["TimeStamp"]);
                    redirectUrl = FilePath + "&TxnId=" + TxnId + "&Date=" + TimeStamp;
                    //redirectUrl = FilePath + "/" + SignerName + "/" + SignerAadhaar + "/" + TxnId+"/"+TimeStamp;
                }

                var result1 = new
                {
                    status = statusClass.StatusCode
                };

            }
            return redirectUrl;
        }
        public void GetCertificateFromRequestWithCancel(string xmlFilePath, string errorCode, string errorMessage)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);
            string temp = "";

            XmlNodeList elemList = doc.GetElementsByTagName("SignatureValue");
            for (int i = 0; i < elemList.Count; i++)
            {
                temp = elemList[i].InnerText;
            }
            string base64EncodedString = temp;
            byte[] bytes = Convert.FromBase64String(base64EncodedString);
            // Convert the bytes back to the original string
            string originalString = System.Text.Encoding.UTF8.GetString(bytes);
            string searchPattern = "\u0003\u0013";

            int startIndex = originalString.IndexOf(searchPattern);
            string UserName = "";
            string timeStamp = "";
            string ResponseCode = "";
            string txnid = "";
            string Status = "";
            if (startIndex != -1)
            {
                startIndex += searchPattern.Length;
                string endIndex1 = "\u00110";
                int endIndex = originalString.IndexOf(endIndex1, startIndex);

                if (endIndex != -1)
                {
                    string extractedData = originalString.Substring(startIndex, endIndex - startIndex);
                    Console.WriteLine("Extracted Data: " + extractedData);
                    string processedString = "";
                    if (extractedData.EndsWith("1"))
                    {
                        processedString = extractedData.Substring(0, extractedData.Length - 1);
                    }
                    string input = processedString;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (char.IsUpper(input[i]))
                        {
                            UserName = input.Substring(i);
                            break;
                        }
                    }
                    //string processedString1 = RemoveLowercaseAndUnicode(input);
                    Console.WriteLine("Original string: " + UserName);
                    //Console.WriteLine("Processed string: " + processedString1);
                }
                else
                {
                    Console.WriteLine("End pattern not found.");
                }
            }
            XmlNodeList esignRespNodes = doc.GetElementsByTagName("EsignResp");
            if (esignRespNodes.Count > 0)
            {
                XmlElement esignRespElement = (XmlElement)esignRespNodes[0];
                timeStamp = esignRespElement.GetAttribute("ts");
                ResponseCode = esignRespElement.GetAttribute("resCode");
                txnid = esignRespElement.GetAttribute("txn");
                Status = esignRespElement.GetAttribute("status");
            }
            else
            {
                Console.WriteLine("Start pattern not found.");
            }
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("UserName", UserName),
                new DataItems("TimeStamp", timeStamp),
                new DataItems("ResponseCode", ResponseCode),
                new DataItems("TxnId", txnid),
                new DataItems("ErrorCode", errorCode),
                new DataItems("ErrorMessage", errorMessage),
                new DataItems("Status", Status),
                new DataItems("QueryType", "SaveLog")
            };
            statusClass = bal.PostFunction(pro.SignatureResponseLog, obj);
            _ = new
            {
                status = statusClass.StatusCode
            };
        }
        public JsonResult SendVerifyLinkByEmail(string Email, string fileid, string SignerName, int SignerID, string FilePath, string UploadedDocumentId)
        {
            string FileName = Path.GetFileNameWithoutExtension(FilePath);
            string OriginalSignerName = SignerName;
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
                msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
                Email = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Email));
                fileid = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(fileid));
                SignerName = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(SignerName));
                FilePath = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(FilePath));
                Guid uniqueIdentifier = Guid.NewGuid();
                // Store the mapping between the identifier and the parameters in your database
                StoreMappingInDatabase(uniqueIdentifier, Email, fileid, SignerName, SignerID, FilePath, UploadedDocumentId);
                msg.Subject = "Invitation to Electronically Sign a Document";
                string mss = "http://localhost:50460/Login/SignLogin";
                string urlWithEncodedFileId = $"{mss}?UId={uniqueIdentifier}";

                string message = $@"
<!DOCTYPE html>
<html>
<head>
<style>
  body {{
    font-family: Arial, sans-serif;
    margin: 0;
    padding: 0;
    background-color: #f5f5f5;
    text-align: left;
  }}
  
  .container {{
    max-width: 600px;
    background-color: #fff;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    text-align: left; /* Align content to the left within the container */
    padding: 20px;
  }}
  
  h1 {{
    color: #007BFF;
    font-size: 24px;
    margin-bottom: 20px;
  }}
  
  p {{
    font-size: 16px;
    line-height: 1.5;
    margin-bottom: 20px;
  }}
  
  a {{
    color: #007BFF;
    text-decoration: none;
  }}
  
  .disclaimer {{
    color: #999;
    font-size: 12px;
    margin-top: 20px;
  }}
</style>
</head>
<body>
<div class='container'>
  <h1>Dear {OriginalSignerName},</h1>
  <p>You've received an invitation from Team zipSign to electronically sign the document:</p>
  <p><strong>{FileName}</strong></p>
  <p>To accept the invitation and sign the document, please click on the following link:</p>
  <p><a href='{urlWithEncodedFileId}'>Accept Invitation</a></p>
  <p>If you encounter any issues or have any questions, feel free to contact our support team at <a href='mailto:customersupport@zipsign.com'>customersupport@zipsign.com</a>.</p>
  <p>Thank you for choosing zipSign!</p>
  <div class='disclaimer'>
    <p><em>This is an automated message. Please do not reply to this email.</em></p>
  </div>
</div>
</body>
</html>";
                try
                {
                    msg.Body = message;
                    msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        EnableSsl = true
                    };
                    NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = networkCredential;
                    smtp.Port = 587;
                    smtp.Send(msg);
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
                return Json("");
            }
        }

        public JsonResult SendEmailAfterSuccess(string Email, string SignerName, int SignerID, string FilePath, string UploadedDocumentId)
        {
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
                msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
                msg.Subject = "Document Sign Confirmation";

                string messageWithExpiration = $"Dear {SignerName},\n\n";
                messageWithExpiration += "Thank you for signing the document.\n\n";
                messageWithExpiration += "You can download the document from the attached link.\n\n";

                string baseUrl = "http://localhost:50460/zipSign/SigningRequest";
                string urlWithEncodedFileId = $"{baseUrl}?FilePath={FilePath}";

                string hiddenLink = $"<a href=\"{urlWithEncodedFileId}\" style=\"display: none;\">Download the document</a>";
                string visibleLink = "<span style=\"color: transparent; font-size: 0;\">Download the document</span>";

                string supportEmail = "support@zipsign.com";

                string disclaimer = "\n\n---\n\n";
                disclaimer += "This is an automated message. Please do not reply to this email.";

                msg.Body = $"{messageWithExpiration}{hiddenLink} {visibleLink}\n\n";
                msg.Body += $"If you encounter any issues or have any questions, please do not hesitate to contact our support team at {supportEmail}.\n\n";
                msg.Body += "Regards,\nTeam zipSign\n\n{disclaimer}";

                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                smtp.UseDefaultCredentials = false; // Set to false to use network credentials
                smtp.Credentials = networkCredential;
                smtp.Port = 587;

                smtp.Send(msg);
            }
            redirectUrl = FilePath;
            return Json("");
        }




        public JsonResult GetSignerData(string UploadedDocumentId)
        {
            string UploadedDocument = AESEncryption.AESEncryptionClass.DecryptAES(UploadedDocumentId);

            //SignMaster Sign = new SignMaster();

            List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("UploadedDocumentId", UploadedDocument),
                new DataItems("QuerySelector", "CoordinatesCheck")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
            //  string CoordinatesUpdate = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["CoordinatesUpdate"]);
            return Json(new { UploadedDocument }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocument(string UploadedDocumentId, string SignerID)
        {
            string UploadedDocument = AESEncryption.AESEncryptionClass.DecryptAES(UploadedDocumentId);
            //string SignerId = AESEncryption.AESEncryptionClass.DecryptAES(SignerID);

            SignMaster result = new SignMaster();

            List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("UploadedDocumentId", UploadedDocument),
                new DataItems("SignerID", SignerID),
                new DataItems("QuerySelector", "UpdateDocInfo")
            };
            statusClass = bal.PostFunction(pro.Sp_SignUpload, obj1);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDocumentAllData(string Link)
        {
            List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("UniqueIdentifier", Link),
                new DataItems("QuerySelector", "GetDocument")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
            string EmailID = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"]));
            return Json(new { EmailID }, JsonRequestBehavior.AllowGet);
        }
        private void StoreMappingInDatabase(Guid uniqueIdentifier, string email, string fileId, string signerName, int signerId, string filePath, string uploadedDocumentId)
        {
            string connectionString = GlobalMethods.Global.DocSign.ToString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO LinkMappings (UniqueIdentifier, Email, FileId, SignerName, SignerId, FilePath, UploadedDocumentId,CreatedOn) VALUES (@UniqueIdentifier, @Email, @FileId, @SignerName, @SignerId, @FilePath, @UploadedDocumentId,@CreatedOn)", connection))
                {
                    command.Parameters.AddWithValue("@UniqueIdentifier", uniqueIdentifier);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@FileId", fileId);
                    command.Parameters.AddWithValue("@SignerName", signerName);
                    command.Parameters.AddWithValue("@SignerId", signerId);
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    command.Parameters.AddWithValue("@UploadedDocumentId", uploadedDocumentId);
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    //command.Parameters.AddWithValue("@UploadedDocumentId", uploadedDocumentId);
                    //command.Parameters.AddWithValue("@UploadedDocumentId", uploadedDocumentId);
                    //command.Parameters.AddWithValue("@UploadedDocumentId", uploadedDocumentId);
                    int I = command.ExecuteNonQuery();
                }
            }
        }
        public JsonResult GetDocumentAllData1(string Link)
        {
            List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("UniqueIdentifier", Link),
                new DataItems("QuerySelector", "GetDocument")
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
            string EmailID = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"]));
            string FilePath = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["FilePath"]));
            string FileID = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["FileID"]));
            string SignerName = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerName"]));
            string UploadedDocumentId = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UploadedDocumentId"]);
            string SignerId = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerId"]);
            string UploadedFileName = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["UploadedFileName"]);
            _ = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["DocumentUploadId"]);
            string UploadedOn = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["UploadedOn"]);
            return Json(new { UploadedDocumentId, SignerName, FileID, FilePath, EmailID, SignerId, UploadedFileName, UploadedOn }, JsonRequestBehavior.AllowGet);
        }
    }

}
