using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class PaymentIntegrationController : Controller
    {
        // GET: PaymentIntegration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PhonePe()
        {
            Dictionary<string, object> data = new Dictionary<string, object>
    {
        { "merchantId", "MERCHANTUAT" },
        { "merchantTransactionId", Guid.NewGuid().ToString() },
        { "merchantUserId", "MUID123" },
        { "amount", 10000 },
        { "redirectUrl", Url.Action("Response", "YourControllerName", null, Request.Url.Scheme) },
        { "redirectMode", "POST" },
        { "callbackUrl", Url.Action("Response", "YourControllerName", null, Request.Url.Scheme) },
        { "mobileNumber", "9999999999" },
        { "paymentInstrument", new Dictionary<string, string> { { "type", "PAY_PAGE" } } }
    };
            string encode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
            string saltKey = "099eb0cd-02cf-4e2a-8aca-3e6c6aff0399";
            int saltIndex = 1;
            string stringToHash = encode + "/pg/v1/pay" + saltKey;
            string sha256 = BitConverter.ToString(new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(stringToHash))).Replace("-", "");
            string finalXHeader = sha256 + "###" + saltIndex;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("X-VERIFY", finalXHeader);

                Dictionary<string, string> requestData = new Dictionary<string, string> { { "request", encode } };
                StringContent content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("https://api-preprod.phonepe.com/apis/merchant-simulator/pg/v1/pay", content).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic rData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return Redirect(rData.data.instrumentResponse.redirectInfo.url.ToString());
            }
        }

        [HttpPost]
        public new ActionResult Response()
        {
            string input = Request.Form["request"]; // Assuming the request parameter is posted via form data
            string saltKey = "099eb0cd-02cf-4e2a-8aca-3e";
            // Rest of the code
            return View(input, saltKey);
        }

    }
}