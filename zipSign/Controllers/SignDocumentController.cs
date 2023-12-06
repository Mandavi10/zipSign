using System;
using System.IO;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
        public ActionResult NewDoc(string filePath)
        {
            string destFilePath = GenerateDynamicPDF(filePath);
            return Json(new { destFilePath }, JsonRequestBehavior.AllowGet);
        }

        //public string GenerateDynamicPDF(string originalFilePath)
        //{
        //    string OridinalPath = Server.MapPath(originalFilePath);
        //    string file_withoutExtn = Path.GetFileNameWithoutExtension(originalFilePath);
        //    string WithoutExtension = file_withoutExtn + "123.pdf";
        //    using (var fs = new FileStream(OridinalPath, FileMode.Open, FileAccess.Read))
        //    {
        //        var destFilePath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"], WithoutExtension);

        //        var pdfReader = new PdfReader(fs);
        //        using (var pdfStamper = new PdfStamper(pdfReader, new FileStream(destFilePath, FileMode.Create)))
        //        {
        //            int page = pdfReader.NumberOfPages;

        //            for (int i = 1; i <= page; i++)
        //            {
        //                // Use GetOverContent() within the loop for each page
        //                PdfContentByte contentByte = pdfStamper.GetOverContent(i);

        //                // Check if AcroForm is present
        //                if (pdfStamper.AcroFields != null)
        //                {
        //                    string signerName = "Abhishek";

        //                    // Set field values directly if there's an AcroForm
        //                    pdfStamper.AcroFields.SetField("Reason", signerName);
        //                    pdfStamper.AcroFields.SetField("Location", "Kota, Rajasthan");

        //                    float x = 430;
        //                    float y = 55;
        //                    float width = 150;
        //                    float height = 40;

        //                    // Example: Add a signature field
        //                    contentByte.SetColorFill(BaseColor.BLACK);
        //                    contentByte.SetFontAndSize(BaseFont.CreateFont(), 12);
        //                    contentByte.BeginText();
        //                    contentByte.ShowTextAligned(Element.ALIGN_LEFT, signerName, x, y + height / 2, 0);
        //                    contentByte.EndText();

        //                    // SetVisibleSignature() is optional if you're manually placing the signature field
        //                    pdfStamper.AddSignature("SignatureFieldName", i, x, y, x + width, y + height);
        //                }
        //            }

        //            pdfStamper.Close();
        //            pdfReader.Close();

        //            return destFilePath;
        //        }
        //    }
        //}
        //public string GenerateDynamicPDF(string originalFilePath)
        //{
        //    string originalPath = Server.MapPath(originalFilePath);
        //    string fileWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
        //    string destinationFileName = fileWithoutExtension + "123.pdf";

        //    using (var fs = new FileStream(originalPath, FileMode.Open, FileAccess.Read))
        //    {
        //        var destFilePath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"], destinationFileName);

        //        var pdfReader = new PdfReader(fs);

        //        using (var pdfStamper = new PdfStamper(pdfReader, new FileStream(destFilePath, FileMode.Create)))
        //        {
        //            int totalPages = pdfReader.NumberOfPages;

        //            for (int currentPage = 1; currentPage <= totalPages; currentPage++)
        //            {
        //                // Use GetOverContent() within the loop for each page
        //                PdfContentByte contentByte = pdfStamper.GetOverContent(currentPage);

        //                // Check if AcroForm is present
        //                if (pdfStamper.AcroFields != null)
        //                {
        //                    string signerName = "Abhishek";

        //                    // Set field values directly if there's an AcroForm
        //                    pdfStamper.AcroFields.SetField("Reason", signerName);
        //                    pdfStamper.AcroFields.SetField("Location", "Kota, Rajasthan");

        //                    float x = 430;
        //                    float y = 55;
        //                    float width = 150;
        //                    float height = 40;

        //                    // Example: Add a signature field
        //                    contentByte.SetColorFill(BaseColor.BLACK);

        //                    // Specify a font and size (replace with your actual font)
        //                    BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        //                    contentByte.SetFontAndSize(baseFont, 12);

        //                    contentByte.BeginText();
        //                    contentByte.ShowTextAligned(Element.ALIGN_LEFT, signerName, x, y + height / 2, 0);
        //                    contentByte.EndText();

        //                    // SetVisibleSignature() is optional if you're manually placing the signature field
        //                    pdfStamper.AddSignature("SignatureFieldName", currentPage, x, y, x + width, y + height);
        //                }
        //            }
        //            pdfStamper.Close();
        //            pdfReader.Close();
        //            return destFilePath;
        //        }
        //    }
        //}
        public string GenerateDynamicPDF(string originalFilePath)
        {
            //DateTime a = new DateTime();
            DateTime CurrentDate = DateTime.Now;
            string originalPath = Server.MapPath(originalFilePath);
            string fileWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
            string withoutExtension = fileWithoutExtension + "123.pdf";

            using (var fs = new FileStream(originalPath, FileMode.Open, FileAccess.Read))
            {
                var destFilePath = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["ConsumePath"], withoutExtension);

                var pdfReader = new PdfReader(fs);
                using (var pdfStamper = new PdfStamper(pdfReader, new FileStream(destFilePath, FileMode.Create)))
                {
                    int page = pdfReader.NumberOfPages;

                    for (int i = 1; i <= page; i++)
                    {
                        PdfContentByte contentByte = pdfStamper.GetOverContent(i);

                        //if (pdfStamper.AcroFields != null)
                        //{
                        //    //string signerName = "Name: Abhishek" + "\n" +
                        //    //           "Location: Delhi" + "\n" +
                        //    //           "Date: " + CurrentDate;
                        //    string signerName = "{\\rtf1\\ansi Name: Abhishek\\par Location: Delhi\\par Date: " + CurrentDate.ToString("yyyy-MM-dd") + "\\par}";

                        //    //string location = "Kota, Rajasthan";
                        //    pdfStamper.AcroFields.SetField("Reason", signerName + CurrentDate);
                        //    pdfStamper.AcroFields.SetField("Location", "Kota, Rajasthan");

                        //    float x = 450; // Adjust this value based on your desired position
                        //    float y = 160; // Adjust this value based on your desired position

                        //    // Draw signer name directly on the page without using contentByte.AddSignature
                        //    contentByte.SetColorFill(BaseColor.BLACK);
                        //    contentByte.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED), 12);
                        //    contentByte.BeginText();
                        //    contentByte.ShowTextAligned(Element.ALIGN_LEFT, signerName, x, y, 0);
                        //    contentByte.EndText();
                        //}
                        string signerName = "Name: Abhishek\nLocation: Delhi\nDate: " + CurrentDate.ToString("yyyy-MM-dd");

                        if (pdfStamper.AcroFields != null)
                        {
                            pdfStamper.AcroFields.SetField("Reason", signerName);
                            pdfStamper.AcroFields.SetField("Location", "Kota, Rajasthan");

                            float x = 450; // Adjust this value based on your desired position
                            float y = 160; // Adjust this value based on your desired position

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

                    return destFilePath;
                }
            }
        }
    }
}
