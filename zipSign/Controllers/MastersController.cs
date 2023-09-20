using BusinessDataLayer;
using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
namespace zipSign.Controllers
{
    public class MastersController : Controller
    {
        private BusinessDataLayerClass bal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();
        private ProcMaster pro = new ProcMaster();
        public ActionResult DesignationMaster()
        {
            return View();
        }
        public ActionResult AllDesignations()
        {
            return View();
        }
        public ActionResult AllDepartments()
        {
            return View();
        }
        public ActionResult DepartmentMaster()
        {
            return View();
        }
        public ActionResult DomainControl()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DeptInsert(DeptMaster objDept)
        {
            try
            {
                List<DataItems> obj = new List<DataItems>
                {
                    new DataItems("DepartmentId", objDept.DepartmentId),
                    new DataItems("DepartmentCode", objDept.DepartmentCode),
                    new DataItems("DepartmentName", objDept.DepartmentName),
                    new DataItems("Description", objDept.Description),
                    new DataItems("IsActive", objDept.IsActive),
                    new DataItems("QuerySelector", "InsertRecord")
                };
                statusClass = bal.PostFunction(pro.DepartmentMaster, obj);
                var result = new
                {
                    status = "200",
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var result1 = new
                {
                    status = "101",
                };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SearchData1(pagination objpage)
        {
            ResultData1 res1 = new ResultData1();
            List<DeptMaster> result = new List<DeptMaster>();
            List<pagination> result2 = new List<pagination>();
            List<DataItems> obj = new List<DataItems>();
            string querySelector = "ShowRecord"; // Default query selector for ShowRecord operation
            if (!string.IsNullOrEmpty(objpage.keyword))
            {
                querySelector = "Search"; // Set query selector for search operation
                obj.Add(new DataItems("DepartmentName", objpage.keyword));
            }
            obj.Add(new DataItems("QuerySelector", querySelector));
            obj.Add(new DataItems("PageCount", objpage.pagecount));
            statusClass = bal.GetFunctionWithResult(pro.DepartmentMaster, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new DeptMaster
                    {
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        DepartmentCode = Convert.ToString(dr["DepartmentCode"]),
                        DepartmentName = Convert.ToString(dr["DepartmentName"]),
                        Description = Convert.ToString(dr["Description"]),
                        CreatedBy = Convert.ToString(dr["CreatedBy"]),
                        CreatedOn = Convert.ToString(dr["CreatedOn"]),
                        UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                        IsActive = Convert.ToString(dr["IsActive"]),
                    });
                }
                res1.Table1 = result;
            }
            if (statusClass.DataFetch.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[1].Rows)
                {
                    result2.Add(new pagination
                    {
                        pagecount = Convert.ToInt32(dr["pagecount"]),
                        count = Convert.ToInt32(dr["count"]),
                        page = Convert.ToInt32(dr["page"]),
                        size = Convert.ToInt32(dr["Size"]),
                    });
                }
                res1.Table2 = result2;
            }

            return Json(res1, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult SearchDataForSigned(pagination objpage)
        {
            ResultData res1 = new ResultData();
            List<SignMaster> result = new List<SignMaster>();
            List<pagination> result2 = new List<pagination>();
            List<DataItems> obj = new List<DataItems>();
            string querySelector = "ShowSignedDoc"; // Default query selector for ShowRecord operation
            if (!string.IsNullOrEmpty(objpage.keyword))
            {
                querySelector = "SearchforSigned"; // Set query selector for search operation
                obj.Add(new DataItems("UploadedFileName", objpage.keyword));
            }
            obj.Add(new DataItems("QuerySelector", querySelector));
            obj.Add(new DataItems("PageCount", objpage.pagecount));
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new SignMaster
                    {
                        DocumentUploadId = Convert.ToInt32(dr["DocumentUploadId"]),
                        UploadedFileName = Convert.ToString(dr["UploadedFileName"]),
                        DocumentName = Convert.ToString(dr["DocumentName"]),
                        SignStatus = Convert.ToString(dr["SignStatus"]),
                        UploadedOn = Convert.ToString(dr["UploadedOn"]),
                        UploadedBy = Convert.ToString(dr["UploadedBy"]),
                        // CreatedOn = Convert.ToString(dr["CreatedOn"]),
                        // UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                        // IsActive = Convert.ToString(dr["IsActive"]),
                    });
                }
                res1.Table1 = result;
            }

            if (statusClass.DataFetch.Tables.Count > 1 && statusClass.DataFetch.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[1].Rows)
                {
                    result2.Add(new pagination
                    {
                        pagecount = Convert.ToInt32(dr["pagecount"]),
                        count = Convert.ToInt32(dr["count"]),
                        page = Convert.ToInt32(dr["page"]),
                        size = Convert.ToInt32(dr["Size"]),
                    });
                }
                res1.Table2 = result2;
            }

