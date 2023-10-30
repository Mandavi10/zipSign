using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace zipSign.Controllers.APIs
{
    public class SendMobileOTPAPIController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        public IHttpActionResult GetSMSData([FromBody] JObject requestData)
        {
            SMSModel Data = requestData["Data"].ToObject<SMSModel>();
            string MobilePattern = @"^\d{10}$";

            if (string.IsNullOrWhiteSpace(Data.CusName))
            {
                return Json(new { status = false, message = "Please Enter User Name" });
            }
            else if (!Regex.IsMatch(Data.MobileNo, MobilePattern))
            {
                return Json(new { status = false, message = "Please Enter Correct Mobile Number" });
            }
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("UserName", Data.CusName),
                new DataItems("Phoneno", Data.MobileNo),
                new DataItems("OTP", OTP),
                new DataItems("QueryType", "GetSMSData")
            };
            statusClass = bal.GetFunctionWithResult(pro.USP_GetSMSData, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                string apiUrl = "https://smsapi.zipnach.com/api/SMS/SMSClient";
                if ((statusClass.DataFetch.Tables[0].Rows[0]["EmailId"]) != null)
                {
                    Data.EmailID = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["EmailId"]));
                }
                Data.EntityId = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Appid"]));

                Data.Mandateid = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Mandateid"]));

                Data.Message = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["SMS_MessageString"]);

                Data.MobileNo = AESEncryption.AESEncryptionClass.EncryptAES(Data.MobileNo);

                Data.OTP = AESEncryption.AESEncryptionClass.EncryptAES(OTP);

                Data.CusName = Data.CusName.ToString();

                Data.Ref1 = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Refrenceno"]);
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        string inputJson = new JavaScriptSerializer().Serialize(Data);
                        client.Headers["Content-type"] = "application/json";
                        client.Encoding = Encoding.UTF8;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                        string Result = client.UploadString(apiUrl, inputJson);
                        Response results = JsonConvert.DeserializeObject<Response>(Result);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            Random random = new Random();
            int txnId = random.Next(100, 1000);
            string TraceNumber = "612000" + DateTime.Now.ToString("ddMMyyyyHHmmss") + txnId;
            List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("TxnId", TraceNumber),
                new DataItems("Otp", OTP),
                new DataItems("GeneratedUsing",  Data.MobileNo),
                new DataItems("ActionType","SignUp"),
                new DataItems("QueryType", "OtpExpired"),

            };
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj1);
            return Json(new { status = true, message = "OTP Sent Successfully", TraceNumber, OTP });

        }
    }
}
