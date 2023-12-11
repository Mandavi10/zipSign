using BusinessLayerModel;
using Pkcs7pdf_Multiple_EsignService;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class VSignController : Controller
    {
        // GET: VSign   //
        public ActionResult Index()
        {
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
            string jarPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\JAR Files\\Verasays-eSign-Web-4.0.jar";
            string txtFilePath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\CoordinatesTXTFile\\Coordinatesfile.txt";
            int Coordinates = objModel.Coordinates;
            string ekycId = "";
            string aspId = "YSPLUAT001";
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
            string CoordinatesPath = System.Configuration.ConfigurationManager.AppSettings["ConsumePath"] + "Content\\CoordinatesTXTFile\\Coordinatesfile.txt";
            try
            {
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
    }
}