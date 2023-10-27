using System;
using System.Collections.Generic;
using System.Data;

namespace BusinessLayerModel
{
    #region FOR MODEL
    
    public class SignUp
    {
        public string Name { get; set; }
        public string OTP { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string State { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string panNumber { get; set; }
        public string CorpName { get; set; }
        public object Ref1 { get; set; }
    }
    public class SMSModel

    {
        public string Ref1 { get; set; }
        public string CusName { get; set; }
        public string Mandateid { get; set; }

        public string MobileNo { get; set; }

        public string Message { get; set; }

        public string EntityId { get; set; }

        public string OTP { get; set; }

        public string EmailID { get; set; }


    }
    public class Response
    {
        public string TxnID { get; set; }
    }
    public class Login
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
    public class SignMaster
    {
        public List<SignerInfo> signerInfos { get; set; }
        public int DocumentUploadId { get; set; }
        public string DocumentName { get; set; }
        public string UploadedFileName { get; set; }
        public int ReferenceNumber { get; set; }
        public string UploadedDoc { get; set; }
        public string UploadedBy { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string SignStatus { get; set; }
        public string keyword { get; set; }
        public string filePath { get; set; }
        public int IsDocumentOpen { get; set; }
        public string DocumentOpenOn { get; set; }
        public string IsSigned { get; set; }
        public string DocumentName1 { get; set; }
        public string UploadedOn { get; set; }
        public string SignedOn { get; set; }
    }
    public class ResultData
    {
        public List<SignMaster> Table1 { get; set; }
        public List<pagination> Table2 { get; set; }

    }
    public class ResultDataForCertificate
    {
        public List<DSCCertificateMgt> Table1 { get; set; }
        public List<pagination> Table2 { get; set; }
        public string Error { get; set; }
    }
    public class ResultData1
    {
        public List<DeptMaster> Table1 { get; set; }
        public List<pagination> Table2 { get; set; }
    }
    public class ResultData2
    {
        public List<UserInsert1> Table1 { get; set; }
        public List<pagination> Table2 { get; set; }
    }
    public class DeptMaster
    {
        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public int ReferenceNumber { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string IsActive { get; set; }
    }
    public class pagination
    {
        public int pagecount { get; set; }
        public int count { get; set; }
        public int page { get; set; }
        public int size { get; set; }
        public string keyword { get; set; }
    }
    public class profile
    {
        public string UserName { get; set; }
        public int ProId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Add1 { get; set; }
        public string Add2 { get; set; }
        public int Pin { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string GST { get; set; }
        public string ComWebURL { get; set; }
        public string EDomian { get; set; }
        public byte[] UserPhoto { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string UserId { get; set; }
    }
    public class UserInsert1
    {
        public int Userid { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string UserType { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyOn { get; set; }
        public string Active { get; set; }
        public string Mobileapp { get; set; }
        public string SpecificDomaincontrol { get; set; }

    }
    public class SignerInfo
    {
        public string SignerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ExpireInDays { get; set; }
        public string signerType { get; set; }
        public string DocumentExpiryDay { get; set; }
    }

    public class NSDLGetSet
    {
        public string AuthMethod { get; set; }
        public string xml_get { get; set; }
        public string requestxml { get; set; }
        public bool IsOtpChecked { get; set; }
        public string HdnSameValue { get; set; }
        public object HdnFilePath { get; set; }
    }

    //DSC Signature
    public class DSCCertificateMgt
    {
        public string CertificateName { get; set; }
        public string CertificateType { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public string Role { get; set; }
        public string UploadedBy { get; set; }
        public string PasswordType { get; set; }
        public string UploadedOn { get; set; }
        public int Row { get; set; }
        public string IsActive1 { get; set; }
        public string Table { get; set; }
        //public List<CertificationManagement> Table { get; set; }
    }
    public class CertificationManagement
    {
        public int Userid { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
    }

    #endregion

    #region FOR COMMON STATUS AND PARAMETERS
    public class CommonStatus
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public DataSet DataFetch { get; set; }
        public int Errorid { get; set; }

        public DataTable FillDatatableWithParam(string v1, string v2, string v3, string v4, string v5, string traceNumber, string v6, string filePath)
        {
            throw new NotImplementedException();
        }
    }
    public class DataItems
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }
        public DataItems() { }
        public DataItems(string sName, dynamic sValue)
        {
            this.Name = sName;
            this.Value = sValue;
        }
    }
    public class DynamicParameters
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class CommonManger_Aadhaar
    {

        public DataTable FillDatatableWithParam(string v1, string v2, string v3, string v4, string v5, string v6, string mandateId, string msgId, string appId)
        {
            throw new NotImplementedException();
        }
    }


    public class AuthViewModel
    {
        public string File { get; set; }
        public string SignerName { get; set; }
        public string Fileid { get; set; }
        public string Emailid { get; set; }
        public string SignerID { get; set; }
        public string UniqueSignerID { get; set; }

        //public string documentid { get; set; }


        public int documentid { get; set; }
        public int Coordinates { get; set; }

        public string XmlData { get; set; }
        public string hdnmessageid { get; set; }
        public string UploadedDocumentId { get; set; }
        public string hdnappid { get; set; }

        // Add other properties as needed for the remaining form fields
    }


    #endregion

    #region FOR PROCEDURES AND CHOICES

    public class ProcMaster
    {

        public string Signup = "SPUserMaster";
        public string AdminLogin = "SP_SuperAdminLogin";
        public string Sp_SignUpload = "Sp_SignUpload";
        public string DepartmentMaster = "Sp_DepartmentMaster";
        public string USP_GetSMSData = "USP_GetSMSData";
        public string SP_UserProfile = "SP_UserProfile";
        public string Sp_UserCreation = "Sp_UserCreation";
        public string Sp_SignerMaster = "Sp_SignerMaster";
        public string SignatureResponseLog = "SignatureResponseLog";
        public string Sp_CertificateManagement = "Sp_CertificateManagement";
    }

    #endregion

    #region FOR SECURITY
    //public class Security
    //{
    //    public string Decrypt(string cipherText)
    //    {
    //        string EncryptionKey = System.Configuration.ConfigurationSettings.AppSettings["EncryptionKey"].ToString();
    //        byte[] cipherBytes = Convert.FromBase64String(cipherText);
    //        using (Aes encryptor = Aes.Create())
    //        {
    //            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
    //            encryptor.Key = pdb.GetBytes(32);
    //            encryptor.IV = pdb.GetBytes(16);
    //            using (MemoryStream ms = new MemoryStream())
    //            {
    //                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
    //                {
    //                    cs.Write(cipherBytes, 0, cipherBytes.Length);
    //                    cs.Close();
    //                }
    //                cipherText = Encoding.Unicode.GetString(ms.ToArray());
    //            }
    //        }
    //        return cipherText;
    //    }
    //    public string Encrypt(string clearText)
    //    {
    //        string EncryptionKey = System.Configuration.ConfigurationSettings.AppSettings["EncryptionKey"].ToString();
    //        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
    //        using (Aes encryptor = Aes.Create())
    //        {
    //            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
    //            encryptor.Key = pdb.GetBytes(32);
    //            encryptor.IV = pdb.GetBytes(16);
    //            using (MemoryStream ms = new MemoryStream())
    //            {
    //                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
    //                {
    //                    cs.Write(clearBytes, 0, clearBytes.Length);
    //                    cs.Close();
    //                }
    //                clearText = Convert.ToBase64String(ms.ToArray());
    //            }
    //        }
    //        return clearText;
    //    }
    //}
    #endregion


    //#region FOR SMS
    //public class SmsModel
    //{
    //    public void SendSMS(string mobile, string message)
    //    {
    //        string api = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["New_SMSAPI"]);
    //        api = api.Replace(";", "&");
    //        api = api.Replace("MOBILENUMBER", mobile);
    //        api = api.Replace("MOBILEMESSAGE", message);
    //        ServicePointManager.Expect100Continue = true;
    //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    //        HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(api);
    //        HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
    //        System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
    //        string responseString = respStreamReader.ReadToEnd();
    //        respStreamReader.Close();
    //        myResp.Close();
    //    }
    //}
    //#endregion
}
