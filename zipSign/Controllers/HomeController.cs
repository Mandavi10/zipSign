using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class HomeController : Controller
    {
        //Security objSecurity = new Security();

        public ActionResult Index()
        {
            //string pass = objSecurity.Decrypt("Jbj7I+f2nzOCWyFg+IX3Zw==");

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}