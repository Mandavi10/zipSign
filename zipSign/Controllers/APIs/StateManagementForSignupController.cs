using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
namespace zipSign.Controllers.APIs
{
    public class StateManagementForSignupController : ApiController
    {
        [HttpGet]
        [Route("StateManagementForSignup/GetStates")]
        public IHttpActionResult GetStates()
        {
            List<string> states = new List<string>
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

            var stateObjects = states.Select(state => new { name = state }).ToList();

            var response = new
            {
                status = "1",
                state = stateObjects
            };

            return Ok(response);
        }
    }
}

