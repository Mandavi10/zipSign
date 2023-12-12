using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class ForgotPasswordAPIController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        public IHttpActionResult ResetPassword([FromBody] JObject requestData)
        {
            ForgotPasswordModel Data = requestData["Data"].ToObject<ForgotPasswordModel>();
            if (string.IsNullOrEmpty(Data.Email))
            {
                return Json(new { status = false, message = "Email can't Empty" });
            }
            else
            {
                string pattern = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
                if (!Regex.IsMatch(Data.Email, pattern))
                {
                    return Json(new { status = false, message = "Invalid Email format" });
                }
            }
            List<DataItems> obj = new List<DataItems>
                {
                    //string clientIP = GetClientIP();
                    //obj.Add(new DataItems("IP", clientIP));
                    new DataItems("Email", Data.Email),
                    new DataItems("QueryType", "GetDataForUser")
                };
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            if (statusClass.StatusCode == 10)
            {
                var result = new
                {
                    status = false,
                    message = "User not Exist!"
                };
                return Json(result);
            }
            string UserCode = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UserMasterId"]);
            string UserEmail = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"]);
            SendLinkviaEmail(UserEmail, UserCode);
            return Json(new { status = true, message = "Email Send Successfully." });
        }


        public string SendLinkviaEmail(string Email, string UserCode)
        {
            string EncUserCode = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(UserCode));
            string LinkText = GenerateResetLink(EncUserCode);

            //string LinkText = GenerateResetLink(UserCode);

            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
                msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
                msg.Subject = "Password Reset Request";

                string message = @"
            <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f5f5f5;
                    }
                    .container {
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                        background-color: #fff;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                    }
                    h1 {
                        color: #007BFF;
                    }
                    p {
                        font-size: 16px;
                        line-height: 1.5;
                        margin-bottom: 20px;
                    }
                    .disclaimer {
                        color: #999;
                        font-size: 12px;
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Password Reset Request</h1>
                    <p>Dear User,</p>
                    <p>You have requested to reset your password. Below is your password reset link:</p>
                    <a href='" + LinkText + @"' style='color: #007BFF;'>" + LinkText + @"</a>
                    <p>This link is valid for 10 minutes from the time of this email.</p>
                    <p><strong>Do not share this link with anyone.</strong></p>
                    <p class='disclaimer'>If you did not request this password reset, please <a href='mailto:youremail@example.com' style='color: #007ACC; font-weight: bold; text-decoration: underline;'>contact us immediately</a>.</p>
                    <p class='disclaimer'>Please do not reply to this email for any inquiries – messages sent to this address cannot be answered.</p>
                    <p class='disclaimer'>For assistance, kindly contact our Customer Service Representative at customersupport@zipsign.com</p>
                </div>
            </body>
            </html>";


                msg.Body = message;
                msg.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx")
                };

                smtp.Send(msg);

                DateTime createdOn = DateTime.Now;
                DateTime expiryTime = createdOn.AddMinutes(10); // Changed to 10 minutes for link validity
                InsertLinkIntoDatabase(UserCode, Email, createdOn, expiryTime, LinkText);
            }

            return "";
        }

        private string GenerateResetLink(string userCode)
        {
            //return $"http://localhost:50460/Login/ChangePassword?UserCode={userCode}";
            return $"https://uataadharsign.zipsign.in/Login/ChangePassword?UserCode={userCode}";
        }
        private void InsertLinkIntoDatabase(string userCode, string email, DateTime createdOn, DateTime expiryTime, string LinkText)
        {
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("Email", email),
                new DataItems("CreatedOn", createdOn),
                new DataItems("CreatedBy", userCode),
                new DataItems("ExpiredOn", expiryTime),
                new DataItems("Link", LinkText),
                new DataItems("QueryType", "SendLink")
            };
            statusClass = bal.PostFunction(pro.Signup, obj);
        }
        public class ForgotPasswordModel
        {
            public string Email { get; set; }
        }
    }
}
