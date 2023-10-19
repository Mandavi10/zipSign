﻿using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;


namespace zipSign.Controllers.APIs
{
    public class SignUpAPIController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();


        [HttpPost]
        [Route("SignUpAPI/SignUp")]
        public IHttpActionResult SignUp([FromBody] JObject requestData)
        {
            SignUp Data = requestData["Data"].ToObject<SignUp>();
            string UserType = requestData["UserType"].ToString();
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { status = "Validation failed", errors });
            }
            if (string.IsNullOrWhiteSpace(Data.Name))
            {
                return Json(new { statuscode = "SU1", status = "Enter Name" });
            }
            if (!IsValidEmail(Data.Email))
            {
                return Json(new { statuscode = "SU2", status = "Invalid email format" });
            }
            if (!IsValidMobile(Data.Mobile))
            {
                return Json(new { statuscode = "SU3", status = "Mobile Number should be 10 Digits and only starts with 6/7/8/9" });
            }
            if (Data.State == "Select State")
            {
                return Json(new { statuscode = "SU4", status = "Please select state" });
            }
            if (!IndianStates.Contains(Data.State))
            {
                return Json(new { statuscode = "SU8", status = "Please select a valid Indian state" });
            }
            if (!IsValidPassword(Data.Password))
            {
                return Json(new { statuscode = "SU5", status = "Password must contain one lowercase letter, one uppercase letter, one numeric digit, at least 8 characters, and one special character" });
            }
            if (!IsValidConfirmPassword(Data.ConfirmPassword))
            {
                return Json(new { statuscode = "SU6", status = "Invalid Confirm password format" });
            }
            if (Data.Password != Data.ConfirmPassword)
            {
                return Json(new { statuscode = "SU7", status = "Password and Confirm Password do not match" });
            }
            if (UserType == "corporate")
            {
                if (!IsValidPAN(Data.panNumber))
                {
                    return Json(new { statuscode = "SU8", status = "Invalid PAN format" });
                }
            }
            else
            {
                List<DataItems> obj = new List<DataItems>
            {
                new DataItems("Name", Data.Name),
                new DataItems("Email", Data.Email),
                new DataItems("MobileNumber", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Data.Mobile))),
                new DataItems("MobileOTP", Data.OTP),
                new DataItems("State", Data.State),
                new DataItems("Password", AESEncryption.AESEncryptionClass.EncryptAES(Convert.ToString(Data.Password))),
                new DataItems("UserType", UserType)
            };
                obj.Add(new DataItems("QueryType", "InsertRecord"));
                statusClass = bal.PostFunction(pro.Signup, obj);

                if (statusClass.StatusCode == 1)
                {
                    var result = new
                    {
                        StatusCode = statusClass.StatusCode,
                        message = "SignUp Succcessfully"
                    };
                    return Json(result);
                }
                else if (statusClass.StatusCode == 2)
                {
                    var result = new
                    {
                        StatusCode = statusClass.StatusCode,
                        message = "Duplicate PAN Number"// Duplicate PAN Number
                    };
                    return Json(result);
                }
                else if (statusClass.StatusCode == 3)
                {
                    var result = new
                    {
                        StatusCode = statusClass.StatusCode,
                        message = "Duplicate Phone Number"// Duplicate Phone 
                    };
                    return Json(result);
                }
                else if (statusClass.StatusCode == 4)
                {
                    var result = new
                    {
                        StatusCode = statusClass.StatusCode,
                        message = "Duplicate Email"//Duplicate Email
                    };
                    return Json(result);
                }
            }
            return Json("");
        }
        private bool IsValidEmail(string email)
        {
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
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return false;
            }
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
        public static List<string> IndianStates = new List<string>
    {
        "Andaman and Nicobar Islands",
            "Andhra Pradesh",
            "Arunachal Pradesh",
            "Assam",
            "Bihar",
            "Chandigarh",
            "Chhattisgarh",
            "Chennai",
            "Dadra & Nagar Haveli and Daman & Diu",
            "Delhi",
            "Goa",
            "Gujarat",
            "Haryana",
            "Himachal Pradesh",
            "Jammu and Kashmir",
            "Jharkhand",
            "Karnataka",
            "Kerala",
            "Ladakh",
            "Lakshadweep Islands",
            "Madhya Pradesh",
            "Maharashtra",
            "Manipur",
            "Meghalaya",
            "Mizoram",
            "Nagaland",
            "Odisha",
            "Pondicherry",
            "Punjab",
            "Rajasthan",
            "Sikkim",
            "Tamil Nadu",
            "Telangana",
            "Tripura",
            "Uttar Pradesh",
            "Uttarakhand",
            "West Bengal"
    };
        private class ApiResponse
        {
            public string Status { get; set; }
            public object Data { get; set; }
        }
    }
}