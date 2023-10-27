using BusinessDataLayer;
using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class SendEmailForSignupController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        [Route("SendEmailForSignup/SendOTP")]
        public IHttpActionResult SendOTP([FromBody] OTPRequest otpRequest)
        {
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$";
            if (!Regex.IsMatch(otpRequest.Email, pattern))
            {
                return Json(new { status = false, message = "Invalid Email Format ex. User@Example.com" });
            }

            try
            {
                string SentOTP = SendOTPviaEmail(otpRequest.Email);
                return Json(new { status = true, message = "OTP Send Successfully", OTP = SentOTP });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Failed To Send OTP. Reason:" + ex });
            }
        }

        private string SendOTPviaEmail(string Email)
        {
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
                msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
                msg.Subject = "Verify Your Email Address – zipSign";
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
                message += "<p>Thank you for signing up with zipSign. To complete your registration and enjoy all the benefits of our service, please verify your email address by entering the below One Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>Once your email and mobile are verified, you'll have full access to your account, and you can start using our platform immediately.</p>";
                message += "<p class='disclaimer'>If you did not register with zipSign, please ignore this email. If you have any questions or need assistance, please don't hesitate to contact our support team at customersupport@zipsign.com.</p>";
                message += "<p>Thank you for choosing zipSign!</p>";
                message += "<p class='disclaimer'>Sincerely,</p>";
                message += "<p class='disclaimer'>Customer Support.</p>";
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
                Random random = new Random();
                int txnId = random.Next(100, 1000);
                string TraceNumber = "612000" + DateTime.Now.ToString("ddMMyyyyHHmmss") + txnId;
                List<DataItems> obj = new List<DataItems>
            {
                new DataItems("TxnId", TraceNumber),
                new DataItems("Otp", OTP),
                new DataItems("GeneratedUsing", Email),
                new DataItems("ActionType","SignUp"),
                new DataItems("QueryType", "OtpExpired"),
            };
                statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            }
            return OTP;
        }
    }


    public class OTPRequest
    {
        public string Email { get; set; }
    }

}

