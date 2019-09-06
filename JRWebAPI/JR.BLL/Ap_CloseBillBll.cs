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
    /// 收付款单
    /// </summary>
    public class Ap_CloseBillBll : U8BllBase
    {
        public Ap_CloseBillBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        /// <summary>
        /// 新增收付款单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public string AddAp_CloseBill(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, int ModelType, EntityAp_CloseBillHead entity, out int success)
        {
            success = 0;
            Ap_CloseBillBll bll = new Ap_CloseBillBll(StrAccID, AccYear, UserCode, PlainPassword);

            #region 验证
            // 必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }
            foreach (EntityAp_CloseBillBody entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }

            // 部门 
            string deptName = bll.GetDepartmentName(entity.cDeptCode);
            if (string.IsNullOrWhiteSpace(deptName))
            {
                return string.Format("U8不存在部门编码:{0},或者部门编码非末级!", entity.cDeptCode);
            }
            if (ModelType == 8004) //收款单接口ID
            {
                if (bll.GetGlmendFlag(entity.dVouchDate.Year, entity.dVouchDate.Month, "bflag_AR") == true)
                {
                    return string.Format("U8单据{0}日期所在月份已经结账!", entity.cVouchID);
                }
                // 客户编号  
                string cusName = bll.GetCustomerName(entity.cDwCode);
                if (string.IsNullOrWhiteSpace(cusName)) return string.Format("U8不存在客户编号:{0}!", entity.cDwCode);
                entity.cFlag = "AR";  // 应收应付标志 AP为应付
                entity.VT_ID = entity.cVouchType == "48" ? 8052 : 8055;
                entity.csysbarcode = entity.cVouchType == "48" ? "||ar48|" + entity.cVouchID : "||ar49|" + entity.cVouchID;
            }
            else if (ModelType == 8018) //付款单接口ID
            {
                if (bll.GetGlmendFlag(entity.dVouchDate.Year, entity.dVouchDate.Month, "bflag_AP") == true)
                {
                    return string.Format("U8单据{0}日期所在月份已经结账!", entity.cVouchID);
                }
                string vendorname = bll.GetVendorName(entity.cDwCode);
                if (string.IsNullOrEmpty(vendorname)) return string.Format("U8不存在供应商编号:{0}!", entity.cDwCode);
                entity.cFlag = "AP";  // 应收应付标志 AP为应付
                entity.VT_ID = entity.cVouchType == "49" ? 8053 : 8051;
                entity.csysbarcode = entity.cVouchType == "49" ? "||ap49|" + entity.cVouchID : "||ap48|" + entity.cVouchID;
            }
            else
            {
                return "没有找到可对应的操作项";
            }
            entity.cexch_name = "人民币";
            // GetSettleStyleName 结算方式
            string cSSCodeName = bll.GetSettleStyleName(entity.cSSCode);
            if (string.IsNullOrWhiteSpace(cSSCodeName)) return string.Format("U8不存在结算方式编码:{0}!", entity.cSSCode);
            if (entity.iAmount < 0 || entity.iAmount != entity.Details.Sum(p => p.iAmt))
            {
                return "金额信息异常";
            }
            #endregion
            //设置默认值
            entity.cexch_name = entity.cexch_name == null ? "人民币" : entity.cexch_name;//币种名称
            entity.iPayType = 0;
            entity.bFromBank = 0;
            entity.bToBank = 0;
            entity.bSure = 0;
            entity.iPeriod = entity.dVouchDate.Month;//会计期间
            entity.iAmount_f = entity.iAmount;
            entity.iRAmount = entity.iAmount;
            entity.iRAmount_f = entity.iAmount;
            entity.cOperator = entity.cOperator == null ? bll.GetUserName(UserCode) : entity.cOperator;
            if (entity.cNatBankCode != null)
            {
                var bank = bll.GetBankInfo(entity.cNatBankCode);//获取银行信息
                if (bank != null)
                {
                    entity.cNatBank = bank.cBName;//本单位银行名称
                    entity.cNatBankAccount = bank.cBAccount;//本单位银行账号                    
                }
            }

            foreach (EntityAp_CloseBillBody ecb in entity.Details)
            {
                ecb.cCusVen = entity.cDwCode;
                ecb.cDepCode = entity.cDeptCode;
                ecb.ifaresettled_f = 0;              
                ecb.iType = ecb.iType == null ? 1 : ecb.iType;        
                ecb.bPrePay = ecb.iType == 1 ? 1 : 0;
                ecb.iAmt_f = ecb.iAmt;
                ecb.iRAmt_f = ecb.iRAmt;
            }

            string id = bll.InsertAp_CloseBill(entity,ModelType);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }


        /// <summary>
        /// 新增收付款单
        /// </summary>
        /// <param name="Ap_CloseBillHead"></param>
        /// <returns></returns>
        public string InsertAp_CloseBill(EntityAp_CloseBillHead Ap_CloseBillHead, int ModelType)
        {
            int? Ap_CloseBillHeadID;
            if (Ap_CloseBillHead == null || Ap_CloseBillHead.Details == null || Ap_CloseBillHead.Details.Count == 0)
            {
                throw new JRException("收付款单新增失败!没有数据!");
            }

            #region 验证

            //判断数据库中是否含有该收付款单号，有的话不能添加
            int num = 0;
            string cmdText = "select count(*) as Num from Ap_CloseBill where cVouchID=@cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",Ap_CloseBillHead.cVouchID)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }

            if (num > 0)
            {
                throw new JRException("数据传递有误，已存在收付款单号!");
            }

            CreateDHColumn("Ap_CloseBills");

            #endregion
            //获取结算方式科目
            string ccode = string.IsNullOrEmpty(Ap_CloseBillHead.cNatBankAccount) ? null : GetcSSCode(Ap_CloseBillHead.cSSCode, Ap_CloseBillHead.cNatBankAccount, Ap_CloseBillHead.dVouchDate.Year.ToString());
            // 得到主表ID和从表ID
            int detailCount = Ap_CloseBillHead.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "SK", detailCount, out head_id, out body_end_Id);

            string sql = string.Format(@"
insert into Ap_CloseBill(iPayType,cVouchType,cVouchID,dVouchDate,iPeriod,cDwCode,
cexch_name,cOperator,cFlag,iID,bFromBank,bToBank,bSure,VT_ID,cSSCode,
cDeptCode,cDigest,iAmount,iAmount_f,iRAmount,iRAmount_f,
cNatBank,cNatBankAccount,cDefine2,cDefine9,cDefine10,
iAmount_s,dcreatesystime,cCode,csysbarcode
)VALUES
(@iPayType,@cVouchType,@cVouchID,@dVouchDate,@iPeriod,@cDwCode,
@cexch_name,@cOperator,@cFlag,@iID,@bFromBank,@bToBank,@bSure,@VT_ID,@cSSCode,
@cDeptCode,@cDigest,@iAmount,@iAmount_f,@iRAmount,@iRAmount_f,
@cNatBank,@cNatBankAccount,@cDefine2,@cDefine9,@cDefine10,
0,@dVouchDate,@cCode,@csysbarcode
)");
            SqlParameter[] para = {                               
                                      new SqlParameter("@iPayType",Ap_CloseBillHead.iPayType),
                                      new SqlParameter("@cVouchType",Ap_CloseBillHead.cVouchType),
                                      new SqlParameter("@cVouchID",Ap_CloseBillHead.cVouchID),
                                      new SqlParameter("@dVouchDate",Ap_CloseBillHead.dVouchDate.ToShortDateString()),
                                      new SqlParameter("@iPeriod",Ap_CloseBillHead.iPeriod),
                                      new SqlParameter("@cDwCode",Ap_CloseBillHead.cDwCode),
                                      new SqlParameter("@cexch_name",Ap_CloseBillHead.cexch_name),
                                      new SqlParameter("@cOperator",GetDBValue(Ap_CloseBillHead.cOperator)),
                                      new SqlParameter("@cFlag",GetDBValue(Ap_CloseBillHead.cFlag)),
                                      new SqlParameter("@bFromBank",Ap_CloseBillHead.bFromBank),
                                      new SqlParameter("@bToBank",Ap_CloseBillHead.bToBank),
                                      new SqlParameter("@bSure",Ap_CloseBillHead.bSure),
                                      new SqlParameter("@VT_ID",Ap_CloseBillHead.VT_ID),                                   
                                      new SqlParameter("@cSSCode",GetDBValue(Ap_CloseBillHead.cSSCode)),  
                                      new SqlParameter("@cDeptCode",GetDBValue(Ap_CloseBillHead.cDeptCode)), 
                                      new SqlParameter("@cDigest",GetDBValue(Ap_CloseBillHead.cDigest)), 
                                      new SqlParameter("@iAmount",Ap_CloseBillHead.iAmount), 
                                      new SqlParameter("@iAmount_f",Ap_CloseBillHead.iAmount_f), 
                                      new SqlParameter("@iRAmount",Ap_CloseBillHead.iRAmount), 
                                      new SqlParameter("@iRAmount_f",Ap_CloseBillHead.iRAmount_f), 
                                      new SqlParameter("@cNatBank",GetDBValue(Ap_CloseBillHead.cNatBank)), 
                                      new SqlParameter("@cNatBankAccount",GetDBValue(Ap_CloseBillHead.cNatBankAccount)), 
                                      new SqlParameter("@cDefine2",GetDBValue(Ap_CloseBillHead.cDefine2)),
                                      new SqlParameter("@cDefine9",GetDBValue(Ap_CloseBillHead.cDefine9)), 
                                      new SqlParameter("@cDefine10",GetDBValue(Ap_CloseBillHead.cDefine10)), 
                                      new SqlParameter("@iID",head_id), 
                                      new SqlParameter("@cCode",GetDBValue(ccode)),    
                                      new SqlParameter("@csysbarcode",Ap_CloseBillHead.csysbarcode),                      
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();

                    Ap_CloseBillHeadID = GetAp_CloseBillHeadID(Ap_CloseBillHead.cVouchID);
                    if (Ap_CloseBillHeadID == null)
                    {
                        throw new JRException("收付款单表头新增失败!");
                    }

                    //组装SQL语句                    
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntityAp_CloseBillBody body = Ap_CloseBillHead.Details[i];

                        string bodySql = string.Format(@"
insert into Ap_CloseBills(ifaresettled_f,iID,ID,iType,bPrePay,cCusVen,iAmt_f,iAmt,iRAmt_f,iRAmt,
cDepCode,cDefine22,cDefine25,iAmt_s,iRAmt_s,iSrcClosesID,cDefine24,DHID,cKm
)
VALUES(@ifaresettled_f,@iID,@ID,@iType,@bPrePay,@cCusVen,@iAmt_f,@iAmt,@iRAmt_f,@iRAmt,
@cDepCode,@cDefine22,@cDefine25,0,0,null,@cDefine24,@DHID,@cKm
)
");
                        SqlParameter[] bodyPara = { 
                                      new SqlParameter("@ifaresettled_f",body.ifaresettled_f),
                                      new SqlParameter("@iID",Ap_CloseBillHeadID),                                   
                                      new SqlParameter("@iType",body.iType),                                     
                                      new SqlParameter("@bPrePay",body.bPrePay),
                                      new SqlParameter("@cCusVen",body.cCusVen),
                                      new SqlParameter("@iAmt_f",body.iAmt_f),
                                      new SqlParameter("@iAmt",body.iAmt),
                                      new SqlParameter("@iRAmt_f",body.iRAmt_f),
                                      new SqlParameter("@iRAmt",body.iRAmt),
                                      new SqlParameter("@DHID",body.DHID),
                                      new SqlParameter("@cDepCode",GetDBValue(body.cDepCode)),
                                      new SqlParameter("@cDefine22",GetDBValue(body.cDefine22)),
                                      new SqlParameter("@cDefine25",GetDBValue(body.cDefine25)),
                                      new SqlParameter("@cDefine24",GetDBValue(body.cDefine24)),
                                      new SqlParameter("@ID",(body_end_Id - detailCount + i + 1)),
                                      new SqlParameter("@cKm",GetDBValue(GetArCode(body.iType,Ap_CloseBillHead.dVouchDate.Year.ToString(),ModelType))),

                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });

                    }

                    // 执行SQL
                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚
                    DeleteAp_CloseBill(Ap_CloseBillHead.cVouchID);

                    throw ex;
                }
            }
            else
            {
                throw new JRException("收付款单表头新增失败!");
            }

            return (Ap_CloseBillHeadID == null ? null : Ap_CloseBillHeadID.ToString());
        }



        public int? GetAp_CloseBillHeadID(string cVouchID)
        {
            string sql = "SELECT iID FROM Ap_CloseBill WHERE cVouchID = @cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",cVouchID)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["iID"].ToString());
            }

            return null;
        }
        /// <summary>
        /// 删除收付款单
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int DeleteAp_CloseBill(string cVouchID)
        {
            if (string.IsNullOrWhiteSpace(cVouchID))
                return 0;

            List<ExecuteHelp> list = new List<ExecuteHelp>();

            string sql = "DELETE Ap_CloseBill WHERE cVouchID = @cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",cVouchID)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });
            int? Ap_CloseBillID = GetAp_CloseBillHeadID(cVouchID);
            string delBodySql = "DELETE Ap_CloseBills where iID = @iID";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@iID",Ap_CloseBillID)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }
        public int QueryAp_CloseBill(string cvouchid)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from Ap_CloseBill where cVouchID=@cVouchID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVouchID",cvouchid)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }
            return num;
        }
        /// <summary>
        /// 获取银行信息
        /// </summary>
        /// <param name="cbcode"></param>
        /// <returns></returns>
        public EntityBank GetBankInfo(string cbcode)
        {
            EntityBank model = new EntityBank();
            string sql = "SELECT cBName,cBAccount FROM Bank WHERE cBCode = @cBCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cBCode",cbcode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                model.cBName = dt.Rows[0]["cBName"].ToString();
                model.cBAccount = dt.Rows[0]["cBAccount"].ToString();
                return model;
            }
            return null;
        }

        /// <summary>
        /// 根据款项类型获取基础科目设置中的科目信息
        /// </summary>
        /// <param name="iType">0：应收款|应付款、1：预收款|预付款、2：其他费用</param>
        /// <returns></returns>
        public string GetArCode(int? iType, string iYear, int iModelsType)
        {
            string cNote_f = "";
            string cFlag = "";
            if (iType == 0 ) cNote_f = "kzkm";
            if (iType == 1 ) cNote_f = "prekm";
            if (iModelsType == 8004) cFlag = "R";
            if (iModelsType == 8018) cFlag = "P";


            string sql = @"SELECT CASE WHEN cFlag = 'R' THEN cArCode WHEN cFlag = 'P' THEN cApCode ELSE NULL END AS Value  
                            FROM Ap_InputCode WHERE iyear = @iYear AND cNote_f = @cNote_f AND cFlag = @cFlag";
            SqlParameter[] paras = {
                                       new SqlParameter("@iYear",iYear),
                                       new SqlParameter("@cNote_f",cNote_f),
                                       new SqlParameter("@cFlag",cFlag),                                       
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                string sCode = dt.Rows[0]["Value"].ToString();
                return sCode;
            }
            return null;
        }
    }
}
