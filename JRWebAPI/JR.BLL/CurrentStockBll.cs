using JR.HL;
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
    /// 库存量查询处理类
    /// </summary>
    public class CurrentStockBll : U8BllBase
    {
        public CurrentStockBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
      
        /// <summary>
        /// 库存量查询
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <param name="cWhCode"></param>
        /// <returns></returns>
        public List<EntityCurrentStock> GetCurrentStock(EntityCurrentStock entity,out string msg)
        {
            msg = "";
            List<EntityCurrentStock> list = new List<EntityCurrentStock>();
            if (string.IsNullOrEmpty(entity.cInvCode) && string.IsNullOrEmpty(entity.cWhCode))
            {
                msg = "参数传递有误!存货编号和仓库编号不能同时为空";
                return list;
            }

            string sql = @"select I.cInvCode,cWhCode,cBatch,(CASE WHEN bInvBatch=1 THEN  CASE WHEN bStopFlag =1 OR bGSPStop= 1 THEN 0 ELSE 
                            ISNULL(iQuantity,0)- IsNull(fStopQuantity,0) END  + ISNULL(fInQuantity,0) - ISNULL(fOutQuantity,0) ELSE  
                            CASE WHEN bStopFlag =1 OR bGSPStop= 1 THEN 0 ELSE ISNULL(iQuantity,0)- IsNull(fStopQuantity,0) END  + 
                            ISNULL(fInQuantity,0) - ISNULL(fOutQuantity,0) END) AS iQuantity 
                            from v_ST_currentstockForReport  CS inner join dbo.Inventory I ON I.cInvCode = CS.cInvCode  where 1= 1";
            if (!string.IsNullOrWhiteSpace(entity.cInvCode))
            {
                //sql += string.Format(" and cInvCode = '{0}'", entity.cInvCode);
                sql += string.Format(" and I.cInvCode in ('{0}')", entity.cInvCode.Replace(",","','"));
            }
            if (!string.IsNullOrWhiteSpace(entity.cWhCode))
            {
                //sql += string.Format(" and cWhCode = '{0}'" ,entity.cWhCode);
                sql += string.Format(" and cWhCode in ('{0}')" ,entity.cWhCode.Replace(",","','"));
            }

            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())  
            {
                EntityCurrentStock ecs = new EntityCurrentStock();
                ecs.cInvCode = datareader["cInvCode"].ToString();
                ecs.cWhCode = datareader["cWhCode"].ToString();
                ecs.cBatch = datareader["cBatch"].ToString();
                ecs.iQuantity = Convert.ToInt32(datareader["iQuantity"]);
                list.Add(ecs);
            }
            return list;
        }
    }
}
