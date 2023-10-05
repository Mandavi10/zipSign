using BusinessAccessLayer;
using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace zipSign.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BusinessAccesslayerClass bal = new BusinessAccesslayerClass();
        private CommonStatus statusClass = new CommonStatus();

        //Security objSecurity = new Security();
        private readonly ProcMaster pro = new ProcMaster();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        public ActionResult Profileview()
        {
            List<profile> result = new List<profile>();

            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QuerySelector", "ShowRecord")
            };
            statusClass = bal.GetFunctionWithResult(pro.DepartmentMaster, obj);
            foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)

            {
                statusClass = bal.GetFunctionWithResult(pro.DepartmentMaster, obj);

                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new profile
                    {
                        ProId = Convert.ToInt32(dr["DocumentUploadId"]),
                        FirstName = Convert.ToString(dr["DocumentName"]),
                        LastName = Convert.ToString(dr["DocumentName"]),
                        Add1 = Convert.ToString(dr["SignStatus"]),
                        Add2 = Convert.ToString(dr["UploadedOn"]),
                        ComWebURL = Convert.ToString(dr["UploadedBy"]),
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return RedirectToAction("Error", new { errorMessage = ex.Message });
            }


            profile userProfile = new profile
            {
                ProId = 1,
                FirstName = "John",
                LastName = "Doe",
                Add1 = "Address 1",
                Add2 = "Address 2",
                Pin = 123456,
                DOB = Convert.ToString(DateTime.Now),
                GST = "123456789",
                ComWebURL = "http://example.com",
                EDomian = "example.com"
            };

            return View(userProfile);
        }

    }
}