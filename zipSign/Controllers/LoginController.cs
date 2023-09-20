using BusinessAccessLayer;
using BusinessLayerModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace zipSign.Controllers
{
    [SessionState(SessionStateBehavior.Required)]
    public class LoginController : Controller
    {
        private BusinessAccesslayerClass bal = new BusinessAccesslayerClass();
        private CommonStatus statusClass = new CommonStatus();

        //Security objSecurity = new Security();
        private ProcMaster pro = new ProcMaster();
        public ActionResult Index()
        {
            Session["CAPTCHA"] = GetRandomText();
            return View();
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        public ActionResult Signup2()
        {
            return View();
        }

        public ActionResult SignLogin()
        {
            return View();
        }

        //[ValidateAntiForgeryToken]

        [HttpPost]
        //public JsonResult SignUp(SignUp objSignUpModel, string UserType)
        //{
        //    string clientIP = GetClientIP();
        //    string Password = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objSignUpModel.Password));
        //    string PhoneNumber = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objSignUpModel.Mobile));
        //    try
        //    {
        //        SignUp Data = new SignUp();
        //        List<DataItems> obj = new List<DataItems>();
        //        obj.Add(new DataItems("Name", objSignUpModel.Name));
        //        obj.Add(new DataItems("Email", objSignUpModel.Email));
        //        obj.Add(new DataItems("MobileNumber", PhoneNumber));
        //        obj.Add(new DataItems("MobileOTP", objSignUpModel.OTP));
        //        obj.Add(new DataItems("State", objSignUpModel.State));
        //        obj.Add(new DataItems("Password", Password));
        //        obj.Add(new DataItems("UserType", UserType));
        //        obj.Add(new DataItems("Login_IP_Address", clientIP));

        //        if (UserType == "corporate")
        //        {
        //            obj.Add(new DataItems("PAN", objSignUpModel.panNumber));
        //            obj.Add(new DataItems("CorpName", objSignUpModel.CorpName));
        //        }
        //        obj.Add(new DataItems("QueryType", "InsertRecord"));
        //        statusClass = bal.PostFunction(pro.Signup, obj);
        //        if (statusClass.StatusCode == 1)
        //        {
        //            var result = new
        //            {
        //                status = statusClass.StatusCode
        //            };
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (statusClass.StatusCode == 2)
        //        {
        //            var result = new
        //            {
        //                status = statusClass.StatusCode, //Duplicate PAN Number
        //            };
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //        else if (statusClass.StatusCode == 3)
        //        {
        //            var result = new
        //            {
        //                status = statusClass.StatusCode, //Duplicate Email
        //            };
        //            return Json(result, JsonRequestBehavior.AllowGet);

        //        }
        //        else if (statusClass.StatusCode == 4)
        //        {
        //            var result = new
        //            {
        //                status = statusClass.StatusCode, //Duplicate Phone Number
        //            };
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    var result1 = new
        //    {
        //        status = "101",
        //    };
        //    return Json(result1, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SignUp(SignUp objSignUpModel, string UserType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { status = "Validation failed", errors });
                }
                if (!IsValidEmail(objSignUpModel.Email) || objSignUpModel.Email == null)
                {
                    return Json(new { status = "Invalid email format" });
                }
                if (!IsValidMobile(objSignUpModel.Mobile))
                {
                    return Json(new { status = "Invalid mobile format" });
                }
                if (!IsValidPassword(objSignUpModel.Password))
                {
                    return Json(new { status = "Invalid password format" });
                }
                if (UserType == "corporate")
                {
                    if (!IsValidPAN(objSignUpModel.panNumber))
                    {
                        return Json(new { status = "Invalid PAN format" });
                    }
                }
                SignUp Data = new SignUp();
                List<DataItems> obj = new List<DataItems>();
                obj.Add(new DataItems("Name", objSignUpModel.Name));
                obj.Add(new DataItems("Email", objSignUpModel.Email));
                obj.Add(new DataItems("MobileNumber", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objSignUpModel.Mobile))));
                obj.Add(new DataItems("MobileOTP", objSignUpModel.OTP));
                obj.Add(new DataItems("State", objSignUpModel.State));
                obj.Add(new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objSignUpModel.Password))));
                obj.Add(new DataItems("UserType", UserType));
                if (UserType == "corporate")
                {
                    obj.Add(new DataItems("PAN", objSignUpModel.panNumber));
                    obj.Add(new DataItems("CorpName", objSignUpModel.CorpName));
                }
                obj.Add(new DataItems("QueryType", "InsertRecord"));
                statusClass = bal.PostFunction(pro.Signup, obj);

                if (statusClass.StatusCode == 1)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (statusClass.StatusCode == 2)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode, // Duplicate PAN Number
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (statusClass.StatusCode == 3)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode, // Duplicate Email
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else if (statusClass.StatusCode == 4)
                {
                    var result = new
                    {
                        status = statusClass.StatusCode, // Duplicate Phone Number
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = ex.Message });
            }

            var result1 = new
            {
                status = "101",
            };
            return Json(result1, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidEmail(string email)
        {
            // Use regex to validate the email format
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidMobile(string mobile)
        {
            // Use regex to validate the mobile number format (e.g., allow only digits)
            string pattern = @"^\d{10}$";
            return Regex.IsMatch(mobile, pattern);
        }

        private bool IsValidPassword(string password)
        {
            // Perform your password validation logic here
            // For example, you can check for minimum length, special characters, etc.
            // Replace the following placeholder logic with your actual password validation rules
            return password.Length >= 8;
        }

        private bool IsValidPAN(string pan)
        {
            // Use regex to validate PAN format
            string pattern = @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$";
            return Regex.IsMatch(pan, pattern);
        }









        [HttpPost]
        public JsonResult Login(Login objLoginModel, string captchaInput)
        {
            try
            {
                string expectedCaptcha = Session["CAPTCHA"] as string;
                bool isCaptchaValid = string.Equals(captchaInput, expectedCaptcha, StringComparison.OrdinalIgnoreCase);
                if (isCaptchaValid)
                {
                    SignUp Data = new SignUp();
                    List<Login> result = new List<Login>();
                    List<DataItems> obj = new List<DataItems>();
                    obj.Add(new DataItems("Email", objLoginModel.Email));
                    obj.Add(new DataItems("MobileNumber", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objLoginModel.Mobile))));
                    obj.Add(new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objLoginModel.Password))));
                    obj.Add(new DataItems("QueryType", "LoginData"));
                    statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
                    if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                        {
                            result.Add(new Login
                            {
                                UserName = Convert.ToString(dr["Name"]),
                                Email = Convert.ToString(dr["Email"]),
                                Mobile = Convert.ToString(dr["MobileNumber"]),
                                UserId = Convert.ToString(dr["UserMasterID"]),
                            });
                            this.Session["UserId"] = statusClass.DataFetch.Tables[0].Rows[0]["UserMasterID"];
                        }
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch
            {
            }
            var result1 = new
            {
                status = statusClass.StatusCode
            };
            return Json(result1, JsonRequestBehavior.AllowGet);
        }
        public static string GetClientIP()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipRange = ip.Split(',');
                return ipRange[0].Trim();
            }
            else
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }


        private string GetRandomText()
        {
            StringBuilder randomText = new StringBuilder();
            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";
            Random r = new Random();
            for (int j = 0; j <= 5; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            return randomText.ToString();
        }
        public FileResult GetCaptchaImage()
        {
            string text = GetRandomText();
            Session["CAPTCHA"] = text;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    Font font = new Font("Arial", 15);
                    SizeF textSize = drawing.MeasureString(text, font);
                    using (Bitmap captchaImage = new Bitmap((int)textSize.Width + 40, (int)textSize.Height + 20))
                    {
                        using (Graphics captchaGraphics = Graphics.FromImage(captchaImage))
                        {
                            Color backColor = Color.White;
                            Color textColor = Color.Red;
                            captchaGraphics.Clear(backColor);
                            using (Brush textBrush = new SolidBrush(textColor))
                            using (Brush backBrush = new SolidBrush(backColor))
                            {
                                captchaGraphics.DrawString(text, font, textBrush, 20, 10);
                                captchaGraphics.DrawString(text, font, backBrush, 0, 88);
                            }
                            using (MemoryStream ms = new MemoryStream())
                            {
                                captchaImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                return File(ms.ToArray(), "image/png");
                            }
                        }
                    }
                }
            }
        }
        [HttpPost]
        public ActionResult GetSMSData(SMSModel Sm)
        {
            Random rnd = new Random();
            string OTP1 = rnd.Next(100000, 999999).ToString();
            Session["otp"] = OTP1;
            List<DataItems> obj = new List<DataItems>();
            obj.Add(new DataItems("UserName", Sm.CusName));
            obj.Add(new DataItems("Phoneno", Sm.MobileNo));
            obj.Add(new DataItems("OTP", OTP1));
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

                Sm.OTP = AESEncryption.AESEncryptionClass.EncryptAES(OTP1);

                Sm.CusName = Sm.CusName.ToString();

                Sm.Ref1 = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Refrenceno"]);
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        string inputJson = new JavaScriptSerializer().Serialize(Sm);
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
            var result1 = new
            {
                status = "201",
            };
            return Json(result1, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult GetSMSData1(SMSModel Sm, string CusName, string MobileNo)
        {
            Random rnd = new Random();
            string OTP1 = rnd.Next(100000, 999999).ToString();
            Session["otp"] = OTP1;
            List<DataItems> obj = new List<DataItems>();
            Sm.CusName = CusName;
            Sm.MobileNo = MobileNo;
            obj.Add(new DataItems("UserName", Sm.CusName));
            obj.Add(new DataItems("Phoneno", Sm.MobileNo));
            obj.Add(new DataItems("OTP", OTP1));
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

                Sm.OTP = AESEncryption.AESEncryptionClass.EncryptAES(OTP1);

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
            var result1 = new
            {
                status = "201",
            };
            return Json(result1, JsonRequestBehavior.AllowGet);

        }
        //public JsonResult GetEmailData(string Email)
        //{

        //    Random rnd = new Random();
        //    string OTP = rnd.Next(100000, 999999).ToString();
        //    Session["otp"] = OTP;
        //    using (System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage("rohan153555@gmail.com", Email))
        //    {
        //        msg.Subject = "One Time Password :";
        //        string mss = "Your One Time Password is:" + OTP;
        //        msg.Body = mss.ToString();
        //        msg.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;
        //        NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = networkCredential;
        //        smtp.Port = 587;
        //        smtp.Send(msg);
        //        return Json("OTP send successfully !", OTP);
        //    }
        //}

        //public JsonResult GetEmailData(string Email)
        //{
        //    Random rnd = new Random();
        //    string OTP = rnd.Next(100000, 999999).ToString();
        //    Session["otp"] = OTP;
        //    using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
        //    {
        //        msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
        //        msg.Subject = "Your One Time Password (OTP) for Account Verification";
        //        string message = "<html>";
        //        message += "<head>";
        //        message += "<style>";
        //        message += "body { font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5; }";
        //        message += ".container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #fff; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }";
        //        message += "h1 { color: #007BFF; }";

        //        message += "p { font-size: 16px; line-height: 1.5; margin-bottom: 20px; }";
        //        message += ".disclaimer { color: #999; font-size: 12px; }";
        //        message += ".footer { background-color: #007BFF; color: #fff; padding: 09px 0; text-align: center; }";
        //        message += "</style>";
        //        message += "</head>";
        //        message += "<body>";
        //        message += "<div class='container'>";
        //        message += "<h1>Welcome to zipSign</h1>";
        //        message += "<p>Dear User,</p>";
        //        message += "<p>Your One Time Password (OTP) for accessing our service is:</p>";
        //        message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
        //        message += "<p>Please use this OTP to complete your action.</p>";
        //        message += "<p class='disclaimer'>If you did not request this OTP, please ignore this email.</p>";
        //        message += "<p>Best regards,</p>";
        //        message += "<p>Team zipSign</p>";
        //        message += "<p class='disclaimer'>This is an auto-generated email. Please do not reply to this message.</p>";
        //        message += "</div>";
        //        message += "<div class='footer'>";
        //        message += "<p>&copy; " + DateTime.Now.Year + " zipSign. All rights reserved.</p>";
        //        message += "</div>";
        //        message += "</body>";
        //        message += "</html>";
        //        msg.Body = message;
        //        msg.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;
        //        NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = networkCredential;
        //        smtp.Port = 587;
        //        smtp.Send(msg);
        //        return Json("OTP sent successfully!", OTP);
        //    }
        //}
        //For SignUp

        public ActionResult SendOTP(string CusName, string MobileNo, string Email)
        {
            string PhoneNumber = AESEncryption.AESEncryptionClass.DecryptAES(MobileNo);
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            Session["otp"] = OTP;
            SendOTPviaSMS(CusName, PhoneNumber, OTP);
            SendOTPviaEmail(Email, OTP);
            var result = new
            {
                status = "201",
                message = "OTP sent successfully to both phone and email."
            };
            return Json(result, JsonRequestBehavior.AllowGet);
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
            var result1 = new
            {
                status = "201",
            };
            //return Json(result1, JsonRequestBehavior.AllowGet);
        }

        private void SendOTPviaEmail(string Email, string OTP)
        {
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
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
                //message += "<img src='https://imgur.com/I9P7Zms' alt='Background Image' style='position: absolute; top: 0; left: 0; width: 100%; height: 100%; opacity: 0.3; background-repeat: no-repeat; background-size: cover;' />";
                message += "<p>Dear User,</p>";
                message += "<p>Below is your One-Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>This password is valid for 10 minutes to complete sign-in, requested 07 September, 2023 at 12:01 PM IST.</p>";
                message += "<p>Never share this password with anyone.</p>";
                message += "<p class='disclaimer'>If you have not initiated this One Time Password, please <a href='mailto:youremail@example.com' style='color: #007ACC; font-weight: bold; text-decoration: underline;'>contact us</a>.</p>";
                //message += "<p>To Contact Us</p>";
                message += "<p class='disclaimer'>Please do not reply to the email for any enquiries – messages sent to this address cannot be answered.</p>";
                message += "<p class='disclaimer'>Kindly contact our Customer Service Representative: customersupport@zipsign.com</p>";
                message += "</div>";
                //message += "<div class='footer'>";
                //message += "<p>&copy; " + DateTime.Now.Year + " zipSign. All rights reserved.</p>";
                message += "</div>";
                message += "</body>";
                message += "</html>";
                msg.Body = message;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(msg);
                // return Json("OTP sent successfully!", OTP);
            }
        }

        public JsonResult GetEmailData(string Email)
        {
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            Session["otp"] = OTP;
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
                //message += "<img src='https://imgur.com/I9P7Zms' alt='Background Image' style='position: absolute; top: 0; left: 0; width: 100%; height: 100%; opacity: 0.3; background-repeat: no-repeat; background-size: cover;' />";
                message += "<p>Dear User,</p>";
                message += "<p>Thank you for signing up with zipSign. To complete your registration and enjoy all the benefits of our service, please verify your email address by entering the below One Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>Once your email and mobile are verified, you'll have full access to your account, and you can start using our platform immediately.</p>";
                message += "<p class='disclaimer'>If you did not register with zipSign, please ignore this email. If you have any questions or need assistance, please don't hesitate to contact our support team at customersupport@zipsign.com.</p>";
                message += "<p>Thank you for choosing zipSign!</p>";
                message += "<p class='disclaimer'>Sincerely,</p>";
                message += "<p class='disclaimer'>Customer Support.</p>";
                message += "</div>";
                //message += "<div class='footer'>";
                //message += "<p>&copy; " + DateTime.Now.Year + " zipSign. All rights reserved.</p>";
                message += "</div>";
                message += "</body>";
                message += "</html>";
                msg.Body = message;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(msg);
                return Json("OTP sent successfully!", OTP);
            }
        }

        //For Login

        public JsonResult GetEmailDataForLogin(string Email)
        {
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            Session["otp"] = OTP;
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
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
                //message += "<img src='https://imgur.com/I9P7Zms' alt='Background Image' style='position: absolute; top: 0; left: 0; width: 100%; height: 100%; opacity: 0.3; background-repeat: no-repeat; background-size: cover;' />";
                message += "<p>Dear User,</p>";
                message += "<p>Below is your One-Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>This password is valid for 10 minutes to complete sign-in, requested 07 September, 2023 at 12:01 PM IST.</p>";
                message += "<p>Never share this password with anyone.</p>";
                message += "<p class='disclaimer'>If you have not initiated this One Time Password, please <a href='mailto:youremail@example.com' style='color: #007ACC; font-weight: bold; text-decoration: underline;'>contact us</a>.</p>";
                //message += "<p>To Contact Us</p>";
                message += "<p class='disclaimer'>Please do not reply to the email for any enquiries – messages sent to this address cannot be answered.</p>";
                message += "<p class='disclaimer'>Kindly contact our Customer Service Representative: customersupport@zipsign.com</p>";
                message += "</div>";
                //message += "<div class='footer'>";
                //message += "<p>&copy; " + DateTime.Now.Year + " zipSign. All rights reserved.</p>";
                message += "</div>";
                message += "</body>";
                message += "</html>";
                msg.Body = message;
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(msg);
                return Json("OTP sent successfully!", OTP);
            }
        }



        public JsonResult VerifyOTP(string VOTP)
        {
            string LoginIPAddress = GetClientIP();
            object UserMasterId = Session["UserId"];
            List<DataItems> obj = new List<DataItems>();
            obj.Add(new DataItems("UserMasterID", UserMasterId));
            obj.Add(new DataItems("Login_IP_Address", LoginIPAddress));
            obj.Add(new DataItems("QueryType", "LoginOTP"));

            statusClass = bal.PostFunction(pro.Signup, obj);
            string temp = Session["otp"].ToString();
            int msg = 0;
            if (temp == VOTP)
            {
                msg = 1;
            }
            else
            {
                msg = 2;
            }

            return Json(msg);
        }

        public JsonResult VerifyMobileOTP(string VOTP)
        {
            string LoginIPAddress = GetClientIP();
            string temp = Session["otp"].ToString();
            int msg = 0;
            if (temp == VOTP)
            {
                msg = 1;
            }
            else
            {
                msg = 2;
            }
            return Json(msg);
        }


        public JsonResult VerifyMobile(string VOTP, string FilePath, string SignerID, string UploadedDocumentId)
        {
            string FilePath1 = AESEncryption.AESEncryptionClass.DecryptAES(FilePath);
            //string SignerId = AESEncryption.AESEncryptionClass.DecryptAES(SignerID);
            string DocumentUploadedId = AESEncryption.AESEncryptionClass.DecryptAES(UploadedDocumentId);

            List<DataItems> obj = new List<DataItems>();
            obj.Add(new DataItems("SignerID", SignerID));
            obj.Add(new DataItems("UploadedDocumentId", DocumentUploadedId));
            obj.Add(new DataItems("QuerySelector", "CheckDocSigned"));
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            int Status = statusClass.StatusCode;
            string SignedPDF = "";
            if (statusClass.StatusCode == 1)
            {
                SignedPDF = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignedPDFPath"]);
            }

            string LoginIPAddress = GetClientIP();
            string temp = Session["otp"].ToString();
            int msg = 0;
            if (temp == VOTP)
            {
                msg = 1;
            }
            else
            {
                msg = 2;
            }

            return Json(new { Status = Status, msg = msg, Path = FilePath1, SignerId = SignerID, DocumentUploadedId = DocumentUploadedId, SignedPDF = SignedPDF }, JsonRequestBehavior.AllowGet);
            //return Json(msg, FilePath);
        }

        //public JsonResult VerifyMobile(string VOTP, string FilePath)
        //{
        //    string FilePath1 = AESEncryption.AESEncryptionClass.DecryptAES(FilePath);
        //    string LoginIPAddress = GetClientIP();
        //    string temp = Session["otp"].ToString();
        //    int msg = 0;
        //    if (temp == VOTP)
        //    {
        //        msg = 1;
        //    }
        //    else
        //    {
        //        msg = 2;
        //    }

        //    return Json(new { msg = msg, Path = FilePath1 }, JsonRequestBehavior.AllowGet);
        //    //return Json(msg, FilePath);
        //}
        public JsonResult VerifyEmailOTP(string VOTP)
        {
            string temp = Session["otp"].ToString();
            int msg = 0;

            if (temp == VOTP)
            {
                msg = 1;
            }
            else
            {
                msg = 2;
            }
            return Json(msg);
        }
        //public JsonResult SendVerfifyLinkByEmail(string Email, string fileId)
        //{
        //    using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
        //    {
        //        string Emailid = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Email));
        //        String File = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(fileId));
        //        msg.Subject = "Document for signing";
        //        string mss = "http://localhost:50460/zipSign/SignatureRequest";
        //        string urlWithEncodedFileId = mss + "?File=" + File;
        //        msg.Body = urlWithEncodedFileId.ToString();
        //        msg.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.gmail.com";
        //        smtp.EnableSsl = true;
        //        NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = networkCredential;
        //        smtp.Port = 587;
        //        smtp.Send(msg);
        //        return Json("");
        //    }s
        //}
        public JsonResult SendVerifyLinkByEmail(string Email, string fileId, int expday)
        {
            using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
            {
                msg.From = new MailAddress("rohan153555@gmail.com", "Team zipSign");
                string Emailid = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Email));
                string File = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(fileId));
                msg.Subject = "Document for signing";
                string mss = "http://localhost:50460/Login/SignLogin";
                string urlWithEncodedFileId = mss + "?Emailid=" + Emailid + "&File=" + File;
                DateTime expirationDate = DateTime.Today.AddDays(expday);
                string linkText = "Click here to view & sign the document";
                string linkHtml = $"<a href=\"{urlWithEncodedFileId}\">{linkText}</a>";
                string messageWithExpiration = $"This link will expire on {expirationDate:d}. Please make sure to sign the document before that date.";
                msg.Body = $"{linkHtml}<br/><br/>{messageWithExpiration}";
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 587;
                smtp.Send(msg);

                return Json("");
            }
        }

        //public JsonResult SendEmailAfterSuccess(string Email, string SignerName, int SignerID, string FilePath, string UploadedDocumentId)
        //{
        //    List<DataItems> obj = new List<DataItems>();
        //    obj.Add(new DataItems("UploadedDocumentId", UploadedDocumentId));
        //    obj.Add(new DataItems("QuerySelector", "SendMultiEmail"));
        //    statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);

        //    if (statusClass.StatusCode == 1)
        //    {
        //        Email = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"]);
        //    }
        //    for (int i = 0; i <= Email.Count(); i++)
        //    {
        //        using (MailMessage msg = new MailMessage("rohan153555@gmail.com", Email))
        //        {
        //            //Email = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Email));
        //            //fileid = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(fileid));
        //            //SignerName = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(SignerName));
        //            ////SignerID = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToInt32(SignerID));
        //            //FilePath = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(FilePath));
        //            //UploadedDocumentId = AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(UploadedDocumentId));
        //            msg.Subject = "Document Sign Confirmation";
        //            string messageWithExpiration = $"Dear User,";
        //            messageWithExpiration += "";
        //            messageWithExpiration += "Thankyou for signing the document.";
        //            messageWithExpiration += "You can download the document, from the attached link.";
        //            string mss = "http://localhost:50460/";
        //            //string mss = "https://uataadharsign.zipsign.in/Login/SignLogin";
        //            string urlWithEncodedFileId = mss + "?&File=" + FilePath;
        //            //DateTime expirationDate = DateTime.Today.AddDays(3);
        //            string linkText = "Click here to download the document";
        //            string linkHtml = $"<a href=\"{urlWithEncodedFileId}\">{linkText}</a>";
        //            msg.Body = $"{linkHtml}<br/><br/>{messageWithExpiration}";
        //            msg.IsBodyHtml = true;
        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = "smtp.gmail.com";
        //            smtp.EnableSsl = true;
        //            NetworkCredential networkCredential = new NetworkCredential("rohan153555@gmail.com", "rojrxjrxxynojgyx");
        //            smtp.UseDefaultCredentials = true;
        //            smtp.Credentials = networkCredential;
        //            smtp.Port = 587;
        //            smtp.Send(msg);

        //        }
        //    }
        //    return Json("");
        //}


        public ActionResult SignOut(string UserMasterID)
        {
            List<DataItems> obj = new List<DataItems>();
            string clientIP = GetClientIP();
            obj.Add(new DataItems("LogOut_IP_Address", clientIP));
            obj.Add(new DataItems("UserMasterID", UserMasterID));
            obj.Add(new DataItems("QueryType", "SignOut"));
            statusClass = bal.PostFunction(pro.Signup, obj);
            return Json(new { success = true });
        }

    }
}





