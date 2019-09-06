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
   /// 采购订单
   /// </summary>
    public class PO_PomainBll:U8BllBase
    {
       public PO_PomainBll(string accID, int beginYear, string userID, string password)
           : base(accID, beginYear, userID, password)
       {

       }

       /// <summary>
       /// 新增采购订单
       /// </summary>
       /// <param name="UserCode"></param>
       /// <param name="PlainPassword"></param>
       /// <param name="StrAccID"></param>
       /// <param name="AccYear"></param>
       /// <param name="Act"></param>
       /// <param name="Entity"></param>
       /// <returns></returns>
       public string AddPO_Pomain(string UserCode, string PlainPassword,
                                  string StrAccID, int AccYear, string Act, EntityPO_Pomain entity,out int success)
       {
           success = 0;
           PO_PomainBll bll = new PO_PomainBll(StrAccID, AccYear, UserCode, PlainPassword);
           #region 验证
           string msg = "";
           if (!entity.CheckEntity(out msg))
           {
               return msg;
           }
           foreach (EntityPO_PoDetails entitybody in entity.Details)
           {
               if (!entitybody.CheckEntity(out msg))
               {
                   return msg;
               }
           }
           if (bll.GetGlmendFlag(entity.dPODate.Year, entity.dPODate.Month, "bflag_PU") == true)
           {
               return string.Format("U8单据{0}日期所在月份已经结账!", entity.cPOID);
           }
           if (!string.IsNullOrEmpty(entity.cVenCode))
           {
               string vendorname = bll.GetVendorName(entity.cVenCode);
               if (string.IsNullOrEmpty(vendorname))
                   return string.Format("U8中不存在供应商编码:{0}!", entity.cVenCode);
           }
           #endregion
           #region 表头
           //entity.POID = 0;//
           //entity.cPOID = "";
           //entity.dPODate = DateTime.Now;
           //entity.cVenCode = "";
           entity.cexch_name = entity.cexch_name == null ? "人民币" : entity.cexch_name;//币种名称
           entity.nflat = 1;//汇率
           //entity.iTaxRate = 17;//表头税率
           entity.cMaker = bll.GetUserName(UserCode);
           entity.iVTid = 8173;//单据模板号
           entity.cBusType = "普通采购";//业务类型
           entity.iDiscountTaxType = 0;//扣税类别
           entity.cState = 1; //状态默认已审核
           entity.iVerifyStateex = 2;//单据审核状态 （-1，终审不同意，0，未提交，1已提交，2，终审同意）
           entity.cSysbarCode = "||pupo|" + entity.cPOID;
           #endregion
           foreach (EntityPO_PoDetails item in entity.Details)
           {
               item.ID = 0;//采购订单子表标识
               item.iUnitPrice = item.iTaxPrice / (1 + item.iPerTaxRate / 100);
               item.iMoney = item.iSum / (1 + item.iPerTaxRate / 100);//原币无税金额
               item.iTax = item.iSum - item.iMoney;//原币税额
               item.iNatUnitPrice = item.iUnitPrice;//本币无税单价
               item.iNatMoney = item.iMoney;//本币无税金额
               item.iNatTax = item.iTax;//本币税额
               item.iNatSum = item.iSum;//本币价税合计
               item.iTaxPrice = item.iTaxPrice;//原币含税单价
               entity.iTaxRate = item.iPerTaxRate;//表头税率 
           }
           string id = bll.InsertPO_Pomain(entity);
           success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
           return id;
       }
       public string InsertPO_Pomain(EntityPO_Pomain PO_PomainHead)
       {
           int? PO_PomainHeadID;
           if (PO_PomainHead == null || PO_PomainHead.Details == null || PO_PomainHead.Details.Count == 0)
           {
               throw new JRException("采购订单新增失败!没有数据!");
           }
           #region 验证
           int num = 0;
           string cmdText = "select count(*) as Num from PO_Pomain where cPOID=@cPOID";
           SqlParameter[] paras = {
                                       new SqlParameter("@cPOID",PO_PomainHead.cPOID)
                                   };
           DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
           if (dt != null && dt.Rows.Count > 0)
           {
               num = int.Parse(dt.Rows[0]["Num"].ToString());
           }

           if (num > 0)
           {
               throw new JRException("数据传递有误，已存在该单号！");
           }          
           #endregion
           int detailCount = PO_PomainHead.Details.Count;
           int head_id = 0;
           int body_en_Id = 0;
           GetId("00", "POMain", detailCount, out head_id, out body_en_Id);
           string sql = string.Format(@"insert into PO_Pomain(POID,cPOID,dPODate,cVenCode,cexch_name,nflat,iTaxRate,
                                        cMaker,cBusType,iDiscountTaxType,cmaketime,cState,cVerifier,iVTid,
                                        iverifystateex,cAuditDate,cAuditTime,csysbarcode)
                                        values
                                        (@POID,@cPOID,@dPODate,@cVenCode,@cexch_name,@nflat,@iTaxRate,
                                        @cMaker,@cBusType,@iDiscountTaxType,@cmaketime,@cState,@cVerifier,@iVTid,
                                        @iverifystateex,@cAuditDate,@cAuditTime,@csysbarcode)");
           SqlParameter[] para = {                               
                                      new SqlParameter("@POID",head_id),
                                      new SqlParameter("@cPOID",PO_PomainHead.cPOID),
                                      new SqlParameter("@dPODate",PO_PomainHead.dPODate),
                                      new SqlParameter("@cVenCode",PO_PomainHead.cVenCode),  
                                      new SqlParameter("@cexch_name",PO_PomainHead.cexch_name),
                                      new SqlParameter("@nflat",PO_PomainHead.nflat),
                                      new SqlParameter("@iTaxRate",PO_PomainHead.iTaxRate),
                                      new SqlParameter("@cMaker",PO_PomainHead.cMaker),
                                      new SqlParameter("@cBusType",PO_PomainHead.cBusType),
                                      new SqlParameter("@iDiscountTaxType",PO_PomainHead.iDiscountTaxType),
                                      new SqlParameter("@cmaketime",DateTime.Now),
                                      new SqlParameter("@cState",PO_PomainHead.cState),
                                      new SqlParameter("@cVerifier",PO_PomainHead.cMaker),
                                      new SqlParameter("@iVTid",PO_PomainHead.iVTid),
                                      new SqlParameter("@iverifystateex",PO_PomainHead.iVerifyStateex),
                                      new SqlParameter("@cAuditDate",DateTime.Now.ToShortDateString()),
                                      new SqlParameter("@cAuditTime",DateTime.Now),
                                      new SqlParameter("@csysbarcode",PO_PomainHead.cSysbarCode),
                   
                                  };
           int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
           if (headCount > 0)
           {
               try
               {
                   List<ExecuteHelp> sqlList = new List<ExecuteHelp>();
                   PO_PomainHeadID = GetPO_PomainHeadID(PO_PomainHead.cPOID);
                   for (int i = 0; i < detailCount; i++)
                   {
                       EntityPO_PoDetails body = PO_PomainHead.Details[i];
                       string bodySql = string.Format(@"insert into PO_PoDetails(ID,cInvCode,iQuantity,iUnitPrice,iMoney,
                                                        iTax,iSum,iNatUnitPrice,iNatMoney,iNatTax,iNatSum,dArriveDate,
                                                        iPerTaxRate,POID,iTaxPrice)
                                                        values
                                                        (@ID,@cInvCode,@iQuantity,@iUnitPrice,@iMoney,
                                                        @iTax,@iSum,@iNatUnitPrice,@iNatMoney,@iNatTax,@iNatSum,@dArriveDate,
                                                        @iPerTaxRate,@POID,@iTaxPrice)");
                       SqlParameter[] bodyPara = {                               
                                      new SqlParameter("@ID",body_en_Id-detailCount+i+1),
                                      new SqlParameter("@cInvCode",body.cInvCode),
                                      new SqlParameter("@iQuantity",body.iQuantity),
                                      new SqlParameter("@iUnitPrice",body.iUnitPrice),                                      
                                      new SqlParameter("@iMoney",body.iMoney),
                                      new SqlParameter("@iTax",body.iTax),
                                      new SqlParameter("@iSum",body.iSum),
                                      new SqlParameter("@iNatUnitPrice",body.iNatUnitPrice),
                                      new SqlParameter("@iNatMoney",body.iNatMoney),
                                      new SqlParameter("@iNatTax",body.iNatTax),
                                      new SqlParameter("@dArriveDate",DateTime.Now),
                                      new SqlParameter("@iNatSum",body.iNatSum),
                                      new SqlParameter("@iPerTaxRate",body.iPerTaxRate),
                                      new SqlParameter("@POID",PO_PomainHeadID),
                                      new SqlParameter("@iTaxPrice",body.iTaxPrice),                                 
                                      
                                  };
                       sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });
                   }
                   int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
               }
               catch (Exception ex)
               {
                   DeletePO_Pomain(PO_PomainHead.cPOID);
                   throw ex;
               }
           }
           else
           {
               throw new JRException("采购订单表头新增失败");
           }
           return (PO_PomainHeadID == null ? null : PO_PomainHeadID.ToString());
       }
       public int DeletePO_Pomain(string cPOID)
       {
           if (string.IsNullOrWhiteSpace(cPOID))
               return 0;
           List<ExecuteHelp> list = new List<ExecuteHelp>();
           string sql = "DELETE PO_Pomain WHERE cPOID = @cPOID";
           SqlParameter[] paras = {
                                       new SqlParameter("@cPOID",cPOID)
                                   };
           list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });
           int? POID = GetPO_PomainHeadID(cPOID);
           string delBodySql = "DELETE PO_PoDetails where POID = @POID";
           SqlParameter[] bodyParas = {
                                       new SqlParameter("@POID",POID)
                                   };
           list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });
           return this.DB_SqlHelper.ExecuteNonQuery(list);
       }
       public int? GetPO_PomainHeadID(string cPOID)
       {
           string sql = "SELECT POID FROM PO_Pomain WHERE cPOID = @cPOID";
           SqlParameter[] paras = {
                                       new SqlParameter("@cPOID",cPOID)
                                   };
           DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
           if (dt != null && dt.Rows.Count > 0)
           {
               return int.Parse(dt.Rows[0]["POID"].ToString());
           }

           return null;
       }
       /// <summary>
       /// 判断单号是否存在
       /// </summary>
       /// <param name="cPOID"></param>
       /// <returns></returns>
       public int GetPO_Pomain(string cPOID)
       {
           int num = 0;
           string cmdText = "select count(*) as Num from PO_Pomain where cPOID=@cPOID";
           SqlParameter[] paras = {
                                       new SqlParameter("@cPOID",cPOID)
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
