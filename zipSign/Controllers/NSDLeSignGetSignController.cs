using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkcs7pdf_Multiple_EsignService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml;

using System.Xml.Linq;


namespace zipSign.Controllers
{
    public class NSDLeSignGetSignController : ApiController
    {
        [HttpPost]
        public IHttpActionResult PDFSignature()
        {

            string XMLData = "";
            string UploadedFilePath = "";
            var BodyStrem = new StreamReader(HttpContext.Current.Request.InputStream);
            BodyStrem.BaseStream.Seek(0, SeekOrigin.Begin);
            string bodyText = BodyStrem.ReadToEnd();
            string[] separators = new string[] { "Path", "XMLData" };
            string[] parts = bodyText.Split(separators, StringSplitOptions.None);
            if (parts.Length > 1)
            {
                 XMLData = parts[1];
                UploadedFilePath = parts[2];
            }
            string XMLPATHYU= Path.GetFileNameWithoutExtension(UploadedFilePath);
            string xmlData = XMLData;
            XDocument xmlDoc1 = XDocument.Parse(xmlData);
            string PathToXML = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Uploads\\SignUpload\\" + XMLPATHYU + "_eSignRequestXml.txt";
            try
            {
                File.WriteAllText(PathToXML, xmlData);
                Console.WriteLine("XML data saved successfully as a text file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            _ = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + UploadedFilePath;
            string baseDirectory = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"];
            string filePath = UploadedFilePath; // Replace forward slashes with backslashes
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
                    string physicalPath = System.Web.HttpContext.Current.Server.MapPath(responsexmlPath);
                    File.WriteAllText(physicalPath, responseXml);
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
        }
            //    //string timeStamp = "";
            //    //ViewBag.RedirectUrl = redirectUrl;
            //    //return Redirect(redirectUrl);
            //    //return View();
            //}



        public class RequestModel
        {
            public string filePathfromUpload { get; set; }
            public string XMLFile { get; set; }
        }
    }
}
