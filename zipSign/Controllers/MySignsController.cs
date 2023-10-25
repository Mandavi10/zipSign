using System;
using System.Text.RegularExpressions;
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

        public ActionResult BuySignHistory()
        {
            return View();
        }

        public ActionResult SignTransactionLog()

        {
            return View();
        }
        public ActionResult Signature(string AmountRupees)
        {
            int result;
            if (IsLength(AmountRupees, 9) == true && Isspecialcharacter(AmountRupees) == false && !CheckHTML(AmountRupees) == true)
            {
                result = 0;
            }
            else
            {
                result = 1;
            }
            return Json(result);
        }
        public bool CheckHTML(string text)
        {
            bool flag = false;
            try
            {
                Regex tagWithoutClosingRegex = new Regex(@"<(\s*[(\/?)\w+]*)");
                bool hasTags = tagWithoutClosingRegex.IsMatch(text.ToString());
                if (hasTags)
                {
                    flag = true;
                }
                else
                {
                    string[] printer = { "&#38", "&lt", "&gt", "&#39", "&#34", "script" };
                    for (int i = 0; i < printer.Length; i++)
                    {
                        if (text.Contains(printer[i]))
                        {
                            flag = true;
                        }
                    }
                }
                return flag;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return flag;
        }



        public bool Isspecialcharacter(string str)
        {
            char[] specialcharacter = str.ToCharArray(); bool flag = false;
            for (int i = 0; i < specialcharacter.Length; i++)
            {
                if (!char.IsLetterOrDigit(specialcharacter[i]))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }
        public bool IsLength(string str, int length)
        {
            bool flag = false;
            if (Convert.ToInt32(str.Length) <= length)
            {
                flag = true;
            }
            return flag;
        }
    }
}