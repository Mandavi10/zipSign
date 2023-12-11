using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Script.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
namespace zipSign.Controllers.APIs
{
    public class SignInAPIController : ApiController
    {
        public ProcMaster pro = new ProcMaster();
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [Route("SignInAPI/Login")]
        [HttpPost]
        public IHttpActionResult Login([FromBody] JObject requestData)
        {
            string secretKey = "a1b2c3d4e5f6g7h8i9j0kA1B2C3D4E5F6G7H8I9J0";
            string issuer = "Test";
            string audience = "Demo";
           
            
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
                    string UserId = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UserMasterID"]);
                    string Token = GenerateJwtToken(secretKey, issuer, audience, UserId);
                    var result = new
                    {
                        status = true,
                        message = "Login Succcessfully",
                        Email = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"])),
                        Mobile = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["MobileNumber"]),

                        Name = statusClass.DataFetch.Tables[0].Rows[0]["Name"],
                        UserId = statusClass.DataFetch.Tables[0].Rows[0]["UserMasterID"]

                    };
                    OTPResult otpResponse = SendOTP(statusClass.DataFetch.Tables[0].Rows[0]["Name"].ToString(), statusClass.DataFetch.Tables[0].Rows[0]["MobileNumber"].ToString(), statusClass.DataFetch.Tables[0].Rows[0]["Email"].ToString());

                    var response = new
                    {
                        status = result.status,
                        message = result.message,
                        LoginResult = new
                        {
                            Email = result.Email,
                            Mobile = result.Mobile,
                            Name = result.Name,
                            UserId = result.UserId,
                            Token= Token
                        },
                        OtpResult = new
                        {
                            TraceNumber = otpResponse.TraceNumber
                        }
                    };
                    
                    return Json(response);

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
                if (statusClass.StatusCode == 8)
                {
                    var result = new
                    {
                        status = false,
                        message = "Already Login"
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
        public class CompositeResponse
        {
            public object LoginResult { get; set; }
            public OTPResult OtpResult { get; set; }
        }

        public class OTPResult
        {
            public string TraceNumber { get; set; }
        }
        public OTPResult SendOTP(string CusName, string MobileNo, string Email)
        {
            string PhoneNumber = AESEncryption.AESEncryptionClass.DecryptAES(MobileNo);
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            SendOTPviaSMS(CusName, PhoneNumber, OTP);
            SendOTPviaEmail(Email, OTP);
            Random random = new Random();
            int txnId = random.Next(100, 1000);
            string TraceNumber = "612000" + DateTime.Now.ToString("ddMMyyyyHHmmss") + txnId;
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("TxnId", TraceNumber),
                new DataItems("Otp", OTP),
                new DataItems("GeneratedUsing", Email),
                new DataItems("ActionType","Login"),
                new DataItems("QueryType", "OtpExpired"),

            };
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            OTPResult result = new OTPResult
            {
                TraceNumber = TraceNumber,
            };
            return result;
        }

        private void SendOTPviaSMS(string CusName, string MobileNo, string OTP)
        {
            SMSModel Sm = new SMSModel();
            List<DataItems> obj = new List<DataItems>();
            Sm.CusName = CusName;
            Sm.MobileNo = MobileNo;
            obj.Add(new DataItems("UserName", Sm.CusName));
            obj.Add(new DataItems("Phoneno", Sm.MobileNo));
            obj.Add(new DataItems("OTP", OTP));
            obj.Add(new DataItems("QueryType", "GetSMSData"));
            statusClass = bal.GetFunctionWithResult(pro.USP_GetSMSData, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                string apiUrl = "https://smsapi.zipnach.com/api/SMS/SMSClient";
                if ((statusClass.DataFetch.Tables[0].Rows[0]["EmailId"]) != null)
                {
                    Sm.EmailID = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["EmailId"]));
                }
                Sm.EntityId = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Appid"]));

                Sm.Mandateid = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Mandateid"]));

                Sm.Message = Convert.ToString(statusClass.DataFetch.Tables[1].Rows[0]["SMS_MessageString"]);

                Sm.MobileNo = AESEncryption.AESEncryptionClass.EncryptAES(Sm.MobileNo);

                Sm.OTP = AESEncryption.AESEncryptionClass.EncryptAES(OTP);

                Sm.CusName = Sm.CusName.ToString();

                Sm.Ref1 = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Refrenceno"]);
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        string inputJson = (new JavaScriptSerializer()).Serialize(Sm);
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

            _ = new
            {
                status = "201",
            };
            //return Json(result1, JsonRequestBehavior.AllowGet);
        }

        private void SendOTPviaEmail(string Email, string OTP)
        {
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
                string formattedDate = DateTime.Now.ToString("dd MMMM, yyyy 'at' hh:mm tt 'IST'");
                msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
                msg.Subject = "Sign-in into zipSign";
                string message = "<html>";
                message += "<head>";
                message += "<style>";
                message += "body { font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5; }";
                message += ".container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }";
                message += "h1 { color: #007BFF; }";
                message += "p { font-size: 16px; line-height: 1.5; margin-bottom: 20px; }";
                message += ".disclaimer { color: #999; font-size: 12px; }";
                message += ".footer { background-color: #007BFF; color: #fff; padding: 09px 0; text-align: center; }";
                message += "</style>";
                message += "</head>";
                message += "<body>";
                message += "<div class='container'>";
                message += "<p>Dear User,</p>";
                message += "<p>Below is your One-Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>This password is valid for 10 minutes to complete sign-in, requested " + formattedDate + ".</p>";
                message += "<p>Never share this password with anyone.</p>";
                message += "<p class='disclaimer'>If you have not initiated this One Time Password, please <a href='mailto:youremail@example.com' style='color: #007ACC; font-weight: bold; text-decoration: underline;'>contact us</a>.</p>";
                message += "<p class='disclaimer'>Please do not reply to the email for any enquiries – messages sent to this address cannot be answered.</p>";
                message += "<p class='disclaimer'>Kindly contact our Customer Service Representative: customersupport@zipsign.com</p>";
                message += "</div>";
                message += "</div>";
                message += "</body>";
                message += "</html>";
                msg.Body = message;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true
                };
                NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(msg);
            }
        }
        private class ApiResponse
        {
            public string status { get; set; }
            public object Data { get; set; }
        }
        public string GenerateJwtToken(string secretKey, string issuer, string audience, string userId)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] { new Claim(ClaimTypes.Name, userId) },
                expires: DateTime.UtcNow.AddMinutes(2), // Adjust the token expiration as needed
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }


}

