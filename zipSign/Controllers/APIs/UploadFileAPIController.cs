using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class UploadFileAPIController : ApiController
    {
        public IHttpActionResult UploadFiles()
        {
            _ = new List<DataItems>();

            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpPostedFile file = httpRequest.Files["HelpSectionImages"];
                if (file != null && file.ContentLength > 0)
                {
                    string randomKey = CreateRandomKey();
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    string[] allowedExtensions = new[] { ".pdf" };
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("Invalid file format. Only PDF files are allowed.");
                    }

                    int maxFileSize = 10 * 1024 * 1024; // 10 MB
                    if (file.ContentLength > maxFileSize)
                    {
                        return BadRequest("File size exceeds the maximum allowed limit.");
                    }

                    string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string fileName = $"{originalFileName}_{timestamp}_{randomKey}{fileExtension}";
                    string localFilePath = "/Uploads/SignUpload/" + fileName;
                    string filePath = HttpContext.Current.Server.MapPath("~/Uploads/SignUpload/" + fileName);

                    try
                    {
                        file.SaveAs(filePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                        return InternalServerError(new Exception("Error saving the file: " + ex.Message));
                    }

                    return Ok(new { status=true,FilePath = filePath, uniquefileName = fileName });
                }

                return BadRequest("No file received.");
            }

            return BadRequest("No files received in the request.");
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

    }
}
