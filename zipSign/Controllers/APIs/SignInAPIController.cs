using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class SignInAPIController : ApiController
    {
        public ProcMaster pro = new ProcMaster();
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        [Route("SignInAPI/Login")]
        public IHttpActionResult Login([FromBody] JObject requestData)
        {
            Login Data = requestData["Data"].ToObject<Login>();
            try
            {
                string emailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                string mobileRegexPattern = @"^(\+\d{1,3}[- ]?)?\d{10}$";
                string input = Data.Email;
                if (string.IsNullOrEmpty(input))
                {
                    return Json(new { status = false, message = "Email/Mobile can't Empty" });
                }
                else if (!Regex.IsMatch(input, emailRegexPattern) && !Regex.IsMatch(input, mobileRegexPattern))
                {
                    return Json(new { status = false, message = "Invalid email/mobile format" });
                }
                else if (string.IsNullOrEmpty(Data.Password))
                {
                    return Json(new { status = false, message = "Password can't Empty" });
                }
                else
                {
                    string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
                    if (!Regex.IsMatch(Data.Password, pattern))
                    {
                        return Json(new { status = false, message = "Invalid password format" });
                    }
                }

                List<DataItems> obj = new List<DataItems>
            {
                new DataItems("Email", Data.Email),
                new DataItems("MobileNumber", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Data.Email))),
                new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Data.Password))),
                new DataItems("QueryType", "LoginData")
            };
                statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
                if (statusClass.StatusCode == 5)
                {
                    var result = new
                    {
                        status = true,
                        message = "Login Succcessfully",
                        Email = statusClass.DataFetch.Tables[0].Rows[0]["Email"],
                        Mobile = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["MobileNumber"]),
                    };
                    return Json(result);
                }
                if (statusClass.StatusCode == 6)
                {
                    var result = new
                    {
                        status = false,
                        message = "Invalid Credentials"
                    };
                    return Json(result);
                }
                if (statusClass.StatusCode == 7)
                {
                    var result = new
                    {
                        status = false,
                        message = "Account locked due to too many failed attempts"
                    };
                    return Json(result);
                }
            }

            catch (Exception ex)
            {
                return Json(new { ex });
            }
            return Json("");
        }
        private class ApiResponse
        {
            public string status { get; set; }
            public object Data { get; set; }
        }
    }
}
