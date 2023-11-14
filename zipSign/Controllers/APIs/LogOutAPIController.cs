using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class LogOutAPIController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        public IHttpActionResult SignOut([FromBody] JObject requestData)
        {
            Logout Data = requestData["Data"].ToObject<Logout>();
            if (string.IsNullOrEmpty(Data.UserMasterID))
            {
                return Json(new { status = false, message = "Please check you internet connection" });
            }
            List<DataItems> obj = new List<DataItems>();
            string clientIP = GetClientIP();
            obj.Add(new DataItems("LogOut_IP_Address", clientIP));
            obj.Add(new DataItems("UserMasterID", Data.UserMasterID));
            obj.Add(new DataItems("QueryType", "SignOut"));
            statusClass = bal.PostFunction(pro.Signup, obj);
            //Session.Abandon();
            if(statusClass.StatusCode==9)
            {
                return Json(new { status = true, message = "Logout Successfully" });
            }
            if (statusClass.StatusCode == 10)
            {
                return Json(new { status = false, message = "User not found" });
            }
            return Json("");
        }
        public static string GetClientIP()
        {
            try
            {
                string publicIP = string.Empty;
                // Check for Internet connectivity
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    using (WebClient client = new WebClient())
                    {
                        // Use a public DNS server to resolve your public IP address
                        publicIP = client.DownloadString("https://icanhazip.com").Trim();
                    }
                }
                return publicIP;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
    }
}
