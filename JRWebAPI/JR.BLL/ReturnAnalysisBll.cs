using JR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.BLL
{
    /// <summary>
    /// 退货分析报表处理类
    /// </summary>
    public class ReturnAnalysisBll : U8BllBase
    {
        public ReturnAnalysisBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        /// <summary>
        /// 查询退货分析报表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<EntityReturnAnalysisReport> GetReturnAnalysis(EntityReturnAnalysisRQC entity)
        {
            List<EntityReturnAnalysisReport> list = new List<EntityReturnAnalysisReport>();
            //string sql = string.Format(@"exec prc_report_ReturnAnalysis '{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}','{0}'",
            //    entity.DateBeg, entity.DateEed, entity.Depart1, entity.Depart2, entity.cCusCode1, entity.cCusCode2, entity.cCusDefine11, entity.cCusDefine12, entity.ICategory1, entity.ICategory2,
            //    entity.InvCode1, entity.InvCode2, entity.SumQuantity1, entity.SumQuantity2, entity.SumMoney1, entity.SumMoney2, entity.ReProportion1, entity.ReProportion2);
            //return this.DB_SqlHelper.ExecuteDataTable(CommandType.Text, sql);
            string SQL_StoredProcedure = "prc_report_ReturnAnalysis";
            SqlParameter[] sp_para = new SqlParameter[4];
            sp_para[0] = new SqlParameter("@datebeg", entity.DateBeg);
            sp_para[1] = new SqlParameter("@dateend", entity.DateEed);
            //sp_para[2] = new SqlParameter("@Depart1", entity.Depart1);
            //sp_para[3] = new SqlParameter("@Depart2", entity.Depart2);
            sp_para[2] = new SqlParameter("@cCusCode", entity.cCusCode);

            //sp_para[5] = new SqlParameter("@cCusCode2", entity.cCusCode2);
            //sp_para[6] = new SqlParameter("@cCusDefine11", entity.cCusDefine11);
            //sp_para[7] = new SqlParameter("@cCusDefine12", entity.cCusDefine12);
            //sp_para[8] = new SqlParameter("@ICategory1", entity.ICategory1);
            //sp_para[9] = new SqlParameter("@ICategory2", entity.ICategory2);

            sp_para[3] = new SqlParameter("@InvCode", entity.InvCode);
            //sp_para[11] = new SqlParameter("@InvCode2", entity.InvCode2);
            //sp_para[12] = new SqlParameter("@SumQuantity1", entity.SumQuantity1);
            // sp_para[13] = new SqlParameter("@SumQuantity2", entity.SumQuantity2);
            //sp_para[14] = new SqlParameter("@SumMoney1", entity.SumMoney1);

            //sp_para[15] = new SqlParameter("@SumMoney2", entity.SumMoney2);
            //sp_para[16] = new SqlParameter("@ReProportion1", entity.ReProportion1);
            //sp_para[17] = new SqlParameter("@ReProportion2", entity.ReProportion2);
            DataTable dt = new DataTable();
            dt = DB_SqlHelper.ExecuteDataTable(CommandType.StoredProcedure, SQL_StoredProcedure, sp_para);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EntityReturnAnalysisReport data = new EntityReturnAnalysisReport();
                DataRow dr = dt.Rows[i];
                data.cDepCode = dr["cDepCode"].ToString();
                data.cDepName = dr["cDepName"].ToString();
                data.cCusCode = dr["cCusCode"].ToString();
                data.cCusName = dr["cCusName"].ToString();
                data.cCusAbbName = dr["cCusAbbName"].ToString();
                data.cCusDefine1 = dr["cCusDefine1"].ToString();
                data.cInvCode = dr["cInvCode"].ToString();
                data.cInvName = dr["cInvName"].ToString();
                data.cInvStd = dr["cInvStd"].ToString();
                data.cInvCCode = dr["cInvCCode"].ToString();
                data.cInvCName = dr["cInvCName"].ToString();
                data.cComUnitCode = dr["cComUnitCode"].ToString();
                data.cComUnitName = dr["cComUnitName"].ToString();
                data.thIQuantity = Convert.ToDecimal(dr["thIQuantity"].ToString());
                data.thINatSum = Convert.ToDecimal(dr["thINatSum"].ToString());
                data.fhIQuantity = Convert.ToDecimal(dr["fhIQuantity"].ToString());
                data.fhINatSum = Convert.ToDecimal(dr["fhINatSum"].ToString());
                data.actualQty = Convert.ToDecimal(dr["actualQty"].ToString());
                data.actualSum = Convert.ToDecimal(dr["actualSum"].ToString());
                //data.percentSum = dr["percentSum"].ToString();

                list.Add(data);
            }
            return list;
        }
    }
}
