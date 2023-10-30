using BusinessAccessLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class UploadFileAPIController : ApiController
    {
        private readonly BusinessAccesslayerClass bal = new BusinessAccesslayerClass();
        private CommonStatus statusClass = new CommonStatus();
        private readonly ProcMaster pro = new ProcMaster();
        [HttpPost]
        public IHttpActionResult UploadFiles([FromBody] JObject requestData)
        {
            JsonRequestModel Data = requestData["Data"].ToObject<JsonRequestModel>();
            if (Data == null || string.IsNullOrEmpty(Data.DocumentName) || string.IsNullOrEmpty(Data.UploadedDoc) || string.IsNullOrEmpty(Data.Base64String) || string.IsNullOrEmpty(Data.UserType) || string.IsNullOrEmpty(Data.UserId) || string.IsNullOrEmpty(Data.UserName)||string.IsNullOrEmpty(Data.UserEmail))
            {
                return Json(new { Status = false, message = "Please Fill Required Fields" });
            }
            try
            {
                string randomKey = CreateRandomKey();
                byte[] bytes = Convert.FromBase64String(Data.Base64String);
                string originalFileName = Path.GetFileNameWithoutExtension(Data.FileName);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string fileName = $"{originalFileName}_{timestamp}_{randomKey}_{"MobApp"}.pdf";
                string filePath = HttpContext.Current.Server.MapPath("~/Uploads/SignUpload/" + fileName);
                File.WriteAllBytes(filePath, bytes);
                SignInsert(Data.DocumentName, Data.UploadedDoc, filePath, Data.UserType, Data.UserId, Data.UserName, Data.UserEmail, Data.ReferenceNumber, Data.DocumentName1);
                return Json(new { Status = true, message="Success"});
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, message = ex });
            }
        }

        public static string CreateRandomKey()
        {
            string _allowedChars = "09876543212345678901234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random((int)DateTime.Now.Ticks);
            char[] chars = new char[9];
            for (int i = 0; i < 9; i++)
            {
                chars[i] = _allowedChars[randNum.Next(_allowedChars.Length)];
            }
            return new string(chars);
        }


        public IHttpActionResult SignInsert(string DocumentName, string UploadedDoc, string filePath, string UserType, string UserId, string userName, string userEmail, int ReferenceNumber, string DocumentName1)
        {
            if (string.IsNullOrEmpty(DocumentName) || string.IsNullOrEmpty(UploadedDoc) || string.IsNullOrEmpty(filePath))
            {
                return Json(new { status = false, message = "User Not Found" });
            }
            else
            {
                List<DataItems> obj = new List<DataItems>();
                string UniqueSignerID = CreateRandomKey();
                string UniqueID = "612000" + UniqueSignerID;
                obj.Add(new DataItems("DocumentName", DocumentName1));
                obj.Add(new DataItems("UploadedDoc", UploadedDoc));
                obj.Add(new DataItems("UploadedFileName", DocumentName));
                obj.Add(new DataItems("ReferenceNumber", ReferenceNumber));
                obj.Add(new DataItems("filePath", filePath));
                obj.Add(new DataItems("SignerType", UserType));
                obj.Add(new DataItems("UploadedBy", UserId));
                obj.Add(new DataItems("IsSent", 0));
                obj.Add(new DataItems("QuerySelector", "InsertSign"));
                statusClass = bal.PostFunction(pro.Sp_SignUpload, obj);
                int UploadedDocumentId = statusClass.StatusCode;
                LogTrail("", "Document Upload", userName, userEmail, UploadedDocumentId, UserType);
                if (statusClass.StatusCode >= 0)
                {
                    List<DataItems> obj1 = new List<DataItems>
                    {
                        new DataItems("SignerName", userName),
                        new DataItems("SignerEmail", userEmail),
                        new DataItems("SignerType", UserType),
                        new DataItems("UniqueSignerID", UniqueID),
                        new DataItems("UploadedDocumentId", UploadedDocumentId),
                        new DataItems("UploadedBy", UserId),
                        new DataItems("QuerySelector", "InsertSigner")
                    };
                    statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj1);
                    return Json(new { status = true, message = "File Upload Success" });
                }
                else
                {
                    string SignerID = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerID"]);
                    return Json(new { status = false, message = "File Upload Failed" });
                }
            }
        }
        public void LogTrail(string SignerID, string description, string signername, string Email, int UploadedDocumentId, string UserType)
        {
            string connectionString = GlobalMethods.Global.DocSign.ToString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO TblSignerDetailTrailLog (UniqueSignerID, Action, UserName, EmailID,UploadedDocumentId,CreatedOn,UserType) " +
                                 "VALUES (@UniqueSignerID, @Action, @UserName, @EmailID,@UploadedDocumentId,@CreatedOn,@UserType)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UniqueSignerID", SignerID);
                        command.Parameters.AddWithValue("@Action", description);
                        command.Parameters.AddWithValue("@UserName", signername);
                        command.Parameters.AddWithValue("@EmailID", Email);
                        command.Parameters.AddWithValue("@UploadedDocumentId", UploadedDocumentId);
                        command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        command.Parameters.AddWithValue("@UserType", UserType);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Trail logged successfully.");
                        }
                        else
                        {
                            Console.WriteLine("Trail logging failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }



        public class JsonRequestModel
        {
            public string Base64String { get; set; }
            public string FileName { get; set; }
            public string DocumentName { get; set; }
            public string UploadedDoc { get; set; }
            public string UserType { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public int ReferenceNumber { get; set; }
            public string DocumentName1 { get; set; }

        }

    }
}
