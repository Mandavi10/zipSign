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
        private readonly BusinessAccesslayerClass bal = new BusinessAccesslayerClass();
        private CommonStatus statusClass = new CommonStatus();

        //Security objSecurity = new Security();
        private readonly ProcMaster pro = new ProcMaster();
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
        //Function for SignUp
        [HttpPost]
        public JsonResult SignUp(SignUp objSignUpModel, string UserType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { status = "Validation failed", errors });
                }
                if (string.IsNullOrWhiteSpace(objSignUpModel.Name))
                {
                    return Json(new { status = "Enter Name" });
                }
                if (!IsValidEmail(objSignUpModel.Email))
                {
                    return Json(new { status = "Invalid email format" });
                }
                if (!IsValidMobile(objSignUpModel.Mobile))
                {
                    return Json(new { status = "Mobile Number should be 10 Digits and only starts with 6/7/8/9" });
                }
                if (objSignUpModel.State == "Select State")
                {
                    return Json(new { status = "Please select state" });
                }
                if (!IsValidPassword(objSignUpModel.Password))
                {
                    return Json(new { status = "Password must contain one lowercase letter, one uppercase letter, one numeric digit, at least 8 characters, and one special character" });
                }
                if (!IsValidConfirmPassword(objSignUpModel.ConfirmPassword))
                {
                    return Json(new { status = "Invalid Confirm password format" });
                }
                if (objSignUpModel.Password != objSignUpModel.ConfirmPassword)
                {
                    return Json(new { status = "Password and Confirm Password do not match" });
                }
                if (UserType == "corporate")
                {
                    if (!IsValidPAN(objSignUpModel.panNumber))
                    {
                        return Json(new { status = "Invalid PAN format" });
                    }
                }
                SignUp Data = new SignUp();
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("Name", objSignUpModel.Name),
                    new DataItems("Email", objSignUpModel.Email),
                    new DataItems("MobileNumber", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objSignUpModel.Mobile))),
                    new DataItems("MobileOTP", objSignUpModel.OTP),
                    new DataItems("State", objSignUpModel.State),
                    new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objSignUpModel.Password))),
                    new DataItems("UserType", UserType)
                };
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
            // Check if the email is null or empty
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            // Use regex to validate the email format
            string pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidMobile(string mobile)
        {
            //Check if the mobile no is null or empty
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return false;
            }
            // Use regex to validate the mobile number format (e.g., allow only digits)
            string pattern = @"^\d{10}$";
            return Regex.IsMatch(mobile, pattern);
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
            return Regex.IsMatch(password, pattern);
        }
        private bool IsValidConfirmPassword(string ConfirmPassword)
        {
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                return false;
            }
            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
            return Regex.IsMatch(ConfirmPassword, pattern);


        }

        private bool IsValidPAN(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan))
            {
                return false;
            }
            string pattern = @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$";
            return Regex.IsMatch(pan, pattern);
        }
        //Function for Login
        [HttpPost]
        public JsonResult Login(Login objLoginModel, string captchaInput)
        {
            try
            {
                if (string.IsNullOrEmpty(objLoginModel.Email))
                {
                    return Json(new { status = "Email/Mobile can't Empty" });
                }
                else if (string.IsNullOrEmpty(objLoginModel.Password))
                {
                    return Json(new { status = "Password can't Empty" });
                }
                else
                {
                    string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
                    if (!Regex.IsMatch(objLoginModel.Password, pattern))
                    {
                        return Json(new { status = "Invalid password format" });
                    }
                }
                if (string.IsNullOrEmpty(captchaInput))
                {
                    return Json(new { status = "Please enter captcha" });
                }
                string expectedCaptcha = Session["CAPTCHA"] as string;
                bool isCaptchaValid = string.Equals(captchaInput, expectedCaptcha, StringComparison.Ordinal);
                if (isCaptchaValid)
                {
                    SignUp Data = new SignUp();
                    List<Login> result = new List<Login>();
                    List<DataItems> obj = new List<DataItems>
            {
                new DataItems("Email", objLoginModel.Email),
                new DataItems("MobileNumber", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objLoginModel.Mobile))),
                new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(objLoginModel.Password))),
                new DataItems("QueryType", "LoginData")
            };
                    statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
                    if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
                    {
                        List<profile> userDataList = new List<profile>(); // Create a list to store user data
                        foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                        {
                            result.Add(new Login
                            {
                                UserName = Convert.ToString(dr["Name"]),
                                Email = Convert.ToString(dr["Email"]),
                                Mobile = Convert.ToString(dr["MobileNumber"]),
                                UserId = Convert.ToString(dr["UserMasterID"]),
                            });
                            profile userData = new profile
                            {
                                UserName = Convert.ToString(dr["Name"]),
                                Email = Convert.ToString(dr["Email"]),
                                Mobile = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(dr["MobileNumber"])),
                                UserId = Convert.ToString(dr["UserMasterID"]),
                            };

                            userDataList.Add(userData);
                            this.Session["UserId"] = statusClass.DataFetch.Tables[0].Rows[0]["UserMasterID"];

                        }
                        this.Session["UserData"] = userDataList; // Store the list of user data in the session

                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // IncrementWrongLoginAttempts();
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

        public JsonResult GetUserProfile()
        {
            // Retrieve user profile data from the session
            var userData = Session["UserData"] as List<profile>;

            if (userData != null)
            {
                return Json(userData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "error", message = "User data not found" });
            }
        }


        //public static string GetClientIP()
        //{
        //    string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(ip))
        //    {
        //        string[] ipRange = ip.Split(',');
        //        return ipRange[0].Trim();
        //    }
        //    else
        //    {
        //        return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //}

        //public static string GetClientIP()
        //{
        //    using (var client = new WebClient())
        //    {
        //        // Use a public IP address service to get your public IP
        //        string publicIP = client.DownloadString("https://api64.ipify.org?format=text");
        //        return publicIP;
        //    }
        //}
        public static string GetClientIP()
        {
            try
            {
                string publicIP = string.Empty;

                // Check for Internet connectivity
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    using (var client = new WebClient())
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
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("UserName", Sm.CusName),
                new DataItems("Phoneno", Sm.MobileNo),
                new DataItems("OTP", OTP1),
                new DataItems("QueryType", "GetSMSData")
            };
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
            Random random = new Random();
            int txnId = random.Next(100, 1000);
            string TraceNumber = "612000" + DateTime.Now.ToString("ddMMyyyyHHmmss") + txnId;
            List<DataItems> obj1 = new List<DataItems>
            {
                new DataItems("TxnId", TraceNumber),
                new DataItems("Otp", OTP1),
                new DataItems("GeneratedUsing",  Sm.MobileNo),
                new DataItems("ActionType","SignUp"),
                new DataItems("QueryType", "OtpExpired"),

            };
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj1);

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

        //For Signin

        public ActionResult SendOTP(string CusName, string MobileNo, string Email)
        {
            string PhoneNumber = AESEncryption.AESEncryptionClass.DecryptAES(MobileNo);
            Random rnd = new Random();
            string OTP = rnd.Next(100000, 999999).ToString();
            Session["otp"] = OTP;
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
            var result = new
            {
                status = "201",
                message = "OTP sent successfully to both phone and email."
            };
            return Json(result, MobileNo, JsonRequestBehavior.AllowGet);
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
                message += "<p>This password is valid for 10 minutes to complete sign-in, requested 07 September, 2023 at 12:01 PM IST.</p>";
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
                message += "<p>Dear User,</p>";
                message += "<p>Below is your One-Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>This password is valid for 10 minutes to complete sign-in, requested 07 September, 2023 at 12:01 PM IST.</p>";
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
                return Json("OTP sent successfully!", OTP);
            }
        }

        //For SignLogin
        public JsonResult GetEmailDataForSignLogin(string Email)
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
                message += "<p>Dear User,</p>";
                message += "<p>To complete your registration and enjoy all the benefits of our service, please verify your email address by entering the below One Time Password:</p>";
                message += "<h1 style='color: #007BFF;'>" + OTP + "</h1>";
                message += "<p>Once your email is verified, you'll have full access to your account and can start signing documents securely.</p>";
                message += "<p class='disclaimer'>If you have any questions or need assistance, please don't hesitate to contact our support team at customersupport@zipsign.com.</p>";
                message += "<p>Thank you for choosing zipSign!</p>";
                message += "<p class='disclaimer'>Sincerely,</p>";
                message += "<p class='disclaimer'>Customer Support</p>";
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

                return Json("OTP sent successfully!", OTP);
            }
        }

        //For SignUp Mobile OTP
        public JsonResult VerifyOTP(string VOTP)
        {
         List<DataItems> obj1 = new List<DataItems>
        {
        new DataItems("Otp", VOTP),
        new DataItems("QueryType", "IsValidOTP")
            };
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj1);

            if (statusClass.StatusCode == 9)
            {
                DateTime otpGeneratedTime = (DateTime)statusClass.DataFetch.Tables[0].Rows[0]["CreatedOn"];
                DateTime currentTime = DateTime.Now;
                TimeSpan timeDifference = currentTime - otpGeneratedTime;

                if (timeDifference.TotalMinutes > 10)
                {
                    // OTP has expired
                    return Json(0); // Return a status code indicating expired OTP
                }
                else
                {
                    // OTP is within the valid timeframe, compare with the user input
                    string storedOTP = statusClass.DataFetch.Tables[0].Rows[0]["Otp"].ToString();
                    if (storedOTP == VOTP)
                    {
                        string LoginIPAddress = GetClientIP();
                        object UserMasterId = Session["UserId"];
                        List<DataItems> obj = new List<DataItems>();
                        obj.Add(new DataItems("UserMasterID", UserMasterId));
                        obj.Add(new DataItems("Login_IP_Address", LoginIPAddress));
                        obj.Add(new DataItems("EmailOTP", VOTP));
                        obj.Add(new DataItems("QueryType", "LoginOTP"));

                        statusClass = bal.PostFunction(pro.Signup, obj);
                        return Json(1); // OTP is valid, return a success status
                    }
                    else
                    {
                        return Json(2); // Invalid OTP, return a failure status
                    }
                }
            }
            else
            {
                return Json(2); // Return a status code indicating no OTP record found
            }
        }

        public JsonResult VerifyMobileOTP(string VOTP)
        {
            string temp = Session["otp"].ToString();
            int msg;
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


        public JsonResult VerifyMobile(string VOTP)
        {
            _ = GetClientIP();

            string temp = Session["otp"].ToString();
            int msg;
            if (temp == VOTP)
            {
                msg = 1;
            }
            else
            {
                msg = 2;
            }
            return Json(new { msg, }, JsonRequestBehavior.AllowGet);

            //return Json(msg, FilePath);
        }


        public JsonResult VerifyEmailOTP(string VOTP)
        {
            List<DataItems> obj = new List<DataItems>
        {
        new DataItems("Otp", VOTP),
        new DataItems("QueryType", "IsValidOTP")
        };

            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);

            // Check if OTP record exists in the database response
            if (statusClass.StatusCode == 9)
            {
                DateTime otpGeneratedTime = (DateTime)statusClass.DataFetch.Tables[0].Rows[0]["CreatedOn"];
                DateTime currentTime = DateTime.Now;
                TimeSpan timeDifference = currentTime - otpGeneratedTime;

                if (timeDifference.TotalMinutes > 10)
                {
                    // OTP has expired
                    return Json(0); // Return a status code indicating expired OTP
                }
                else
                {
                    // OTP is within the valid timeframe, compare with the user input
                    string storedOTP = statusClass.DataFetch.Tables[0].Rows[0]["Otp"].ToString();
                    if (storedOTP == VOTP)
                    {
                        return Json(1); // OTP is valid
                    }
                    else
                    {
                        return Json(2); // Invalid OTP
                    }
                }
            }
            else
            {
                return Json(2); // Return a status code indicating no OTP record found
            }
        }
        //For Mobile SignUp OTP
        public JsonResult VerifyMobileOTPForSignUp(string VOTP)
        {
            List<DataItems> obj = new List<DataItems>
        {
        new DataItems("Otp", VOTP),
        new DataItems("QueryType", "IsValidOTP")
       };

            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);

            // Check if OTP record exists in the database response
            if (statusClass.StatusCode == 9)
            {
                DateTime otpGeneratedTime = (DateTime)statusClass.DataFetch.Tables[0].Rows[0]["CreatedOn"];
                DateTime currentTime = DateTime.Now;
                TimeSpan timeDifference = currentTime - otpGeneratedTime;

                if (timeDifference.TotalMinutes > 10)
                {
                    // OTP has expired
                    return Json(0); // Return a status code indicating expired OTP
                }
                else
                {
                    // OTP is within the valid timeframe, compare with the user input
                    string storedOTP = statusClass.DataFetch.Tables[0].Rows[0]["Otp"].ToString();
                    if (storedOTP == VOTP)
                    {
                        return Json(1); // OTP is valid
                    }
                    else
                    {
                        return Json(2); // Invalid OTP
                    }
                }
            }
            else
            {
                return Json(2); // Return a status code indicating no OTP record found
            }
        }



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

                return Json("");
            }
        }


        public ActionResult SignOut(string UserMasterID)
        {

            List<DataItems> obj = new List<DataItems>();
            string clientIP = GetClientIP();
            obj.Add(new DataItems("LogOut_IP_Address", clientIP));
            obj.Add(new DataItems("UserMasterID", UserMasterID));
            obj.Add(new DataItems("QueryType", "SignOut"));
            statusClass = bal.PostFunction(pro.Signup, obj);
            Session.Abandon();
            return Json(new { success = true });
        }


        public ActionResult ResetPassword(string Email, string captchaInput)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return Json(new { status = "Email/Mobile can't Empty" });
            }
            else
            {
                string pattern = @"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$";
                if (!Regex.IsMatch(Email, pattern))
                {
                    return Json(new { status = "Invalid Email format" });
                }
            }
            if (string.IsNullOrEmpty(captchaInput))
            {
                return Json(new { status = "Please enter captcha" });
            }
            string expectedCaptcha = Session["CAPTCHA"] as string;
            bool isCaptchaValid = string.Equals(captchaInput, expectedCaptcha, StringComparison.Ordinal);
            if (isCaptchaValid)
            {
                List<DataItems> obj = new List<DataItems>();
                //string clientIP = GetClientIP();
                //obj.Add(new DataItems("IP", clientIP));
                obj.Add(new DataItems("Email", Email));
                obj.Add(new DataItems("QueryType", "GetDataForUser"));
                statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
                string UserCode = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["UserMasterId"]);
                string UserEmail = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"]);
                return Json(new { statusClass.StatusCode, UserCode, UserEmail });
            }
            else
            {
                return Json(new { status = "Invalid captcha input" });
            }
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

            return $"http://localhost:50460/Login/ChangePassword?UserCode={userCode}";
        }
        private void InsertLinkIntoDatabase(string userCode, string email, DateTime createdOn, DateTime expiryTime, string LinkText)
        {
            List<DataItems> obj = new List<DataItems>();

            obj.Add(new DataItems("Email", email));
            obj.Add(new DataItems("CreatedOn", createdOn));
            obj.Add(new DataItems("CreatedBy", userCode));
            obj.Add(new DataItems("ExpiredOn", expiryTime));
            obj.Add(new DataItems("Link", LinkText));
            obj.Add(new DataItems("QueryType", "SendLink"));
            statusClass = bal.PostFunction(pro.Signup, obj);
        }
        [HttpGet]
        public ActionResult GetDataForPasswordReset(string userCode)
        {
            string DecUserCode = AESEncryption.AESEncryptionClass.DecryptAES(Convert.ToString(userCode));

            List<DataItems> obj = new List<DataItems>();
            obj.Add(new DataItems("CreatedBy", DecUserCode));
            obj.Add(new DataItems("QueryType", "GetPasswordData"));
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            string CreatedOn = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["CreatedOn"]);
            string CreatedBy = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["CreatedBy"]);
            string IsExpired = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["IsExpired"]);
            string ExpiredOn = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["ExpiredOn"]);
            string Email = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Email"]);
            return Json(new { statusClass.StatusCode, CreatedOn, CreatedBy, IsExpired, ExpiredOn, Email }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdatePassword(string userCode, string Email, string OldPassword, string NewPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                return Json(new { error = "All fields are required." }, JsonRequestBehavior.AllowGet);
            }
            if (NewPassword != confirmPassword)
            {
                return Json(new { error = "Password and confirm password do not match." }, JsonRequestBehavior.AllowGet);
            }
            string passwordPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
            if (!Regex.IsMatch(NewPassword, passwordPattern))
            {
                return Json(new { error = "Password must have at least one digit, one lowercase letter, one uppercase letter, one special character, and be at least 8 characters long." }, JsonRequestBehavior.AllowGet);
            }
            //if (IsNewPasswordInHistory(userCode, NewPassword))
            //{
            //    return Json(new { error = "Password cannot be one of the last five passwords." }, JsonRequestBehavior.AllowGet);
            //}
            string clientIP = GetClientIP();
            string EncNewPassword = AESEncryption.AESEncryptionClass.EncryptAES(NewPassword);
            string EncNewPassword1 = AESEncryption.AESEncryptionClass.DecryptAES(EncNewPassword);
            List<DataItems> obj = new List<DataItems>();
            obj.Add(new DataItems("UserMasterID", userCode));
            obj.Add(new DataItems("Email", Email));
            obj.Add(new DataItems("NewPassword", EncNewPassword));
            obj.Add(new DataItems("CreatedBy", userCode));
            obj.Add(new DataItems("IP", clientIP));
            obj.Add(new DataItems("QueryType", "ForgotPassword"));
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);


            // Assuming your statusClass contains information about the result of the password update operation
            if (statusClass.StatusCode == 7)
            {
                return Json(new { success = "Password updated successfully." }, JsonRequestBehavior.AllowGet);
            }
            else if (statusClass.StatusCode == 10)
            {
                return Json(new { error = "New password should not match your last five password." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Failed to update password." }, JsonRequestBehavior.AllowGet);
            }

        }


        public ActionResult ChangePasswordFromProfile(string oldPassword, string newPassword, string confirmPassword, string email, string UserMasterId)
        {
            string passwordPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
            if (string.IsNullOrWhiteSpace(oldPassword))
            {
                return Json(new { error = "Enter Old Password" }, JsonRequestBehavior.AllowGet);
            }
            else if (!Regex.IsMatch(oldPassword, passwordPattern))
            {
                return Json(new { error = "Password must have at least one digit, one lowercase letter, one uppercase letter, one special character, and be at least 8 characters long." }, JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrWhiteSpace(newPassword))
            {
                return Json(new { error = "Enter New Password" }, JsonRequestBehavior.AllowGet);
            }
            else if (!Regex.IsMatch(newPassword, passwordPattern))
            {
                return Json(new { error = "Password must have at least one digit, one lowercase letter, one uppercase letter, one special character, and be at least 8 characters long." }, JsonRequestBehavior.AllowGet);
            }
            else if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                return Json(new { error = "Enter Confirm Password" }, JsonRequestBehavior.AllowGet);
            }
            else if (!Regex.IsMatch(confirmPassword, passwordPattern))
            {
                return Json(new { error = "Password must have at least one digit, one lowercase letter, one uppercase letter, one special character, and be at least 8 characters long." }, JsonRequestBehavior.AllowGet);
            }
            else if (confirmPassword != newPassword)
            {
                return Json(new { error = "Password and confirm password do not match." }, JsonRequestBehavior.AllowGet);
            }
            string clientIP = GetClientIP();
            List<DataItems> obj = new List<DataItems>();
            string EncNewPassword = AESEncryption.AESEncryptionClass.EncryptAES(newPassword);
            string EncOldPassword = AESEncryption.AESEncryptionClass.EncryptAES(oldPassword);
            obj.Add(new DataItems("UserMasterID", UserMasterId));
            obj.Add(new DataItems("NewPassword", EncNewPassword));
            obj.Add(new DataItems("Email", email));
            obj.Add(new DataItems("OldPassword", EncOldPassword));
            obj.Add(new DataItems("IP", clientIP));
            obj.Add(new DataItems("QueryType", "ChangePassword"));
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            if (statusClass.StatusCode == 7)
            {
                return Json(new { success = "Password updated successfully." }, JsonRequestBehavior.AllowGet);
            }
            else if (statusClass.StatusCode == 10)
            {
                return Json(new { success = "Incorrect Old Password" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = "User Not Found" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}





