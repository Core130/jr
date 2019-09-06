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
    /// 销售订单处理类
    /// </summary>
    public class SoMainBll : U8BllBase
    {
        public SoMainBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }

        /// <summary>
        /// 新增销售订单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string AddSale(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntitySaleHead entity, out int success)
        {
            success = 0;
            SoMainBll bll = new SoMainBll(StrAccID, AccYear, UserCode, PlainPassword);
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            foreach (EntitySaleBody entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }

            if (bll.GetGlmendFlag(entity.dDate.Year, entity.dDate.Month, "bflag_SA") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cSOCode);
            }
            //销售部门 
            string deptName = GetDepartmentName(entity.cDepCode);
            if (string.IsNullOrWhiteSpace(deptName))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cDepCode);
            }

            // 客户   
            string cusName = GetCustomerName(entity.cCusCode);

            if (string.IsNullOrWhiteSpace(cusName))
            {
                return string.Format("U8中不存在客户编号:{0}!", entity.cCusCode);
            }
            entity.cCusName = cusName;
            //销售类型
            if (string.IsNullOrWhiteSpace(GetStName(entity.cSTCode)))
            {
                return string.Format("U8中不存在销售类型编码:{0}!", entity.cSTCode);
            }
            //　仓库   
            string warehouseName = GetWarehouseName(entity.cDefine12);
            if (string.IsNullOrWhiteSpace(warehouseName))
            {
                return string.Format("U8中不存在仓库编码:{0}!", entity.cDefine12);
            }
            #endregion
            var inventorys = GetInventorys("'" + string.Join("','", entity.Details.Select(p => p.cInvCode).Distinct()) + "'");

            for (int i = 0; i < entity.Details.Count; i++)
            {
                EntitySaleBody body = entity.Details[i];
                #region 验证单据明细
                var invInfo = inventorys.FirstOrDefault(p => p.cInvCode == body.cInvCode);
                // 存货编号             
                if (invInfo == null)
                {
                    return string.Format("U8中不存在存货编码:{0}!", body.cInvCode);
                }
                int bInvType = GetbInvType(body.cInvCode);

                if (body.iQuantity == 0 && bInvType == 0)
                {
                    return string.Format("数量不能为0!");
                }
                //金额验证
                if (bInvType != 1 && body.iQuotedPrice==0 && body.iSum != 0)
                {
                    return string.Format("存货编码:{0}金额异常!", body.cInvCode);
                }
                body.cInvName = invInfo.cInvName;

                #endregion

                #region 明细栏目计算

                body.iRowNo = (i + 1);
                body.bOrderBOM = 0;
                body.dPreDate = entity.dDate;
                body.dPreMoDate = entity.dDate;
                body.fcusminprice = 0;
                body.ballpurchase = 0;
                body.bOrderBOMOver = 0;
                body.busecusbom = 0;
                body.bsaleprice = 1;
                body.bgift = 0;
                body.KL = 100;
                body.cbSysBarCode = string.Format("||SA17|{0}|{1}", entity.cSOCode, body.iRowNo);

                if (bInvType == 1 )
                {
                    body.iTaxUnitPrice = 0;
                    body.iUnitPrice = 0;
                    body.iNatUnitPrice = 0;
                    body.iQuantity = 0;
                    body.iQuotedPrice = 0;
                    body.KL2 = 100;
                }
                else if(body.iQuotedPrice == 0)
                {
                    body.iTaxUnitPrice = 0;
                    body.iUnitPrice = 0;
                    body.iNatUnitPrice = 0;
                    body.KL2 = 100;
                }
                else
                {
                    body.iTaxUnitPrice = body.iSum / body.iQuantity;
                    body.iUnitPrice = Math.Round(body.iTaxUnitPrice / ((body.iTaxRate / 100) + 1), 2);
                    body.iNatUnitPrice = body.iUnitPrice;
                    body.iDisCount = Math.Round(body.iQuotedPrice * body.iQuantity - body.iSum, 2);
                    body.KL2 = (body.iSum / (body.iQuotedPrice * body.iQuantity)) * 100; 
                    
                }
                body.iMoney = Math.Round(body.iSum / ((body.iTaxRate / 100) + 1), 2);
                body.iTax = body.iSum - Math.Round(body.iSum / ((body.iTaxRate / 100) + 1), 2);               
                body.fSaleCost = GetSA_InvPrice(body.cInvCode);
                body.fSalePrice = body.fSaleCost * body.iQuantity;                
                body.iNatMoney = body.iMoney;
                body.iNatSum = body.iSum;
                body.iNatTax = body.iTax;
                
                #endregion
                // 订货平台过来的ＩＤ现在是存在自定义项25，以后一定要加表，以后一定要加表，以后一定要加表
            }

            // 设置默认值
            entity.cBusType = entity.cBusType == null ? "普通销售" : entity.cBusType;
            entity.cSTCode = entity.cSTCode == null ? "02" : entity.cSTCode;  // 暂时
            entity.cexch_name = "人民币";//币种
            entity.iExchRate = 1;//汇率
            entity.bReturnFlag = 0;
            entity.iVTid = "95";
            entity.cMaker = GetUserName(UserCode);
            entity.iStatus = 1;//审核状态（1-已审核，0-未审核）
            entity.cSysBarCode = string.Format("||SA17|{0}", entity.cSOCode);

            string id = InsertSale(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1; 
            return id;
        }

        /// <summary>
        /// 新增销售订单
        /// </summary>
        /// <param name="saleHead"></param>
        /// <returns></returns>
        public string InsertSale(EntitySaleHead saleHead)
        {
            int? saleHeadId;

            if (saleHead == null || saleHead.Details == null || saleHead.Details.Count == 0)
            {
                throw new JRException("销售订单新增失败!没有订单数据!");
            }
            #region 验证
            int num = 0;            
            num = GetSoMain(saleHead.cSOCode);
            if (num > 0)
            {
                throw new JRException("数据传递有误，已存在该订单号!");
            }         


            CreateDHColumn("SO_SODetails");

            #endregion

            // 得到主表ID和从表ID
            int detailCount = saleHead.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "SOMain", detailCount, out head_id, out body_end_Id);

            string sql = string.Format(@"
insert into SO_SOMain(cSOCode,dDate,cBusType,cSTCode,cCusCode,cPayCode,cDepCode,iTaxRate,cexch_name,cMemo,iExchRate,bReturnFlag,ID ,iVTid,cMaker,   
iMoney,iStatus,cDefine7,cCusName,dPreMoDateBT,dPreDateBT,ireturncount,bcashsale,bmustbook,cSysBarCode,contract_status,  
cDefine1,cDefine8,cDefine12,cCusOAddress,cVerifier,dverifydate,dverifysystime,cDefine14,cDefine3)
VALUES(@cSOCode,@dDate,@cBusType,@cSTCode,@cCusCode,@cPayCode,@cDepCode,@iTaxRate,@cexch_name,left(@cMemo,255),@iExchRate,@bReturnFlag,@soMainId ,@iVTid,@cMaker, 
null,@iStatus,null,@cCusName,@dDate,@dDate,null,0,0,@cSysBarCode,1,     
@cDefine1,@cDefine8,@cDefine12,@cCusOAddress,@cMaker,@dverifydate,@dverifysystime,@cDefine14,@cDefine3
)
");
            SqlParameter[] para = { 
                                      new SqlParameter("@cSOCode",saleHead.cSOCode),
                                      new SqlParameter("@dDate",saleHead.dDate.ToShortDateString()),
                                      new SqlParameter("@cBusType",GetDBValue(saleHead.cBusType)),
                                      new SqlParameter("@cSTCode",GetDBValue(saleHead.cSTCode)),
                                      new SqlParameter("@cCusCode",GetDBValue(saleHead.cCusCode)),
                                      new SqlParameter("@cPayCode",GetDBValue(saleHead.cPayCode)),
                                      new SqlParameter("@cDepCode",GetDBValue(saleHead.cDepCode)),
                                      new SqlParameter("@iTaxRate",saleHead.iTaxRate),
                                      new SqlParameter("@cexch_name",saleHead.cexch_name),
                                      new SqlParameter("@cMemo",GetDBValue(saleHead.cMemo)),
                                      new SqlParameter("@iExchRate",saleHead.iExchRate),
                                      new SqlParameter("@bReturnFlag",saleHead.bReturnFlag),
                                      new SqlParameter("@iVTid",saleHead.iVTid),
                                      new SqlParameter("@cMaker",GetDBValue(saleHead.cMaker)),  
                                      new SqlParameter("@cCusName",GetDBValue(saleHead.cCusName )),                                           
                                      new SqlParameter("@cSysBarCode",GetDBValue(saleHead.cSysBarCode )),     // U8 中特殊的规则       
                                      new SqlParameter("@cDefine1",GetDBValue(saleHead.cDefine1 )),                                             
                                      new SqlParameter("@cDefine8",GetDBValue(saleHead.cDefine8 )),                                             
                                      new SqlParameter("@cDefine12",GetDBValue(saleHead.cDefine12 )),                                             
                                      new SqlParameter("@cCusOAddress",GetDBValue(saleHead.cCusOAddress )),                                           
                                      new SqlParameter("@soMainId",head_id), 
                                      new SqlParameter("@iStatus",saleHead.iStatus),//审核状态
                                      new SqlParameter("@dverifydate",DateTime.Now.ToShortDateString()), 
                                      new SqlParameter("@dverifysystime",DateTime.Now),//审核时间
                                      new SqlParameter("@cDefine14",GetDBValue(saleHead.cDefine14)),//促销政策
                                      new SqlParameter("@cDefine3",GetDBValue(saleHead.cDefine3)),//计入任务
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();

                    saleHeadId = GetSaleID(saleHead.cSOCode);
                    if (saleHeadId == null)
                    {
                        throw new JRException("销售订单表头新增失败!");
                    }

                    // 组装SQL语句
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntitySaleBody body = saleHead.Details[i];

                        string bodySql = string.Format(@"
insert into SO_SODetails(cInvCode,iQuantity,iQuotedPrice,iUnitPrice,iMoney,bOrderBOM,iSOsID,ID,cSOCode,dPreDate,
iSum,iTaxRate,iNum,iTaxUnitPrice,iTax,iNatUnitPrice,iNatMoney,iNatTax,iNatSum,
iFHNum,iFHQuantity,iFHMoney,iKPQuantity,iKPNum,iKPMoney,KL,
KL2,cInvName,fSaleCost,fSalePrice,dPreMoDate,iRowNo ,
fcusminprice,ballpurchase,bOrderBOMOver,busecusbom,
bsaleprice,bgift,cbSysBarCode,DHID,cDefine22,cDefine23,iDisCount,iNatDisCount
)
VALUES(@cInvCode,@iQuantity,@iQuotedPrice,@iUnitPrice,@iMoney,@bOrderBOM,@iSOsID,@ID,@cSOCode,@dPreDate,
@iSum,@iTaxRate,null,@iTaxUnitPrice,@iTax,@iNatUnitPrice,@iNatMoney,@iNatTax,@iNatSum,
null,null,null,null,null,null,@KL,
@KL2,@cInvName,@fSaleCost,@fSalePrice,@dPreMoDate,@iRowNo ,
@fcusminprice,@ballpurchase,@bOrderBOMOver,@busecusbom,
@bsaleprice,@bgift,@cbSysBarCode,@DHID,@cDefine22,@cDefine23,@iDisCount,@iNatDisCount)

");
                        SqlParameter[] bodyPara = { 
                                      new SqlParameter("@cSOCode",saleHead.cSOCode),
                                      new SqlParameter("@cInvCode",body.cInvCode),
                                      new SqlParameter("@iQuantity",body.iQuantity),
                                      new SqlParameter("@iQuotedPrice",body.iQuotedPrice),
                                      new SqlParameter("@iUnitPrice",body.iUnitPrice),
                                      new SqlParameter("@iMoney",body.iMoney),
                                      new SqlParameter("@bOrderBOM",body.bOrderBOM),
                                      new SqlParameter("@ID",saleHeadId),
                                      new SqlParameter("@DHID",body.DHID),
                                      new SqlParameter("@dPreDate",body.dPreDate.ToShortDateString()),
                                      new SqlParameter("@iSum",body.iSum),
                                      new SqlParameter("@iTaxRate",body.iTaxRate),
                                      new SqlParameter("@iTaxUnitPrice",body.iTaxUnitPrice),
                                      new SqlParameter("@iTax",body.iTax),
                                      new SqlParameter("@iNatUnitPrice",body.iNatUnitPrice),
                                      new SqlParameter("@iNatMoney",body.iNatMoney),
                                      new SqlParameter("@iNatTax",body.iNatTax),
                                      new SqlParameter("@iNatSum",body.iNatSum),
                                      new SqlParameter("@KL",body.KL),
                                      new SqlParameter("@KL2",body.KL2),
                                      new SqlParameter("@cInvName",GetDBValue(body.cInvName)),
                                      new SqlParameter("@fSaleCost",body.fSaleCost),
                                      new SqlParameter("@fSalePrice",body.fSalePrice),
                                      new SqlParameter("@dPreMoDate",body.dPreMoDate.ToShortDateString()),
                                      new SqlParameter("@iRowNo",body.iRowNo),
                                      new SqlParameter("@fcusminprice",body.fcusminprice),
                                      new SqlParameter("@ballpurchase",body.ballpurchase),
                                      new SqlParameter("@bOrderBOMOver",body.bOrderBOMOver),
                                      new SqlParameter("@busecusbom",body.busecusbom),
                                      new SqlParameter("@bsaleprice",body.bsaleprice),
                                      new SqlParameter("@bgift",body.bgift),
                                      new SqlParameter("@cbSysBarCode",body.cbSysBarCode),
                                      new SqlParameter("@iSOsID",(body_end_Id - detailCount + i + 1)),
                                      new SqlParameter("@cDefine22",GetDBValue(body.cDefine22)),
                                      new SqlParameter("@cDefine23",GetDBValue(body.cDefine23)),
                                      new SqlParameter("@iDisCount",body.iDisCount),
                                      new SqlParameter("@iNatDisCount",body.iDisCount),
                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });

                    }

                    // 执行SQL
                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚
                    DeleteSale(saleHead.cSOCode);

                    throw ex;
                }
            }
            else
            {
                throw new JRException("销售订单表头新增失败!");
            }

            return (saleHeadId == null ? null : saleHeadId.ToString());
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int DeleteSale(string cSOCode)
        {
            if (string.IsNullOrWhiteSpace(cSOCode))
                return 0;

            List<ExecuteHelp> list = new List<ExecuteHelp>();

            string sql = "DELETE SO_SOMain WHERE cSOCode = @cSOCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cSOCode",cSOCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });

            string delBodySql = "DELETE SO_SODetails where cSOCode = @cSOCode";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@cSOCode",cSOCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }

        /// <summary>
        /// 获取销售订单表头ID
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int? GetSaleID(string cSOCode)
        {
            string sql = "SELECT ID FROM SO_SOMain WHERE cSOCode = @cSOCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cSOCode",cSOCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["ID"].ToString());
            }

            return null;
        }


        /// <summary>
        /// 查询销售订单号是否存在
        /// </summary>
        /// <param name="saleHead"></param>
        /// <returns></returns>
        public int QuerySale(string csocode)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from SO_SOMain where cSOCode=@cSOCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cSOCode",csocode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }
            return num;
        }

        /// <summary>
        /// 打开关闭销售订单
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int OpenCloseSale(string cSOCode, string UserName, string OType)
        {
            string sql = string.Empty;
            if (OType == "close")
            {
                sql = string.Format(@"update SO_SOMain set cCloser=@UserName,dclosesystime=@Date,dclosedate=@Date where cSOCode in (@cSOCode)
                update SO_SODetails set cSCloser=@UserName,dbclosesystime=@Date,dbclosedate=@Date where cSOCode in (@cSOCode)");
            }
            else
            {
                sql = string.Format(@"update SO_SOMain set cCloser=NULL,dclosesystime=NULL,dclosedate=NULL where cSOCode in (@cSOCode)
                update SO_SODetails set cSCloser=NULL,dbclosesystime=NULL,dbclosedate=NULL where cSOCode in (@cSOCode)");
            }

            SqlParameter[] paras = {
                                        new SqlParameter("@Date",DateTime.Now),
                                        new SqlParameter("@cSOCode",cSOCode),
                                        new SqlParameter("@UserName",UserName),

                                   };
            return this.DB_SqlHelper.ExecuteNonQuery(sql, paras);
        }

    }
}
