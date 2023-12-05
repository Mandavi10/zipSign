using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}