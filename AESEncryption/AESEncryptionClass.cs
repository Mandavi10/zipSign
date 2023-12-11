using System;
using System.Security.Cryptography;
using System.Text;


namespace AESEncryption
{
    public class AESEncryptionClass
    {

        public static string EncryptAES(string Decryptvalue)
        {
            string key = Parameter.key;

            string IV = Parameter.IV;     // 16 chars=128 bytes

            byte[] textbytes = ASCIIEncoding.ASCII.GetBytes(Decryptvalue);
            AesCryptoServiceProvider acs = new AesCryptoServiceProvider
            {
                BlockSize = 128,
                KeySize = 256,
                Key = ASCIIEncoding.ASCII.GetBytes(key),
                IV = ASCIIEncoding.ASCII.GetBytes(IV),
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC
            };
            ICryptoTransform icrypt = acs.CreateEncryptor(acs.Key, acs.IV);
            byte[] enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
            icrypt.Dispose();
            string base64string = Convert.ToBase64String(enc).Replace('/', '|');
            return base64string;
        }
        public static string DecryptAES(string Encryptvalue)
        {
            string key = Parameter.key;
            try

            {

                string replacevalue = Encryptvalue.Replace('|', '/');
                string IV = Parameter.IV;     // 16 chars=128 bytes
                                              // string key = Parameter.key; // 32 char =256 bytes
                byte[] encbytes = Convert.FromBase64String(replacevalue);
                AesCryptoServiceProvider acs = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    Key = ASCIIEncoding.ASCII.GetBytes(key),
                    IV = ASCIIEncoding.ASCII.GetBytes(IV),
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC
                };
                ICryptoTransform icrypt = acs.CreateDecryptor(acs.Key, acs.IV);
                byte[] dec = icrypt.TransformFinalBlock(encbytes, 0, encbytes.Length);
                icrypt.Dispose();

                return ASCIIEncoding.ASCII.GetString(dec);
            }
            catch (Exception)
            {
                string replacevalue = Encryptvalue.Replace('|', '/').Replace(" ", "+");
                string IV = Parameter.IV;     // 16 chars=128 bytes
                                              //   string key = Parameter.key; // 32 char =256 bytes
                byte[] encbytes = Convert.FromBase64String(replacevalue);
                AesCryptoServiceProvider acs = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    Key = ASCIIEncoding.ASCII.GetBytes(key),
                    IV = ASCIIEncoding.ASCII.GetBytes(IV),
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC
                };
                ICryptoTransform icrypt = acs.CreateDecryptor(acs.Key, acs.IV);
                byte[] dec = icrypt.TransformFinalBlock(encbytes, 0, encbytes.Length);
                icrypt.Dispose();

                return ASCIIEncoding.ASCII.GetString(dec);
            }
        }
        public static string MD5Hash(string s)
        {
            using (MD5 provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(s)))
                {
                    builder.Append(b.ToString("x2").ToLower());
                }

                return builder.ToString();
            }
        }
    }
}
   