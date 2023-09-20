using BusinessDataLayer;
using BusinessLayerModel;
using System;
using System.Collections.Generic;

namespace BusinessAccessLayer
{
    public class BusinessAccesslayerClass
    {
        private BusinessDataLayerClass dal = new BusinessDataLayerClass();
        private CommonStatus statusClass = new CommonStatus();

        public CommonStatus sp(string SpName, List<DataItems> lstparam)
        {
            try
            {
                statusClass = dal.GetFunctionWithResult(SpName, lstparam);
                return statusClass;
            }
            catch (Exception ex)
            {
                statusClass.Message = ex.Message;
                statusClass.StatusCode = 0;
                return statusClass;
            }
            finally
            {

            }
        }
        public CommonStatus GetFunctionWithResult(string SpName, List<DataItems> lstparam)
        {
            try
            {
                statusClass = dal.GetFunctionWithResult(SpName, lstparam);
                return statusClass;
            }
            catch (Exception ex)
            {
                statusClass.Message = ex.Message;
                statusClass.StatusCode = 0;
                return statusClass;
            }
            finally
            {

            }
        }

        public CommonStatus GetFunction(string SpName, List<DataItems> lstparam)
        {
            try
            {
                statusClass = dal.GetFunction(SpName, lstparam);
                return statusClass;
            }
            catch (Exception ex)
            {
                statusClass.Message = ex.Message;
                statusClass.StatusCode = 0;
                return statusClass;
            }
            finally
            {

            }
        }

        public CommonStatus PostFunction(string SpName, List<DataItems> lstparam)
        {
            try
            {
                statusClass = dal.PostFunction(SpName, lstparam);
                return statusClass;
            }
            catch (Exception ex)
            {
                statusClass.Message = ex.Message;
                statusClass.StatusCode = 0;
                return statusClass;
            }
            finally
            {
            }
        }
    }
}
