using System;
using System.Data.SqlClient;
using System.Web.Http;

namespace zipSign.Controllers.APIs
{
    public class VersionControlAPIController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                VersionControlItem versionControlItem = null;
                using (SqlConnection connection = new SqlConnection(GlobalMethods.Global.DocSign.ToString()))
                {
                    connection.Open();
                    string sqlQuery = "SELECT * FROM Tbl_VersionControl";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                versionControlItem = new VersionControlItem
                                {
                                    CurrentVersion = reader["Version"].ToString()
                                };
                            }
                        }
                    }
                }

                var response = new
                {
                    status = true,
                    message = versionControlItem
                };
                return Json(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    status = false,
                    message = ex.Message
                };
                return Json(errorResponse);
            }
        }

        public class VersionControlItem
        {
            public string CurrentVersion { get; set; }
        }
    }
}
