using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class CertificateManagementController : Controller
    {
        // GET: CertificateManagement
        public ActionResult CertManagement()
        {
            return View();
        }
            public ActionResult AllDocumentSignerCertificate()
            {
                return View();
            }
    
    }
}