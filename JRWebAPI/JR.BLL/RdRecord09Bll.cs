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
    /// 其他出库单
    /// </summary>
    public class RdRecord09Bll : U8BllBase
    {
        public RdRecord09Bll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        /// <summary>
        /// 新增其他出库单
        /// </summary>
        /// <param name="UserCode">用户名</param>
        /// <param name="PlainPassword">密码</param>
        /// <param name="StrAccID">账套号</param>
        /// <param name="AccYear">年份</param>
        /// <param name="Act">动作</param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string AddRdRecord09(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityRdRecord09Head entity, out int success)
        {
            success = 0;
            RdRecord09Bll bll = new RdRecord09Bll(StrAccID, AccYear, UserCode, PlainPassword);

            #region 验证
            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            foreach (EntityRdRecord09Body entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }

            if (bll.GetGlmendFlag(entity.dDate.Year, entity.dDate.Month, "bflag_ST") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cCode);
            }
            // 部门 
            string deptName = GetDepartmentName(entity.cDepCode);
            if (string.IsNullOrWhiteSpace(deptName))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cDepCode);
            }
            entity.cDefine11 = deptName;

            //　仓库   
            string warehouseName = bll.GetWarehouseName(entity.cWhCode);
            if (string.IsNullOrWhiteSpace(warehouseName))
            {
                return string.Format("U8中不存在仓库编码:{0}!!", entity.cWhCode);
            }

            // 收发类别编码     
            if (string.IsNullOrWhiteSpace(bll.GetRdStyleName(entity.cRdCode)))
            {
                return string.Format("U8中不存在收发类别编码:{0}!", entity.cRdCode);
            }

            // 客户编号  
            string cusName = bll.GetCustomerName(entity.cCusCode);

            if (string.IsNullOrWhiteSpace(cusName))
            {
                return string.Format("U8中不存在客户编号:{0}!", entity.cCusCode);
            }
            #endregion

            string msg1 = "";
            for (int i = 0; i < entity.Details.Count; i++)
            {
                EntityRdRecord09Body body = entity.Details[i];
                #region 验证单据明细
                // 存货编号 
                string invName = bll.GetInventoryName(body.cInvCode);
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
                if (body.iQuantity == 0)
                {
                    return string.Format("U8中存货编码:{0}数量不能等于0!", body.cInvCode);
                }
                // 可用量验证

                EntityCurrentStock xclEntity = new EntityCurrentStock();
                xclEntity.cInvCode = body.cInvCode;
                xclEntity.cWhCode = entity.cWhCode;
                CurrentStockBll csBll = new CurrentStockBll(StrAccID, AccYear, UserCode, PlainPassword);
                var csData = csBll.GetCurrentStock(xclEntity, out msg).Sum(p => p.iQuantity);
                if (csData < body.iQuantity)
                {
                    msg = (string.Format("存货编码:{0}可用量不足，当前可用量为：{1}；", body.cInvCode, csData));
                    msg1 += msg;
                }
                #endregion

                #region 明细栏目计算
                body.iUnitCost = bll.GetInvSPrice(body.cInvCode);
                body.iAPrice = body.iUnitCost * body.iQuantity;
                body.irowno = (i + 1);
                body.cDefine22 = string.IsNullOrEmpty(body.cDefine22) ? bll.GetInventoryName(body.cInvCode, 2) : body.cDefine22;
                body.cDefine23 = string.IsNullOrEmpty(body.cDefine23) ? bll.GetInventoryName(body.cInvCode, 3) : body.cDefine23;
                body.cbsysbarcode = string.Format("||st09|{0}|{1}", entity.cCode, body.irowno);
                body.iFlag = 0;//标志
                #endregion
            }
            if (msg1 != "")
            {
                return msg1;
            }


            //设置默认值
            entity.bRdFlag = 0;//收发标志
            entity.blsSTQc = 0;//库存期初标志
            entity.cVouchType = "09";//单据类型编码
            entity.cSource = "库存";//单据来源
            entity.bTransFlag = 0;//是否传递
            entity.cMaker = string.IsNullOrWhiteSpace(entity.cMaker) ? bll.GetUserName(UserCode) : entity.cMaker;
            entity.cBusType = "其他出库";
            entity.VT_ID = 85;
            entity.bOMFirst = 0;
            entity.dnmaketime = entity.dDate;
            entity.cDefine11 = cusName;
            entity.csysbarcode = string.Format("st09|{0}", entity.cCode);

            string id = bll.InsertRdRecord09(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }


        /// <summary>
        /// 新增其他出库单
        /// </summary>
        /// <param name="saleHead"></param>
        /// <returns></returns>
        public string InsertRdRecord09(EntityRdRecord09Head RdRecord09Head)
        {
            int? RdRecord09Id;

            if (RdRecord09Head == null || RdRecord09Head.Details == null || RdRecord09Head.Details.Count == 0)
            {
                throw new JRException("配送出库单新增失败!没有数据!");
            }
            #region  验证

            // 单据是否重复
            int num = 0;
            string cmdText = "select count(*) as Num from RdRecord09 where cCode=@cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",RdRecord09Head.cCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }


            if (num > 0)
            {
                throw new JRException("数据传递有误，ERP中已存在该出库单号!");
            }

            CreateDHColumn("rdrecords09");

            #endregion

            // 得到主表ID和从表ID
            int detailCount = RdRecord09Head.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "rd", detailCount, out head_id, out body_end_Id);

            string sql = string.Format(@"
                                        insert into RdRecord09( bRdFlag,cWhCode,bIsSTQc,ID,cVouchType,cSource,dDate,cCode,bTransFlag,
                                        cBusType,cRdCode,cDepCode,cMaker,cDefine1,cCusCode,cDefine8,cDefine10,
                                        VT_ID,cDefine11,cShipAddress,bOMFirst,dnmaketime,csysbarcode,cMemo
                                        )
                                         VALUES(@bRdFlag,@cWhCode,@bIsSTQc,@RdRecord09Id,@cVouchType,@cSource,@dDate,@cCode,@bTransFlag,
                                        @cBusType,@cRdCode,@cDepCode,@cMaker,@cDefine1,@cCusCode,@cDefine8,@cDefine10,
                                        @VT_ID,@cDefine11,@cShipAddress,@bOMFirst,@dnmaketime,@csysbarcode,left(@cMemo,255)
                                        )");
            SqlParameter[] para = { 
                                      new SqlParameter("@bRdFlag",RdRecord09Head.bRdFlag),
                                      new SqlParameter("@cWhCode",GetDBValue(RdRecord09Head.cWhCode)),
                                      new SqlParameter("@bIsSTQc",RdRecord09Head.blsSTQc),
                                      new SqlParameter("@RdRecord09Id",head_id), // 主表ID
                                      new SqlParameter("@cVouchType",GetDBValue(RdRecord09Head.cVouchType)),
                                      new SqlParameter("@cSource",GetDBValue(RdRecord09Head.cSource)),
                                      new SqlParameter("@dDate",RdRecord09Head.dDate.ToShortDateString()),
                                      new SqlParameter("@cCode",GetDBValue(RdRecord09Head.cCode)),
                                      new SqlParameter("@bTransFlag",RdRecord09Head.bTransFlag),
                                      new SqlParameter("@cBusType",GetDBValue(RdRecord09Head.cBusType)),
                                      new SqlParameter("@cRdCode",GetDBValue(RdRecord09Head.cRdCode)),
                                      new SqlParameter("@cDepCode",GetDBValue(RdRecord09Head.cDepCode)),
                                      new SqlParameter("@cMaker",GetDBValue(RdRecord09Head.cMaker)),
                                      new SqlParameter("@cDefine1",GetDBValue(RdRecord09Head.cDefine1)),
                                      new SqlParameter("@cCusCode",GetDBValue(RdRecord09Head.cCusCode)),
                                      new SqlParameter("@cDefine8",GetDBValue(RdRecord09Head.cDefine8)),
                                      new SqlParameter("@cDefine10",GetDBValue(RdRecord09Head.cDefine10)),
                                      new SqlParameter("@cDefine11",GetDBValue(RdRecord09Head.cDefine11)),
                                      new SqlParameter("@VT_ID",RdRecord09Head.VT_ID),
                                      new SqlParameter("@cShipAddress",GetDBValue(RdRecord09Head.cShipAddress)),
                                      new SqlParameter("@bOMFirst",RdRecord09Head.bOMFirst),
                                      new SqlParameter("@dnmaketime",RdRecord09Head.dnmaketime),
                                      new SqlParameter("@csysbarcode",GetDBValue(RdRecord09Head.csysbarcode)),
                                      new SqlParameter("@cMemo",GetDBValue(RdRecord09Head.cMemo)),
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();

                    RdRecord09Id = GetRdRecord09ID(RdRecord09Head.cCode);//求出库单主表ID（主键）
                    if (RdRecord09Id == null)
                    {
                        throw new JRException("配送出库单表头新增失败!");
                    }
                    string getCurrentStockSql = GetCurrentStockSql("Rd09");
                    // 组装SQL语句
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntityRdRecord09Body body = RdRecord09Head.Details[i];

                        string bodySql = string.Format(@"
                                                        insert into rdrecords09(iFlag,cInvCode,AutoID,ID,iQuantity,
                                                        iUnitCost,iAPrice,cDefine26,cDefine27,irowno,cbsysbarcode,cbaccounter,bCosting,
                                                        iVMISettleQuantity,iVMISettleNum,corufts,iExpiratDateCalcu,isotype,DHID,cDefine22,cDefine23,cBatch
                                                        )
                                                        VALUES(@iFlag,@cInvCode,@AutoID,@ID,@iQuantity,
                                                        @iUnitCost,@iAPrice,@cDefine26,@cDefine27,@irowno,@cbsysbarcode,null,1,
                                                        null,null,null,0,0,@DHID,@cDefine22,@cDefine23,@cBatch);
                                                     
                                                        INSERT INTO IA_ST_UnAccountVouch09 (IDUN,IDSUN,cVouTypeUN,cBustypeUN)
                                                        VALUES(@ID,@AutoID,'09','其他出库');
                                                        INSERT INTO [rdrecords09sub]
                                                        ([AutoID],[ID],[cBG_ItemCode],[cBG_ItemName],[cBG_CaliberKey1],[cBG_CaliberKeyName1],[cBG_CaliberKey2],[cBG_CaliberKeyName2]
                                                        ,[cBG_CaliberKey3],[cBG_CaliberKeyName3],[cBG_CaliberCode1],[cBG_CaliberName1],[cBG_CaliberCode2],[cBG_CaliberName2],[cBG_CaliberCode3],
                                                        [cBG_CaliberName3],[iBG_Ctrl],[cBG_Auditopinion],[iBGSTSum],[iBGIASum],[cBG_CaliberKey4],[cBG_CaliberKeyName4],[cBG_CaliberKey5],[cBG_CaliberKeyName5],
                                                        [cBG_CaliberKey6],[cBG_CaliberKeyName6],[cBG_CaliberCode4]
                                                        ,[cBG_CaliberName4],[cBG_CaliberCode5],[cBG_CaliberName5],[cBG_CaliberCode6],[cBG_CaliberName6])
                                                         VALUES(@AutoID,@ID,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,0.00,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL);
                                                        {0}
                                                        ", getCurrentStockSql);

                        //rdrecords09_DHID
                        SqlParameter[] bodyPara = { 
                                                   new SqlParameter("@iFlag",body.iFlag),
                                                   new SqlParameter("@cInvCode",body.cInvCode),                                     
                                                   new SqlParameter("@AutoID",(body_end_Id - detailCount + i + 1)), // 从表id
                                                   new SqlParameter("@ID",RdRecord09Id),
                                                   new SqlParameter("@iQuantity",body.iQuantity),
                                                   new SqlParameter("@DHID",body.DHID),
                                                   new SqlParameter("@iUnitCost",body.iUnitCost),
                                                   new SqlParameter("@iAPrice",body.iAPrice),
                                                   new SqlParameter("@cDefine26",body.cDefine26),
                                                   new SqlParameter("@cDefine27",body.cDefine27),
                                                   new SqlParameter("@irowno",body.irowno),
                                                   new SqlParameter("@cbsysbarcode",GetDBValue(body.cbsysbarcode)),
                                                   new SqlParameter("@cwhcode",GetDBValue(RdRecord09Head.cWhCode)),
                                                   new SqlParameter("@cDefine22",GetDBValue(body.cDefine22)),
                                                   new SqlParameter("@cDefine23",GetDBValue(body.cDefine23)),   
                                                   new SqlParameter("@cBatch",GetDBValue(body.cBatch)),
                                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });

                    }

                    // 执行SQL
                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚

                    DeleteSale(RdRecord09Head.cCode);

                    throw ex;
                }
            }
            else
            {
                throw new JRException("其他出库单表头新增失败!");
            }

            return (RdRecord09Id == null ? null : RdRecord09Id.ToString());
        }

        /// <summary>
        /// 删除配送出库单
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int DeleteSale(string cCode)
        {
            if (string.IsNullOrWhiteSpace(cCode))
                return 0;

            //处理现存量数据
            var stockSql = GetCurrentStockSql("Rd09", -1);
            var head = GetEntityRdRecord09Head(cCode);
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

            string sql = "DELETE RdRecord09 WHERE cCode = @cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });
            int? RdRecord09Id = GetRdRecord09ID(cCode);
            string delBodySql = "DELETE rdrecords09 where ID = @ID";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@ID",RdRecord09Id)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }
        /// <summary>
        /// 获取配送出库单表头ID
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int? GetRdRecord09ID(string cCode)
        {
            string sql = "SELECT ID FROM RdRecord09 WHERE cCode = @cCode";
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
        /// <summary>
        /// 查询配送出库单号是否存在
        /// </summary>
        /// <param name="RdRecord09Head"></param>
        /// <returns></returns>
        public int QueryRdRecord09(string ccode)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from RdRecord09 where cCode=@cCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",ccode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }
            return num;
        }
        /// <summary>
        /// 获取其他出库单实体数据
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public EntityRdRecord09Head GetEntityRdRecord09Head(string cCode)
        {
            EntityRdRecord09Head entity = new EntityRdRecord09Head();
            List<EntityRdRecord09Body> listBody = new List<EntityRdRecord09Body>();
            string sqlHead = "SELECT cCode,dDate,cWhCode,cRdCode,cDepCode,cDefine1,cCusCode,cDefine8,cDefine10,cShipAddress,cMemo,cMaker FROM RdRecord09 WHERE cCode = @cCode ";
            string sqlBody = "SELECT cInvCode,iQuantity,cDefine26,cDefine27,cDefine22,cDefine23,cBatch FROM RdRecords09 WHERE ID = (SELECT ID FROM RdRecord09 WHERE cCode = @cCode )";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCode",cCode)
                                   };
            SqlDataReader drHead = DB_SqlHelper.ExecuteReader(CommandType.Text, sqlHead, paras);
            SqlDataReader drBody = DB_SqlHelper.ExecuteReader(CommandType.Text, sqlBody, paras);
            while (drBody.Read())
            {
                EntityRdRecord09Body body = new EntityRdRecord09Body();
                body.cInvCode = drBody["cInvCode"].ToString();
                body.iQuantity = Convert.ToDecimal(drBody["iQuantity"] ?? 0);
                body.cDefine26 = Convert.ToDecimal(drBody["cDefine26"] ?? 0);
                body.cDefine27 = Convert.ToDecimal(drBody["cDefine27"] ?? 0);
                body.cDefine22 = drBody["cDefine22"].ToString();
                body.cDefine23 = drBody["cDefine23"].ToString();
                body.cBatch = drBody["cBatch"].ToString();
                listBody.Add(body);
            }

            if (drHead.Read())
            {
                entity.cCode = cCode;
                entity.dDate = Convert.ToDateTime(drHead["dDate"]);
                entity.cWhCode = drHead["cWhCode"].ToString();
                entity.cRdCode = drHead["cRdCode"].ToString();
                entity.cDepCode = drHead["cDepCode"].ToString();
                entity.cDefine1 = drHead["cDefine1"].ToString();
                entity.cCusCode = drHead["cCusCode"].ToString();
                entity.cDefine8 = drHead["cDefine8"].ToString();
                entity.cDefine10 = drHead["cDefine10"].ToString();
                entity.cShipAddress = drHead["cShipAddress"].ToString();
                entity.cMaker = drHead["cMaker"].ToString();
                entity.cMemo = drHead["cMemo"].ToString();
                entity.Details = listBody;
            }
            return entity;
        }
    }
}