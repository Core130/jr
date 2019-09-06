using JR.DAL;
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
    /// 其他应付单
    /// </summary>
    public class Ap_VouchBll : U8BllBase
    {
        public Ap_VouchBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        /// <summary>
        /// 新增其他应付单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public string AddAp_Vouch(string UserCode, string PlainPassword,
                                   string StrAccID, int AccYear, string Act, EntityAp_VouchHead entity,out int success)
        {
            success = 0;
            Ap_VouchBll bll = new Ap_VouchBll(StrAccID, AccYear, UserCode, PlainPassword);
            
            #region 验证
            // 必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }
            foreach (EntityAp_VouchBody entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }

            if (bll.GetGlmendFlag(entity.dVouchDate.Year, entity.dVouchDate.Month, "bflag_AP") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cVouchID);
            }

            if (!string.IsNullOrEmpty(entity.cDwCode))
            {
                string vendorname = bll.GetVendorName(entity.cDwCode);
                if (string.IsNullOrEmpty(vendorname))
                    return string.Format("U8中不存在供应商编号:{0}!", entity.cDwCode);
            }

            #endregion
            #region 表头
            entity.cLink = "P0" + entity.cVouchID;
            entity.cVouchType = "P0";
            entity.cexch_name = entity.cexch_name == null ? "人民币" : entity.cexch_name;//币种名称      
            entity.iRAmount = entity.iAmount_f;
            entity.iAmount = entity.iAmount_f;
            entity.iRAmount_f = entity.iAmount_f;
            entity.cFlag = "AP";  // 应收应付标志 AP为应付,AR为应收
            entity.VT_ID = 8056;  // 单据模板号
            entity.iClosesID = 0;
            entity.iCoClosesID = 0;
            entity.iExchRate = 1;
            entity.bd_c = 0;
            entity.cOperator = entity.cOperator == null ? bll.GetUserName(UserCode) : entity.cOperator;
            entity.cSysBarCode = string.Format("||app0|{0}", entity.cVouchID);

            #endregion
            //表体
            foreach (EntityAp_VouchBody item in entity.Details)
            {
                if (!string.IsNullOrWhiteSpace(item.cCode))
                {
                    string cCode = GetcCode(item.cCode, entity.dVouchDate.Year);
                    if (string.IsNullOrWhiteSpace(cCode))
                    {
                        return string.Format("U8中不存在科目编码:{0}，或者非末级科目!", item.cCode);
                    }
                }
                item.cDwCode = entity.cDwCode;
                item.iAmount = item.iAmount_f;
                item.iExchRate = entity.iExchRate;
                item.cLink = entity.cLink;
                item.cexch_name = entity.cexch_name;             
               
            }
            string id = bll.InsertAp_Vouch(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;

        }
        public string InsertAp_Vouch(EntityAp_VouchHead Ap_VouchHead)
        {
            string Ap_VouchHeadID=string.Empty;
            if (Ap_VouchHead == null)
            {
                throw new JRException("其他应付单新增失败!没有数据!");
            }
            #region 验证
            int num = GetAp_Vouch(Ap_VouchHead.cVouchID);
            if (num > 0)
            {
                throw new JRException(string.Format("单号为：{0}已经生成其他应付单！", Ap_VouchHead.cVouchID));
            }           
            #endregion

            int detailCount = Ap_VouchHead.Details.Count;
            
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "RP", detailCount, out head_id, out body_end_Id);
            //int autoid = GetAutoID() + 1;
            string sql = string.Format(@"insert into Ap_Vouch(cLink,cVouchType,cVouchID,dVouchDate,cDwCode,cexch_name,
                                         iRAmount,iAmount,iAmount_f,iRAmount_f,cFlag,VT_ID,iClosesID,
                                         iCoClosesID,iExchRate,bd_c,cOperator,dcreatesystime,Auto_ID,cSysBarCode,cDigest)
                                         values
                                         (@cLink,@cVouchType,@cVouchID,@dVouchDate,@cDwCode,@cexch_name,
                                         @iRAmount,@iAmount,@iAmount_f,@iRAmount_f,@cFlag,@VT_ID,@iClosesID,
                                         @iCoClosesID,@iExchRate,@bd_c,@cOperator,@dcreatesystime,@Auto_ID,@cSysBarCode,@cDigest)");
            SqlParameter[] para = {                               
                                      new SqlParameter("@cLink",Ap_VouchHead.cLink),
                                      new SqlParameter("@cVouchType",Ap_VouchHead.cVouchType),
                                      new SqlParameter("@cVouchID",Ap_VouchHead.cVouchID),
                                      new SqlParameter("@dVouchDate",Ap_VouchHead.dVouchDate.ToShortDateString()),                                   
                                      new SqlParameter("@cDwCode",Ap_VouchHead.cDwCode),
                                      new SqlParameter("@cexch_name",Ap_VouchHead.cexch_name),
                                      new SqlParameter("@iRAmount",Ap_VouchHead.iRAmount),
                                      new SqlParameter("@iAmount",Ap_VouchHead.iAmount),
                                      new SqlParameter("@iAmount_f",Ap_VouchHead.iAmount_f),
                                      new SqlParameter("@iRAmount_f",Ap_VouchHead.iRAmount_f),
                                      new SqlParameter("@cFlag",Ap_VouchHead.cFlag),
                                      new SqlParameter("@VT_ID",Ap_VouchHead.VT_ID),               
                                      new SqlParameter("@iClosesID",Ap_VouchHead.iClosesID), 
                                      new SqlParameter("@iCoClosesID",Ap_VouchHead.iCoClosesID),
                                      new SqlParameter("@iExchRate",Ap_VouchHead.iExchRate),
                                      new SqlParameter("@bd_c",Ap_VouchHead.bd_c),
                                      new SqlParameter("@cOperator",Ap_VouchHead.cOperator),
                                      new SqlParameter("@dcreatesystime",DateTime.Now), 
                                      new SqlParameter("@Auto_ID",head_id),   
                                      new SqlParameter("@cSysBarCode",Ap_VouchHead.cSysBarCode), 
                                      new SqlParameter("@cDigest",GetDBValue(Ap_VouchHead.cDigest)), 
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();
                    Ap_VouchHeadID = GetAp_VouchHeadID(Ap_VouchHead.cVouchID);
                    if (Ap_VouchHeadID == null)
                    {
                        throw new JRException("其他应付单表头新增失败!");
                    }
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntityAp_VouchBody body = Ap_VouchHead.Details[i];
                        string bodySql = string.Format(@"insert into Ap_Vouchs(cLink,cexch_name,cDwCode,iExchRate,bd_c,iAmount,iAmount_f,cDigest,cCode)
                                                 values
                                                 (@cLink,@cexch_name,@cDwCode,@iExchRate,@bd_c,@iAmount,@iAmount_f,@cDigest,@cCode)");
                        SqlParameter[] bodyPara = {                               
                                      new SqlParameter("@cLink",body.cLink), 
                                      new SqlParameter("@cexch_name",body.cexch_name),                                   
                                      new SqlParameter("@cDwCode",body.cDwCode),
                                      new SqlParameter("@iExchRate",body.iExchRate),
                                      new SqlParameter("@bd_c",1), 
                                      new SqlParameter("@iAmount",body.iAmount),
                                      new SqlParameter("@iAmount_f",body.iAmount_f),    
                                      new SqlParameter("@cDigest",GetDBValue(body.cDigest)), 
                                      new SqlParameter("@cCode",GetDBValue(body.cCode)),                                                                  
                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });
                    }

                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
                }
                catch (Exception ex)
                {
                    DeleteAp_Vouch(Ap_VouchHead.cVouchID);
                    throw ex;
                }


            }
            else
            {
                throw new JRException("其他应付单表头新增失败！");
            }
            return (Ap_VouchHeadID == null ? null : Ap_VouchHeadID.ToString());
        }
        /// <summary>
        /// 删除其他应付单
        /// </summary>
        /// <param name="cVouchID"></param>
        /// <returns></returns>
        public int DeleteAp_Vouch(string cVouchID)
        {
            if (string.IsNullOrWhiteSpace(cVouchID))
                return 0;

            List<ExecuteHelp> list = new List<ExecuteHelp>();
            string sql = "delete Ap_Vouch where cVouchID=@cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",cVouchID)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });
            string cLink = GetAp_VouchHeadID(cVouchID);
            string delBodySql = "DELETE Ap_Vouchs where cLink = @cLink";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@cLink",cLink)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });
            return this.DB_SqlHelper.ExecuteNonQuery(sql,paras);
        }
        ///// <summary>
        ///// 获取应付单表中的最大自动标识
        ///// </summary>
        ///// <returns></returns>
        //public int GetAutoID()
        //{
        //    string sql = "select Max(Auto_ID) as Auto_ID from Ap_Vouch";
        //    DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql);
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        return int.Parse(dt.Rows[0]["Auto_ID"].ToString());
        //    }
        //    return 0;
        //}
        /// <summary>
        /// 获取当前新增的联结号
        /// </summary>
        /// <param name="cVouchID"></param>
        /// <returns></returns>
        public string GetAp_VouchHeadID(string cVouchID)
        {
            string sql = "select cLink from Ap_Vouch where cVouchID=@cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",cVouchID)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql,paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cLink"].ToString();
            }
            return null;
        }
        /// <summary>
        /// 判断该单号是否存在
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public int GetAp_Vouch(string cVouchID)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from Ap_Vouch where cVouchID=@cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",cVouchID)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }

            return num;
        }
    }
}
