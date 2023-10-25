using System;
using System.IO;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TransactionDetails()
        {
            return View();
        }
        [Authorize]
        public string DeleteOldFiles(int CustomDays)
        {
            DirectoryInfo yourRootDir = new DirectoryInfo(Server.MapPath(@"\Uploads\SignUpload"));
            foreach (FileInfo file in yourRootDir.GetFiles())
            {
                if (file.LastWriteTime < DateTime.Now.AddDays(-CustomDays))
                {
                    file.Delete();
                }
            }
            return "File Deletion Successfully";
        }
    }
}