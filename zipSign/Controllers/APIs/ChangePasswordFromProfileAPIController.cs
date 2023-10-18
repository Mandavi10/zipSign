using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class ChangePasswordFromProfileAPIController : ApiController
    {
        private readonly ProcMaster pro = new ProcMaster();
        private readonly BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        [Route("ChangePasswordFromProfile/ChangePassword")]
        public IHttpActionResult ChangePassword([FromBody] JObject requestData)
        {
            ChangePasswordFromProfileModel Data = requestData["Data"].ToObject<ChangePasswordFromProfileModel>();
            string passwordPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
            int UserMasterId = GetUserMasterIdByEmail(Data.email);
            if (string.IsNullOrWhiteSpace(Data.oldPassword))
            {
                return Json(new { statuscode = "CPFP1", status = "Please Enter Old Password" });
            }
            else if (!Regex.IsMatch(Data.oldPassword, passwordPattern))
            {
                return Json(new { statuscode = "CPFP2", status = "Please Enter Correct Old Password" });
            }
            else if (string.IsNullOrWhiteSpace(Data.newPassword))
            {
                return Json(new { statuscode = "CPFP3", status = "Please Enter New Password" });
            }
            else if (!Regex.IsMatch(Data.newPassword, passwordPattern))
            {
                return Json(new { statuscode = "CPFP4", status = "Please Enter Correct New Password" });
            }
            else if (string.IsNullOrWhiteSpace(Data.confirmPassword))
            {
                return Json(new { statuscode = "CPFP5", status = "Please Confirm Password" });
            }
            else if (Data.confirmPassword != Data.newPassword)
            {
                return Json(new { statuscode = "CPFP6", status = "Password Does Not Match" });
            }
            string clientIP = GetClientIP();
            List<DataItems> obj = new List<DataItems>();
            string EncNewPassword = AESEncryption.AESEncryptionClass.EncryptAES(Data.newPassword);
            string EncOldPassword = AESEncryption.AESEncryptionClass.EncryptAES(Data.oldPassword);
            obj.Add(new DataItems("UserMasterID", UserMasterId));
            obj.Add(new DataItems("NewPassword", EncNewPassword));
            obj.Add(new DataItems("Email", Data.email));
            obj.Add(new DataItems("OldPassword", EncOldPassword));
            obj.Add(new DataItems("IP", clientIP));
            obj.Add(new DataItems("QueryType", "ChangePassword"));
            statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
            if (statusClass.StatusCode == 7)
            {
                return Json(new { statuscode = "CPFP6", status = "Password updated successfully." });
            }
            else if (statusClass.StatusCode == 10)
            {
                return Json(new { statuscode = "CPFP7", status = "Incorrect Old Password" });
            }
            else
            {
                return Json(new { statuscode = "CPFP8", status = "User Not Found" });
            }
        }


        public class ChangePasswordFromProfileModel
        {
            public string oldPassword { get; set; }
            public string newPassword { get; set; }
            public string confirmPassword { get; set; }
            public string email { get; set; }
            public string UserMasterId { get; set; }
        }
        public static string GetClientIP()
        {
            try
            {
                string publicIP = string.Empty;
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    using (WebClient client = new WebClient())
                    {
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
        public int GetUserMasterIdByEmail(string email)
        {
            int userMasterId = -1; // Default value indicating failure or non-existence

            using (SqlConnection connection = new SqlConnection("Data Source=192.168.40.86;Initial Catalog=UATDocumentZipSign;Integrated Security=False;User ID=aditi;Password=gBg7$52F"))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT UserMasterID FROM TblUserMaster WHERE Email = '" + email + "'", connection);
                cmd.Parameters.AddWithValue("@Email", email);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    userMasterId = Convert.ToInt32(reader["UserMasterID"]);
                }

                reader.Close();
            }

            return userMasterId;
        }
    }
}

