using BusinessLayerModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BusinessDataLayer
{
    public class BusinessDataLayerClass
    {
        //public BusinessDataLayerClass() { GlobalMethods.Global.connectionString = GlobalMethods.Global.DocSign; }
        private SqlConnection con = new SqlConnection(GlobalMethods.Global.DocSign.ToString());
        private CommonStatus statusClass = new CommonStatus();
        public CommonStatus GetFunction(string SpName, List<DataItems> lstparam)
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCommand cmd = new SqlCommand(SpName, con);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DataItems items in lstparam)
                {
                    cmd.Parameters.AddWithValue("@" + items.Name, items.Value);
                }
                cmd.Parameters.AddWithValue("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                statusClass.DataFetch = ds;
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
                con.Close();
            }
        }
        public CommonStatus PostFunction(string SpName, List<DataItems> lstparam)
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCommand cmd = new SqlCommand(SpName, con);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DataItems items in lstparam)
                {
                    cmd.Parameters.AddWithValue("@" + items.Name, items.Value);
                }
                cmd.Parameters.AddWithValue("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int i = Convert.ToInt32(cmd.Parameters["@result"].Value.ToString());
                statusClass.StatusCode = i;
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
                con.Close();
            }
        }
        public CommonStatus GetFunctionWithResult(string SpName, List<DataItems> lstparam)
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCommand cmd = new SqlCommand(SpName, con);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (DataItems items in lstparam)
                {
                    cmd.Parameters.AddWithValue("@" + items.Name, items.Value);
                }
                cmd.Parameters.AddWithValue("@result", SqlDbType.Int).Direction = ParameterDirection.Output;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                int i = Convert.ToInt32(cmd.Parameters["@result"].Value.ToString());
                statusClass.StatusCode = i;
                statusClass.DataFetch = ds;
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
                con.Close();
            }
        }






    }
}
