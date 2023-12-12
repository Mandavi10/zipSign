using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class SignDocumentController : Controller
    {
        // GET: SignDocument
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DocumentSignRequest()
        {
            return View();
        }
        public ActionResult NewDoc()       
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewDoc(string filePath, string UserName, string Location)
        {
            string destFilePath = GenerateDynamicPDF(filePath, UserName, Location);
            return Json(new { destFilePath }, JsonRequestBehavior.AllowGet);
        }
        public string GenerateDynamicPDF(string originalFilePath, string UserName, string Location)
        {
            string originalPath = Server.MapPath(originalFilePath);
            string fileWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
            string withoutExtension = "Uploads\\SignUpload\\" + fileWithoutExtension + "_Signed_By_National_ID.pdf";

            using (FileStream fs = new FileStream(originalPath, FileMode.Open, FileAccess.Read))
            {
                string destFilePath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"], withoutExtension);

                PdfReader pdfReader = new PdfReader(fs);
                using (PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(destFilePath, FileMode.Create)))
                {
                    int page = pdfReader.NumberOfPages;

                    for (int i = 1; i <= page; i++)
                    {
                        PdfContentByte contentByte = pdfStamper.GetOverContent(i);
                        string signerName = $"Digitally Signed By:\nName: {UserName}\nLocation:{Location}\nDate: {DateTime.Now:yyyy-MM-dd}";
                        if (pdfStamper.AcroFields != null)
                        {
                            pdfStamper.AcroFields.SetField("Reason", signerName);
                            pdfStamper.AcroFields.SetField("Location", "Kota, Rajasthan");

                            float x = 470; 
                            float y = 150;

                            contentByte.SetColorFill(BaseColor.BLACK);
                            contentByte.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED), 12);
                            contentByte.BeginText();
                            // Use Phrase to handle line breaks            
                            Phrase phrase = new Phrase(signerName, new Font(Font.FontFamily.HELVETICA, 12));
                            ColumnText columnText = new ColumnText(contentByte);
                            columnText.SetSimpleColumn(phrase, x, y, 550, 0, 12, Element.ALIGN_LEFT);
                            columnText.Go();
                            contentByte.EndText();
                        }

                    }

                    pdfStamper.Close();
                    pdfReader.Close();

                    return withoutExtension;
                }
            }
        }
    }
}