            return Json(res1, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult GetDepartmentDetails(string departmentCode)
        {
            ResultData1 res1 = new ResultData1();
            List<DeptMaster> result = new List<DeptMaster>();
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QuerySelector", "ShowSignedDocLbl"),
                new DataItems("DepartmentCode", departmentCode)
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
            {
                result.Add(new DeptMaster
                {
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentCode = Convert.ToString(dr["DepartmentCode"]),
                    DepartmentName = Convert.ToString(dr["DepartmentName"]),
                    Description = Convert.ToString(dr["Description"]),
                    CreatedBy = Convert.ToString(dr["CreatedBy"]),
                    CreatedOn = Convert.ToString(dr["CreatedOn"]),
                    UpdatedBy = Convert.ToString(dr["UpdatedBy"]),
                    UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                    IsActive = Convert.ToString(dr["IsActive"]),
                });
            }
            res1.Table1 = result;
            return Json(res1, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetSignedFileDetails(string Filecode)
        {
            ResultData res1 = new ResultData();
            List<SignMaster> result = new List<SignMaster>();
            List<pagination> result2 = new List<pagination>();
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QuerySelector", "ShowSignedDocLbl"),
                new DataItems("DocumentUploadId", Filecode)
            };
            statusClass = bal.GetFunctionWithResult(pro.Sp_SignUpload, obj);
            foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
            {
                result.Add(new SignMaster
                {
                    DocumentUploadId = Convert.ToInt32(dr["DocumentUploadId"]),
                    UploadedFileName = Convert.ToString(dr["UploadedFileName"]),
                    DocumentName = Convert.ToString(dr["DocumentName"]),
                    SignStatus = Convert.ToString(dr["SignStatus"]),
                    UploadedOn = Convert.ToString(dr["UploadedOn"]),
                    UploadedBy = Convert.ToString(dr["UploadedBy"]),
                    // CreatedOn = Convert.ToString(dr["CreatedOn"]),
                    //UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                    //IsActive = Convert.ToString(dr["IsActive"]),
                });
            }
            res1.Table1 = result;
            return Json(res1, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public FileContentResult DownloadExcelFile()
        {
            List<DeptMaster> result = new List<DeptMaster>();
            // Retrieve your data and populate the 'result' list
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("QuerySelector", "ExcelDownload")
            };
            statusClass = bal.GetFunctionWithResult(pro.DepartmentMaster, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
                {
                    result.Add(new DeptMaster
                    {
                        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                        DepartmentCode = Convert.ToString(dr["DepartmentCode"]),
                        DepartmentName = Convert.ToString(dr["DepartmentName"]),
                        CreatedBy = Convert.ToString(dr["CreatedBy"]),
                        CreatedOn = Convert.ToString(dr["CreatedOn"]),
                        UpdatedBy = Convert.ToString(dr["UpdatedBy"]),
                        UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
                        IsActive = Convert.ToString(dr["IsActive"]),
                    });
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Department Id,Department Code,Department Name,Created By,Created On,Updated By,Updated On,IsActive");
            sb.AppendLine($"");
            foreach (DeptMaster item in result)
            {
                sb.AppendLine($"{item.DepartmentId},{item.DepartmentCode},{item.DepartmentName},{item.CreatedBy},{item.CreatedOn},{item.UpdatedBy},{item.UpdatedOn},{item.IsActive}");

            }
            byte[] fileContents = Encoding.UTF8.GetBytes(sb.ToString());//Converts The File Contents Into Byte Array
            return File(fileContents, "application/vnd.ms-excel", "DepartmentDetails.csv");
        }
        [HttpPost]
        public JsonResult GetUserData(DeptMaster objDesig)
        {

            DeptMaster result = new DeptMaster();
            List<DataItems> obj = new List<DataItems>
            {
                new DataItems("DepartmentId", objDesig.DepartmentId),
                new DataItems("QuerySelector", "ShowRecordId")
            };
            statusClass = bal.GetFunctionWithResult(pro.DepartmentMaster, obj);
            if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
            {
                result.DepartmentId = Convert.ToInt32(statusClass.DataFetch.Tables[0].Rows[0]["DepartmentId"]);

                result.DepartmentCode = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["DepartmentCode"]);

                result.DepartmentName = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["DepartmentName"]);

                result.Description = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["Description"]);

                result.IsActive = Convert.ToString(statusClass.DataFetch.Tables[0].Rows[0]["IsActive"]);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var result1 = new
            {
                status = "201",
            };
            return Json(result1, JsonRequestBehavior.AllowGet);
        }
        // [HttpPost]
        //public ActionResult Search(string keyword)
        //{
        //    ResultData res1 = new ResultData();
        //    List<DeptMaster> result = new List<DeptMaster>();
        //    List<DataItems> obj = new List<DataItems>();
        //    obj.Add(new DataItems("QuerySelector", "ShowRecord"));
        //    obj.Add(new DataItems("DepartmentName", keyword));

        //    statusClass = bal.GetFunctionWithResult(pro.DepartmentMaster, obj);

        //    if (statusClass.DataFetch.Tables[0].Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in statusClass.DataFetch.Tables[0].Rows)
        //        {
        //            result.Add(new DeptMaster
        //            {
        //                DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
        //                DepartmentCode = Convert.ToString(dr["DepartmentCode"]),
        //                DepartmentName = Convert.ToString(dr["DepartmentName"]),
        //                CreatedBy = Convert.ToString(dr["CreatedBy"]),
        //                CreatedOn = Convert.ToString(dr["CreatedOn"]),
        //                UpdatedBy = Convert.ToString(dr["UpdatedBy"]),
        //                UpdatedOn = Convert.ToString(dr["UpdatedOn"]),
        //                IsActive = Convert.ToString(dr["IsActive"]),
        //            });
        //        }
        //    }

        //    return Json(res1, JsonRequestBehavior.AllowGet);
        //}

    }

}