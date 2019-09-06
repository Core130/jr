using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.DAL
{
    public class U8DBHelp
    {
        /// <summary>
        /// 获取U8数据库名称
        /// </summary>
        /// <param name="accID">帐套号ID</param>
        /// <param name="beginYear">开始年度</param>
        /// <returns>数据库名</returns>
        public static string GetAccountDataBase(string accID, SqlHelper sqlhelp) // , int beginYear
        {
            string SQL = "SELECT MAX(cDatabase) cDatabase FROM UA_AccountDatabase WHERE cAcc_Id =@cAcc_Id "; // AND iBeginYear = @iBeginYear
            SqlParameter[] para = { 
                                      new SqlParameter("@cAcc_Id",accID),
                                      //new SqlParameter("@iBeginYear",beginYear)
                                  };
            
            sqlhelp.SQL_DATA_BASE = "UFSystem";
            DataTable dt = sqlhelp.ExecuteDataTable(SQL, para);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cDatabase"].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
