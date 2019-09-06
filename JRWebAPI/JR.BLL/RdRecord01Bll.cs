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
    /// 采购入库单
    /// </summary>
    public class RdRecord01Bll : U8BllBase
    {
        public RdRecord01Bll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        /// <summary>
        /// 新增采购入库单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="kind"> 0:采购入库单；1:红字采购入库单</param>
        /// <param name="entity"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public string AddRdRecord01(string UserCode, string PlainPassword,
                                   string StrAccID, int AccYear, string Act, int kind, EntityRdRecord01Head entity, out int success)
        {
            success = 0;
            RdRecord01Bll bll = new RdRecord01Bll(StrAccID, AccYear, UserCode, PlainPassword);
            #region 验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }
            foreach (Entityrdrecords01Body entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }

            if(bll.GetGlmendFlag(entity.dDate.Year,entity.dDate.Month,"bflag_ST")== true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cCode);
            }
            //供应商
            if (!string.IsNullOrEmpty(entity.cVenCode))
            {
                string vendorname = bll.GetVendorName(entity.cVenCode);
                if (string.IsNullOrEmpty(vendorname))
                    return string.Format("U8中不存在供应商编码:{0}!", entity.cVenCode);
            }

            //　仓库   
            string warehouseName = bll.GetWarehouseName(entity.cWhCode);
            if (string.IsNullOrWhiteSpace(warehouseName))
            {
                return string.Format("U8中不存在仓库编码:{0}!!", entity.cWhCode);
            }
            //采购类型
            List<EntityPurchaseType> purchasetype = bll.GetPurchaseType();
            if (purchasetype.Count(p => p.cPTCode == entity.cPTCode) == 0)
            {
                return string.Format("U8中不存在采购类型编码:{0}!!", entity.cPTCode);
            }

            if (GetRdRecord01(entity.cCode) != 0)
            {
                return string.Format("U8中已存在该入库单号{0}!", entity.cCode);
            }
            #endregion
            #region 表头
            //entity.ID = 1;//收发记录主表标识
            entity.bRdFlag = 1;//收发标志
            entity.cVouchType = "01";//单据类型编码
            entity.cBusType = "普通采购";//业务类型       
            entity.cSource = string.IsNullOrWhiteSpace(entity.cOrderCode) ? "库存" : "采购订单"; //单据来源
            entity.cRdCode = string.IsNullOrWhiteSpace(entity.cRdCode) ? "02" : entity.cRdCode;  //收发类别编码

            // 收发类别编码         
            if (string.IsNullOrWhiteSpace(bll.GetRdStyleName(entity.cRdCode, 1)))
            {
                return string.Format("U8中不存在入库类别编码:{0}!", entity.cRdCode);
            }

            entity.bTransFlag = 0;//是否传递
            entity.cMaker = string.IsNullOrWhiteSpace(entity.cMaker) ? bll.GetUserName(UserCode) : entity.cMaker;
            entity.VT_ID = 27;//单据模板号
            entity.bIsSTQc = 0;//库存期初标志
            entity.iExchRate = 1;//汇率
            entity.cExch_Name = entity.cExch_Name == null ? "人民币" : entity.cExch_Name;//币种名称
            entity.cMemo = entity.cMemo == null ? "" : entity.cMemo;//备注
            entity.cSysbarCode = "||st01|" + entity.cCode;

            #endregion

            #region 表体
            foreach (Entityrdrecords01Body body in entity.Details)
            {
                // 存货编号 
                string invName = GetInventoryName(body.cInvCode);
                if (string.IsNullOrWhiteSpace(invName))
                {
                    return string.Format("U8中不存在存货编码:{0}!", body.cInvCode);
                }
                if (!string.IsNullOrWhiteSpace(body.cBatch) && bll.GetInventoryName(body.cInvCode, 4) == "0")
                {
                    return string.Format("U8中存货编码:{0}未启用批次管理，批次信息必须为空!", body.cInvCode);
                }
                if (string.IsNullOrWhiteSpace(body.cBatch) && bll.GetInventoryName(body.cInvCode, 4) == "1")
                {
                    return string.Format("U8中存货编码:{0}启用批次管理，批次信息不能为空!", body.cInvCode);
                };
                int bInvType = GetbInvType(body.cInvCode);
                if (bInvType == 1) body.iQuantity = 0;

                if (kind == 0 && body.iQuantity <= 0 && bInvType == 0)
                {
                    return string.Format("采购入库单入库数量不能小于或等于0!");
                }

                if (kind == 1 && body.iQuantity >= 0 && bInvType == 0)
                {
                    return string.Format("红字采购入库单入库数量不能大于或等于0!");
                }
                //金额验证
                if (bInvType == 0 && Math.Round(body.ioriSum / body.iQuantity, 2, MidpointRounding.AwayFromZero) != Math.Round(body.iOriTaxCost, 2))
                {
                    return string.Format("存货编码:{0}金额异常!", body.cInvCode);
                }

                body.iUnitCost = Math.Round(body.iOriTaxCost / (1 + body.iTaxRate / 100), 2, MidpointRounding.AwayFromZero);//单价（原币无税单价）
                body.iPrice = Math.Round(body.ioriSum / (1 + body.iTaxRate / 100), 2, MidpointRounding.AwayFromZero);//金额(无税金额)
                body.iAPrice = body.iPrice;//暂估金额
                body.iFlag = 0;//标志
                body.fACost = body.iUnitCost;//暂估单价
                body.chVencode = entity.cVenCode;//表头供应商
                body.iOriCost = body.iUnitCost;//原币无税单价
                body.iOriMoney = body.iPrice;//原币无税金额
                body.iOriTaxPrice = body.ioriSum - body.iPrice;//原币税额
                body.iTaxPrice = body.iOriTaxPrice;//本币税额
                body.iSum = body.ioriSum;//本币价税合计
                body.iBillSettleCount = 0;//单据结算次数
                body.iMatSettleState = 0;//结算状态
                entity.iTaxRate = body.iTaxRate;
                body.bCosting = 1;
            }
            #endregion
            string id = bll.InsertRdRecord01(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;

        }
        public string InsertRdRecord01(EntityRdRecord01Head RdRecord01Head)
        {
            int? RdRecord01HeadID;
            if (RdRecord01Head == null || RdRecord01Head.Details == null || RdRecord01Head.Details.Count == 0)
            {
                throw new JRException("采购入库单新增失败!没有数据!");
            }
            #region 验证          

            string POID = null;
            //传入采购订单号时判断对应的采购订单是否存在
            if (!string.IsNullOrWhiteSpace(RdRecord01Head.cOrderCode))
            {
                POID = GetPOID(RdRecord01Head.cOrderCode);
                if (string.IsNullOrEmpty(POID))
                {
                    throw new JRException(string.Format("U8中不存在采购订单号:{0}", RdRecord01Head.cOrderCode));
                }
            }
            #endregion
            int detailCount = RdRecord01Head.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;

            GetId("00", "rd", detailCount, out head_id, out body_end_Id);
            string sql = string.Format(@"insert into RdRecord01(ID,bRdFlag,cVouchType,cBusType,cSource,cWhCode,
                                         dDate,cCode,cRdCode,cPTCode,cVenCode,cMemo,bTransFlag,cMaker,VT_ID,bIsSTQc,
                                         iTaxRate,iExchRate,cExch_Name,dnmaketime,csysbarcode,cOrderCode,ipurorderid)
                                         values
                                         (@ID,@bRdFlag,@cVouchType,@cBusType,@cSource,@cWhCode,
                                         @dDate,@cCode,@cRdCode,@cPTCode,@cVenCode,left(@cMemo,255),@bTransFlag,@cMaker,@VT_ID,@bIsSTQc,
                                         @iTaxRate,@iExchRate,@cExch_Name,@dnmaketime,@csysbarcode,@cOrderCode,@ipurorderid)");

            SqlParameter[] para = {                               
                                      new SqlParameter("@ID",head_id),
                                      new SqlParameter("@bRdFlag",RdRecord01Head.bRdFlag),
                                      new SqlParameter("@cVouchType",RdRecord01Head.cVouchType),
                                      new SqlParameter("@cBusType",RdRecord01Head.cBusType),
                                      new SqlParameter("@cSource",RdRecord01Head.cSource),
                                      new SqlParameter("@cWhCode",RdRecord01Head.cWhCode),
                                      new SqlParameter("@dDate",RdRecord01Head.dDate),
                                      new SqlParameter("@cCode",RdRecord01Head.cCode),
                                      new SqlParameter("@cRdCode",RdRecord01Head.cRdCode),
                                      new SqlParameter("@cPTCode",RdRecord01Head.cPTCode),
                                      new SqlParameter("@cVenCode",RdRecord01Head.cVenCode),
                                      new SqlParameter("@cMemo",RdRecord01Head.cMemo),
                                      new SqlParameter("@bTransFlag",RdRecord01Head.bTransFlag), 
                                      new SqlParameter("@cMaker",RdRecord01Head.cMaker),  
                                      new SqlParameter("@VT_ID",RdRecord01Head.VT_ID),  
                                      new SqlParameter("@bIsSTQc",RdRecord01Head.bIsSTQc),  
                                      new SqlParameter("@iTaxRate",RdRecord01Head.iTaxRate),  
                                      new SqlParameter("@iExchRate",RdRecord01Head.iExchRate),  
                                      new SqlParameter("@cExch_Name",RdRecord01Head.cExch_Name),  
                                      new SqlParameter("@dnmaketime", DateTime.Now), 
                                      new SqlParameter("@csysbarcode",RdRecord01Head.cSysbarCode),
                                      new SqlParameter("@cOrderCode",GetDBValue(RdRecord01Head.cOrderCode)),
                                      new SqlParameter("@ipurorderid",GetDBValue(POID)),
                                  };

            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();
                    RdRecord01HeadID = GetRdRecord01HeadID(RdRecord01Head.cCode);
                    if (RdRecord01HeadID == null)
                    {
                        throw new JRException("采购入库单表头新增失败!");
                    }
                    string getCurrentStockSql = GetCurrentStockSql("Rd01");

                    for (int i = 0; i < detailCount; i++)
                    {
                        Entityrdrecords01Body body = RdRecord01Head.Details[i];
                        string bodySql = string.Format(@"insert into rdrecords01(AutoID,ID,cInvCode,iQuantity,iUnitCost,iPrice,
                                                         iAPrice,iFlag,fACost,chVencode,iOriTaxCost,iOriCost,iOriMoney,iOriTaxPrice,
                                                         ioriSum,iTaxRate,iTaxPrice,iSum,iBillSettleCount,iMatSettleState,iNQuantity
                                                         ,bCosting,irowno,cbsysbarcode,cPOID,iPOsID,cBatch
                                                         ,iSQuantity,iSNum,iMoney,dSDate,bTaxCost,cbaccounter,corufts,isotype,iProductType)
                                                         values
                                                         (@AutoID,@ID,@cInvCode,@iQuantity,@iUnitCost,@iPrice,
                                                         @iAPrice,@iFlag,@fACost,@chVencode,@iOriTaxCost,@iOriCost,@iOriMoney,@iOriTaxPrice,
                                                         @ioriSum,@iTaxRate,@iTaxPrice,@iSum,@iBillSettleCount,@iMatSettleState,@iNQuantity
                                                         ,@bCosting,@irowno,@cbsysbarcode,@cPOID,@iPOsID,@cBatch
                                                         ,0,0,0,NULL,1,NULL,NULL,0,0);
                                                        INSERT INTO IA_ST_UnAccountVouch01 (IDUN,IDSUN,cVouTypeUN,cBustypeUN)
                                                        VALUES(@ID,@AutoID,'01','普通采购');                                                  
                                                        {0}
                                                        update po_podetails set iReceivedQTY=isnull(iReceivedQTY,0) + @iQuantity where POID=@POID and cInvCode =@cInvCode", getCurrentStockSql);
                        SqlParameter[] bodyPara = {                               
                                      new SqlParameter("@AutoID",body_end_Id-detailCount+i+1),
                                      new SqlParameter("@ID",RdRecord01HeadID),
                                      new SqlParameter("@cInvCode",body.cInvCode),
                                      new SqlParameter("@iQuantity",body.iQuantity),
                                      new SqlParameter("@iUnitCost",body.iUnitCost),
                                      new SqlParameter("@iPrice",body.iPrice),
                                      new SqlParameter("@iAPrice",body.iAPrice),
                                      new SqlParameter("@iFlag",body.iFlag),
                                      new SqlParameter("@fACost",body.fACost),
                                      new SqlParameter("@chVencode",body.chVencode),
                                      new SqlParameter("@iOriTaxCost",body.iOriTaxCost),
                                      new SqlParameter("@iOriCost",body.iOriCost), 
                                      new SqlParameter("@iOriMoney",body.iOriMoney),  
                                      new SqlParameter("@iOriTaxPrice",body.iOriTaxPrice),  
                                      new SqlParameter("@ioriSum",body.ioriSum),  
                                      new SqlParameter("@iTaxRate",body.iTaxRate),  
                                      new SqlParameter("@iTaxPrice",body.iTaxPrice),  
                                      new SqlParameter("@iSum",body.iSum),
                                      new SqlParameter("@iBillSettleCount",body.iBillSettleCount),  
                                      new SqlParameter("@iMatSettleState",body.iMatSettleState),  
                                      new SqlParameter("@iNQuantity",body.iQuantity), 
                                      new SqlParameter("@bCosting",body.bCosting),
                                      new SqlParameter("@irowno",i+1),
                                      new SqlParameter("@cbsysbarcode",RdRecord01Head.cSysbarCode + "|"+ (i+1)),
                                      new SqlParameter("@cPOID",GetDBValue(RdRecord01Head.cOrderCode)),
                                      new SqlParameter("@iPOsID",GetDBValue(GetiPOsID(POID,body.cInvCode))),
                                      new SqlParameter("@POID",GetDBValue(POID)),
                                      new SqlParameter("@cBatch",GetDBValue(body.cBatch)),
                                      new SqlParameter("@cWhCode",GetDBValue(RdRecord01Head.cWhCode)),
                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });
                    }
                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚
                    DeleteRdRecord01(RdRecord01Head.cCode);
                    throw ex;
                }
            }
            else
            {
                throw new JRException("采购入库单表头新增失败！");
            }
            return (RdRecord01HeadID == null ? null : RdRecord01HeadID.ToString());
        }

        /// <summary>
        /// 删除单据
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="entity"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public string DelRdRecord01(string UserCode, string PlainPassword,
                                   string StrAccID, int AccYear, string Act, EntityRdRecord01Head entity, out int success)
        {
            success = 1;
            string msg = "";
            RdRecord01Bll bll = new RdRecord01Bll(StrAccID, AccYear, UserCode, PlainPassword);
            if (bll.GetGlmendFlag(entity.dDate.Year, entity.dDate.Month, "bflag_ST") == true)
            {
                success = 0;
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cCode);
            }
            if (GetRdRecord01(entity.cCode) != 0)
            {
                if (RdRecord01Checked(entity.cCode) == 0)
                {
                    msg = string.Format("U8中采购入库单:{0} 已审核,不允许删除!", entity.cCode);
                    success = 0;
                }
                else
                {
                    //执行删除采购入库单操作
                    DeleteRdRecord01(entity.cCode);
                    msg = string.Format("U8中采购入库单:{0} 已删除!", entity.cCode);
                    success = 1;
                }
            }
            else
            {
                msg = string.Format("U8中采购入库单:{0} 不存在!", entity.cCode);
            }
            return msg;
        }
        public int? GetRdRecord01HeadID(string cCode)
        {
            string sql = "SELECT ID FROM RdRecord01 WHERE cCode = @cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["ID"].ToString());
            }

            return null;
        }

        public int DeleteRdRecord01(string cCode)
        {
            if (string.IsNullOrWhiteSpace(cCode))
                return 0;

            //处理现存量数据
            var stockSql = GetCurrentStockSql("Rd01", -1);
            var head = GetEntityRdRecord01Head(cCode);
            if (head.Details.Count != 0)
            {
                var sqlList = head.Details.Select(p => new ExecuteHelp
                {
                    SQL = stockSql,
                    Parameters = new SqlParameter[] { 
                                      new SqlParameter("@cWhCode",GetDBValue(head.cWhCode)),
                                      new SqlParameter("@cInvCode",GetDBValue(p.cInvCode)),
                                      new SqlParameter("@cBatch",GetDBValue(p.cBatch)),  
                                      new SqlParameter("@iQuantity",p.iQuantity),        
                     }
                }).ToList();
                // 执行处理现存量操作语句
                this.DB_SqlHelper.ExecuteNonQuery(sqlList);
            }
            List<ExecuteHelp> list = new List<ExecuteHelp>();

            string sql = "DELETE RdRecord01 WHERE cCode = @cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });
            int? rdrecordsID = GetRdRecord01HeadID(cCode);
            string delBodySql = "DELETE rdrecords01 where ID = @ID";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@ID",rdrecordsID)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }
        /// <summary>
        /// 判断该单号是否存在： 0 不存在 否则存在
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public int GetRdRecord01(string cCode)
        {
            string sql = "select count(*) as Num from RdRecord01 where cCode=@cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            return Convert.ToInt32(this.DB_SqlHelper.ExecuteScalar(CommandType.Text, sql, paras).ToString());
        }
        /// <summary>
        /// 获取采购订单ID
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public string GetPOID(string cCode)
        {
            string POID = null;
            string cmdText = "select POID from po_pomain where cPOID = @cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                POID = dt.Rows[0]["POID"].ToString();
            }
            return POID;
        }
        /// <summary>
        /// 获取采购订单子表ID
        /// </summary>
        /// <param name="POID"></param>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public string GetiPOsID(string POID, string cInvCode)
        {
            if (string.IsNullOrWhiteSpace(POID)) return null;
            string iPOsID = null;
            string cmdText = "select ID from po_podetails where POID = @POID  and  cInvCode = @cInvCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@POID",POID),
                                       new SqlParameter("@cInvCode",cInvCode),
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                iPOsID = dt.Rows[0]["ID"].ToString();
            }
            return iPOsID;
        }
        /// <summary>
        /// 单据有没审核，返回 0 已审核，否则未审核
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public int RdRecord01Checked(string cCode)
        {
            string sql = "select count(*) as Num from RdRecord01 where ISNULL(cHandler,'')='' and  cCode=@cCode ";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            return Convert.ToInt32(this.DB_SqlHelper.ExecuteScalar(CommandType.Text, sql, paras).ToString());
        }
        /// <summary>
        /// 获取采购入库单实体数据
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public EntityRdRecord01Head GetEntityRdRecord01Head (string cCode)
        {
            EntityRdRecord01Head entity = new EntityRdRecord01Head();
            List<Entityrdrecords01Body> listBody = new List<Entityrdrecords01Body>();
            string sqlHead = "SELECT cCode ,dDate,cVenCode,cPTCode,cWhCode,cRdCode,cOrderCode,cMaker,cMemo FROM RdRecord01 WHERE cCode = @cCode ";
            string sqlBody = "SELECT cInvCode,iQuantity,iOriTaxCost,ioriSum,iTaxRate,cBatch FROM RdRecords01 WHERE ID = (SELECT ID FROM RdRecord01 WHERE cCode = @cCode )";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            SqlDataReader drHead = DB_SqlHelper.ExecuteReader(CommandType.Text, sqlHead, paras);
            SqlDataReader drBody = DB_SqlHelper.ExecuteReader(CommandType.Text, sqlBody, paras);
            while (drBody.Read())
            {
                Entityrdrecords01Body body = new Entityrdrecords01Body();
                body.cInvCode = drBody["cInvCode"].ToString();            
                body.iQuantity = Convert.ToDecimal(drBody["iQuantity"] ?? 0);
                body.iOriTaxCost = Convert.ToDecimal(drBody["iOriTaxCost"] ?? 0);
                body.ioriSum = Convert.ToDecimal(drBody["ioriSum"] ?? 0);
                body.iTaxRate = Convert.ToDecimal(drBody["iTaxRate"] ?? 0);
                body.cBatch = drBody["cBatch"].ToString();
                listBody.Add(body);
            }

            if (drHead.Read())
            {
                entity.cCode = cCode;
                entity.dDate = Convert.ToDateTime(drHead["dDate"]);
                entity.cVenCode = drHead["cVenCode"].ToString();
                entity.cPTCode = drHead["cPTCode"].ToString();
                entity.cWhCode = drHead["cWhCode"].ToString();
                entity.cRdCode = drHead["cRdCode"].ToString();
                entity.cOrderCode = drHead["cOrderCode"].ToString();
                entity.cMaker = drHead["cMaker"].ToString();                
                entity.cMemo = drHead["cMemo"].ToString();
                entity.Details = listBody;
            }
            return entity;
        }
    }
}
