using BusinessDataLayer;
using BusinessLayerModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
namespace zipSign.Controllers.APIs
{
    public class StateManagementForSignupController : ApiController
    {
        public ProcMaster pro = new ProcMaster();
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        [HttpPost]
        [Route("StateManagementForSignup/GetStates")]
        public IHttpActionResult GetStates([FromBody] JObject requestData)
        {
            QType Data = requestData["Data"].ToObject<QType>();
            if (Data.QueryType == "GetAllState")
            {
                List<string> states = new List<string>();
                List<DataItems> obj = new List<DataItems>();
                obj.Add(new DataItems("QueryType", "GetAllState"));
                statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
                if (statusClass.StatusCode == 9)
                {
                    foreach (DataRow row in statusClass.DataFetch.Tables[0].Rows)
                    {
                        states.Add(Convert.ToString(row["State"]));
                    }
                    var stateObjects = states.Select(state => new { name = state }).ToList();
                    var response = new
                    {
                        status = true,
                        states = stateObjects,
                        message = "States Fetching Successfully"
                    };
                    return Json(response);
                }
                else
                {
                    var response = new
                    {
                        status = false,
                        message = "Failed to retrieve states data."
                    };
                    return Json(response);
                }
            }
            else
            {
                var response = new
                {
                    status = false,
                    message = "Invalid QueryType."
                };
                return Json(response);
            }
        }
        public class QType
        {
            public string QueryType { get; set; }
        }
    }
}

