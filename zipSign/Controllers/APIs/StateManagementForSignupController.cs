using BusinessDataLayer;
using BusinessLayerModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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
            var tokenHeader = Request.Headers.Authorization;
            if (tokenHeader == null || tokenHeader.Scheme.ToLower() != "bearer")
            {
                var response = new
                {
                    status = false,
                    message = "Authorization header is missing or invalid."
                };
                return Json(response);
            }
            QType Data = requestData["Data"].ToObject<QType>();

            string token = tokenHeader.Parameter;
            string a = GetUserIdFromToken(token);
            //var regex = "^[A-Za-z0-9\\-_.]+$";
            if (Data.QueryType != "GetAllState")
            {
                var response = new
                {
                    status = false,
                    message = "Invalid Query Type."
                };
                return Json(response);
            }
            if (Data == null)
            {
                var response = new
                {
                    status = false,
                    message = "Authorization has been denied for this request."
                };
                return Json(response);
            }

            //if (Data.Token == null)
            //{
            //    var response = new
            //    {
            //        status = false,
            //        message = "Authorization has been denied for this request."
            //    };
            //    return Json(response);
            //}
            //if (!Regex.IsMatch(Data.Token, regex))
            //{
            //    var response = new
            //    {
            //        status = false,
            //        message = "Authorization has been denied for this request."
            //    };
            //    return Json(response);

            //}
            //var token = Request.Headers.GetValues("Authorization").FirstOrDefault();

            switch (a)
            {
                case null:
                    Errors("The token header is malformed or not base64url-encoded.");
                    break;
                case "IDX10000":
                    Errors("The token header is malformed or not base64url-encoded.");
                    break;
                case "IDX10003":
                    Errors("The 'kid' (key identifier) of the token doesn't match the key identifier of any keys provided by the issuer.");
                    break;
                case "IDX10008":
                    Errors("The issuer (iss) is null or whitespace.");
                    break;
                case "IDX10205":
                    Errors("Lifetime validation failed. The 'nbf' (not before) claim is in the future.");
                    break;
                case "IDX10214":
                    Errors("The 'exp' (expiration time) claim of the token is invalid or not a number.");
                    break;
                case "IDX10223":
                    Errors("Lifetime validation failed. The token is expired.");
                    break;
                case "IDX10224":
                    Errors("Lifetime validation failed. The token is not yet valid.");
                    break;
                case "IDX10230":
                    Errors("The token does not have an audience (aud) claim.");
                    break;
                case "IDX10231":
                    Errors("The 'aud' claim is empty or only contains whitespace.");
                    break;
                case "IDX10232":
                    Errors("The 'aud' claim is not valid for this resource.");
                    break;
                case "IDX10234":
                    Errors("The signature is invalid. This is often caused by a mismatch between the signing key and the key used to validate the signature.");
                    break;
                case "IDX10235":
                    Errors("The algorithm specified in the token header is not supported.");
                    break;
                default:
                    Errors("An unknown IDX error occurred.");
                    break;
            }
            if (a == null || a == "")
            {
                var response = new
                {
                    status = false,
                    message = "Authorization has been denied for this request."
                };
                return Json(response);
            }
            if (a.Contains("The token is expired"))
            {
                var response = new
                {
                    status = false,
                    message = "The token is expired."
                };
                return Json(response);
            }
            if (Data.QueryType == "GetAllState")
            {
                List<string> states = new List<string>();
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("QueryType", "GetAllState")
                };
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
            public string Token { get; set; }
        }

        private ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("a1b2c3d4e5f6g7h8i9j0kA1B2C3D4E5F6G7H8I9J0");

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true, // Set to true if you want to validate issuer
                ValidIssuer = "Test", // Specify the valid issuer
                ValidateAudience = true, // Set to true if you want to validate audience
                ValidAudience = "Demo", // DemoSpecify the valid audience
                ValidateLifetime = true, // Set to true to validate token expiration
                ClockSkew = TimeSpan.Zero // Adjust the clock skew if needed
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Errors($"Security token expired: {ex.Message}");
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private string GetUserIdFromToken(string Token)
        {
            try
            {
                if (Token != null)
                {
                    var principal = ValidateJwtToken(Token);
                    if (principal != null)
                    {
                        var userIdClaim = principal.FindFirst(ClaimTypes.Name);
                        return userIdClaim?.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }
        private IHttpActionResult Errors(string errors)
        {
            var response = new
            {
                status = false,
                message = errors
            };
            return Json(response);
        }
    }
}
//using BusinessDataLayer;
//using BusinessLayerModel;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Web.Http;
//namespace zipSign.Controllers.APIs
//{
//    public class StateManagementForSignupController : ApiController
//    {
//        public ProcMaster pro = new ProcMaster();
//        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
//        private CommonStatus statusClass = new CommonStatus();
//        [HttpPost]
//        [Route("StateManagementForSignup/GetStates")]
//        public IHttpActionResult GetStates([FromBody] JObject requestData)
//        {
//            QType Data = requestData["Data"].ToObject<QType>();
//            if (Data.QueryType == "GetAllState")
//            {
//                List<string> states = new List<string>();
//                List<DataItems> obj = new List<DataItems>
//                {
//                    new DataItems("QueryType", "GetAllState")
//                };
//                statusClass = bal.GetFunctionWithResult(pro.Signup, obj);
//                if (statusClass.StatusCode == 9)
//                {
//                    foreach (DataRow row in statusClass.DataFetch.Tables[0].Rows)
//                    {
//                        states.Add(Convert.ToString(row["State"]));
//                    }
//                    var stateObjects = states.Select(state => new { name = state }).ToList();
//                    var response = new
//                    {
//                        status = true,
//                        states = stateObjects,
//                        message = "States Fetching Successfully"
//                    };
//                    return Json(response);
//                }
//                else
//                {
//                    var response = new
//                    {
//                        status = false,
//                        message = "Failed to retrieve states data."
//                    };
//                    return Json(response);
//                }
//            }
//            else
//            {
//                var response = new
//                {
//                    status = false,
//                    message = "Invalid QueryType."
//                };
//                return Json(response);
//            }
//        }
//        public class QType
//        {
//            public string QueryType { get; set; }
//        }
//    }
//}


