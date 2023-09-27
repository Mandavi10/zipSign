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
    }
}