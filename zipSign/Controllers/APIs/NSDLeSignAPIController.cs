using BusinessDataLayer;
using BusinessLayerModel;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using Pkcs7pdf_Multiple_EsignService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class NSDLeSignAPIController : ApiController
    {
        public ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
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
            //string resp_url = $"http://10.10.10.135//NSDL/Page_Load?filePathfromUpload={HttpUtility.UrlEncode(Data.File)}";
            string resp_url = $"https://uataadharsign.zipsign.in/NSDL/Page_Load?filePathfromUpload={HttpUtility.UrlEncode(Data.File)}";
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
                var ApiResponseModel = new
                {
                    status = true,
                    message = "XML Generated Successfully.",
                    XMLData = xml_get,
                    File = Data.File
                };

                return Json(ApiResponseModel);
            }
            catch (Exception ex)
            {
                var ApiResponseModel = new
                {
                    status = false,
                    message = "XML Generation Failed.",
                    Error = ex
                };

                return Json(ApiResponseModel);
            }

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
    }
}
