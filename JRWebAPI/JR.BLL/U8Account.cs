using JR.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.BLL
{
    /// <summary>
    /// 提供U8账套相关服务
    /// </summary>
    public class U8Account
    {
        /// <summary>
        /// 通过账套号获取起始年度
        /// </summary>
        /// <param name="accID"></param>
        /// <returns></returns>
        public static int GetBeginYear(string accID)
        {
            string SQL = "select MAX(iYear) iYear from UA_Account where cAcc_Id = @cAcc_Id";
            SqlParameter[] para = { 
                                      new SqlParameter("@cAcc_Id",accID)
                                  };

            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.SQL_DATA_BASE = "UFSystem";
            DataTable dt = sqlhelp.ExecuteDataTable(SQL, para);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["iYear"].ToString());
            }
            else
            {
                throw new Exception("不能获取账套号起始年度!");
            }
        }
    }
}
