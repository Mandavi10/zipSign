using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class MySignsController : Controller
    {
        // GET: MySigns
        public ActionResult SignUses()
        {
            return View();
        }
        public ActionResult SignPricingMechanism()
        {
            return View();
        }
        public ActionResult SignsPricing()
        {
            return View();
        }
    }
}