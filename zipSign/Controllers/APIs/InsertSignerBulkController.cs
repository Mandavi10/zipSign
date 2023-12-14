using BusinessDataLayer;
using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Data;

namespace zipSign.Controllers.APIs
{
    public class InsertSignerBulkController : ApiController
    {
        public ProcMaster pro = new ProcMaster();
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        public IHttpActionResult SignInsert([FromBody] SignInsertRequest request)
        {
            try
            {
                //if (Session["UserId"] == null)
                //{
                //    return Unauthorized();
                //}

                // Validation and authorization checks...

                int uploadedDocumentId = InsertDocument(request);

                // ... Other processing logic ...

                return Ok(new { UploadedDocumentId = uploadedDocumentId, Message = "Bulk insertion successful" });
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (log, return error response, etc.)
                return InternalServerError(ex);
            }
        }

        private int InsertDocument(SignInsertRequest request)
        {
            int i = 0;
            if (request.UserType == "Multiple Signers")
            {
                var signerInfoTable = new DataTable();
                signerInfoTable.Columns.Add("SignerName", typeof(string));
                signerInfoTable.Columns.Add("UploadedDocumentId", typeof(int));
                signerInfoTable.Columns.Add("SignerEmail", typeof(string));

                foreach (SignerInfo signer in request.SignerInfos)
                {
                    signerInfoTable.Rows.Add(
                        signer.Name,
                        //uploadedDocumentId,
                        signer.Email,
                        signer.MobileNumber,
                        signer.ExpireInDays,
                        signer.SignerType,
                        signer.DocumentExpiryDay,
                        0, 
                        "UniqueID"
                        //Convert.ToInt32(Session["UserId"])
                    );
                }

                List<DataItems> objBulkInsert = new List<DataItems>
            {
                new DataItems("SignerInfoTable", signerInfoTable),
                new DataItems("QuerySelector", "InsertSignerBulk")
            };

                //statusClass = bal.PostFunction(pro.Sp_SignUpload, objBulkInsert);
                statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, objBulkInsert);
                string EmailToSend = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerEmail"]);
                string SignerID = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerID"]);
                string SignerName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerName"]);
                string SignerExpiry = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["SignerExpiryDay"]);
                 i = 0;
            }

            return i;
        }
    }

    public class SignInsertRequest
    {
        public SignMaster SignMaster { get; set; }
        public string UserType { get; set; }
        public List<SignerInfo> SignerInfos { get; set; }
    }

    public class SignerInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ExpireInDays { get; set; }
        public string SignerType { get; set; }
        public string DocumentExpiryDay { get; set; }
    }
}

