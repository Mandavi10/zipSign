using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class VerifyOTPAPIController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        public IHttpActionResult VerifyOTP([FromBody] JObject requestData)
        {
            OTP Data = requestData["Data"].ToObject<OTP>();
            if (string.IsNullOrWhiteSpace(Data.Otp))
            {
                return Json(new { status = false, message = "Please Enter OTP" });
            }
            else if (string.IsNullOrWhiteSpace(Data.TxnId))
            {
                return Json(new { status = false, message = "Please Enter TxnId" });
            }
            List<DataItems> obj = new List<DataItems>
        {
        new DataItems("Otp", Data.Otp),
        new DataItems("TxnId", Data.TxnId),
        new DataItems("QueryType", "IsValidOTP")
       };

            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            if (statusClass.StatusCode == 9)
            {
                if (statusClass.DataFetch.Tables[0].Rows[0]["IsExpired"].ToString() == "True")
                {
                    var result = new
                    {
                        status = false,
                        message = "OTP Expired"
                    };
                    return Json(result);
                }
            }

            if (statusClass.StatusCode == 10)
            {
                var result = new
                {
                    status = false,
                    message = "Enter correct OTP"
                };
                return Json(result);
            }
            if (statusClass.StatusCode == 9)
            {
                DateTime otpGeneratedTime = (DateTime)statusClass.DataFetch.Tables[0].Rows[0]["CreatedOn"];
                DateTime currentTime = DateTime.Now;
                TimeSpan timeDifference = currentTime - otpGeneratedTime;

                if (timeDifference.TotalMinutes > 10)
                {
                    var result = new
                    {
                        status = false,
                        message = "OTP has Expired"
                    };
                    return Json(result);
                }
                else
                {
                    var result = new
                    {
                        status = true,
                        message = "OTP Verified Successfully!"
                    };
                    return Json(result);
                }
            }
            else
            {
                return Json(2); // Return a status code indicating no OTP record found
            }
        }
    }


}


