﻿using BusinessDataLayer;
using BusinessLayerModel;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using Pkcs7pdf_Multiple_EsignService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;

namespace zipSign.Controllers.APIs
{
    public class NSDLeSignAPIController : ApiController
    {
        public ProcMaster pro = new ProcMaster();
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpGet]
        public IHttpActionResult GenerateRequestXML([FromBody] JObject requestData)
        {
            string xml_get = null;
            AuthViewModel Data = requestData["Data"].ToObject<AuthViewModel>();
            string TraceNumber = "612000" + DateTime.Now.ToString("ddMMyyyyHHmmss");
            string documentid = Data.UploadedDocumentId;
            _ = $"{System.Configuration.ConfigurationManager.AppSettings["ConsumePath"]}{Data.File}";
            string baseDirectory = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"];
            string filePath = Data.File.Replace('/', '\\').TrimStart('\\'); // Replace forward slashes with backslashes
            string pdfPath = Path.Combine(baseDirectory, filePath);
            string jarPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\JAR Files\\Runnable_eSign2.1_multiple_LogFile.jar";
            string txtFilePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\CoordinatesTXTFile\\Coordinatesfile.txt";
            int pageCount = GetPdfPageCount(pdfPath);
            int Coordinates = Data.Coordinates;
            string ekycId = "";
            string aspId = "ASPYSPLMUMTEST223";
            string authMode = "1";
            _ = Data.Fileid;
            string resp_url = $"http://localhost:50460/NSDL/Page_Load?filePathfromUpload={HttpUtility.UrlEncode(Data.File)}";
            //string resp_url = $"https://uataadharsign.zipsign.in/NSDL/Page_Load?filePathfromUpload={HttpUtility.UrlEncode(Data.File)}";
            string certificatePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\DSC_.p12\\YoekiDSC1.p12";
            string certificatePassward = "Creative0786!@#";
            string tickImagePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content/images/signbg.png";
            int serverTime = 15;
            string alias = "te-6b59446f-6d6d-416f-8624-858d73c61fd8";
            string nameToShowOnSignatureStamp = "Aadhar_E-sign"; /*Data.SignerName;*/
            string locationToShowOnSignatureStamp = "";
            string reasonForSign = "";
            string pdfPassword = "";
            string txn = TraceNumber;
            string jrebinpath = "";
            string responsesigtype = "";
            int log_err = 1;
            string CoordinatesPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\CoordinatesTXTFile\\Coordinatesfile.txt";
            try
            {
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("TxnNo", txn),
                    new DataItems("SignerID", Data.SignerID),
                    new DataItems("UploadedDocumentId", Data.UploadedDocumentId),
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
                while (!File.Exists(base_folder_path + "\\" + request))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                
                using (StreamReader sr = new StreamReader(base_folder_path + "\\" + request))
                {
                    xml_get = sr.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                
                ex.Message.ToString();
            }
            var ApiResponseModel = new
            {
                status = true,
                message = "XML Generated Successfully.",
                XMLData = xml_get,
                File= Data.File
            };
            return Json(ApiResponseModel);
        }
        private static int GetPdfPageCount(string pdfPath)
        {
            using (PdfReader pdfReader = new PdfReader(pdfPath))
            {
                return pdfReader.NumberOfPages;
            }
        }
        public IHttpActionResult CreatePageSequenceFile(string txtFilePath, int pageCount, int Coordinates, string documentid, string pdfPath, string TraceNumber)
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
                            writer.Write($"{page}-{10},10,45,130;");
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
                var response = new
                {
                    status = false,
                    message = "Failed to retrieve states data."
                };
                return Json(response);
            }
        }

        public class ApiResponseModel
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
        }

        [HttpPost]
        public IHttpActionResult PDFSignature(RequestModel model)
        {
            string xmlData = model.XMLFile;
            string PathToXML = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + model.filePathfromUpload+ "_eSignRequestXml.txt";
            try
            {
                // Save XML data to a text file with .txt extension
                File.WriteAllText(PathToXML, xmlData);

                Console.WriteLine("XML data saved successfully as a text file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            _ = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + model.filePathfromUpload;
            string baseDirectory = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"];
            string filePath = model.filePathfromUpload.Replace('/', '\\').TrimStart('\\'); // Replace forward slashes with backslashes
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
            string CoordinatesPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\CoordinatesTXTFile\\Coordinatesfile.txt";
            string jrebinpath = "";
            string outputFinalPdfPath = "" /*@"D:\Project\ZipSign_New\zipSign\zipSign\NSDL_Request_Response\NSDL_Final_SignedPDF\"*/;
            try
            {
                string responseXml;
                using (StreamReader reader = new StreamReader(PathToXML))
                {
                    responseXml = reader.ReadToEnd();
                }
                //string responseXml = Request.Unvalidated["msg"].ToString();
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
                        //GetCertificateFromRequestWithCancel(unsignedReqXmlpath, errorCode, errorMessage);
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
                            // //Console.WriteLine("Destination file already exists. Rename or remove the existing file.");
                        }
                    }
                    else
                    {
                        ////Console.WriteLine("Source file does not exist.");
                    }
                    //ViewBag.HasError = true;
                    //return View();
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
                            ////Console.WriteLine("Destination file already exists. Rename or remove the existing file.");
                        }
                    }
                    else
                    {
                        ////Console.WriteLine("Source file does not exist.");
                    }
                }

                //GetCertificateFromResponse(responsexmlPath, FilePath);

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
                        //Console.WriteLine("Destination file already exists. Rename or remove the existing file.");
                    }
                }
                else
                {
                    //Console.WriteLine("Source file does not exist.");
                }
            }
            catch (Exception ex)
            {
                PKCS7PDFMultiEsign req_resp = new PKCS7PDFMultiEsign();

                req_resp.WriteLog("file delete error: " + ex.Message, 1, pdfFolder);
            }

            var ApiResponseModel = new
            {
                status = true,
                message = "File Signed Successfully.",
                Data = ""
            };
            return Json(ApiResponseModel);
            //string timeStamp = "";
            //ViewBag.RedirectUrl = redirectUrl;
            //return Redirect(redirectUrl);
            //return View();
        }




        public class RequestModel
        {
            public string filePathfromUpload { get; set; }
            public string XMLFile { get; set; }
        }



    }
}
