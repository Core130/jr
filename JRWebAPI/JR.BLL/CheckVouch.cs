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
    /// 盘点处理类
    /// </summary>
    public class CheckVouchBll : U8BllBase
    {
        public CheckVouchBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }

        /// <summary>
        /// 新增盘点单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string AddCheckVouch(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityCheckVouchHead entity, out int success)
        {
            success = 0;
            CheckVouchBll bll = new CheckVouchBll(StrAccID, AccYear, UserCode, PlainPassword);
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            foreach (EntityCheckVouchBody entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }
            if (bll.GetGlmendFlag(entity.dCVDate.Year, entity.dCVDate.Month, "bflag_ST") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cCVCode);
            }
            //　仓库   
            string warehouseName = GetWarehouseName(entity.cWhCode);
            if (string.IsNullOrWhiteSpace(warehouseName))
            {
                return string.Format("U8中不存在仓库编码:{0}!", entity.cWhCode);
            }
            entity.cORdCode = GetRdCode(18, 0);
            entity.cIRdCode = GetRdCode(18, 1);
            // 出库类别编码         
            if (string.IsNullOrWhiteSpace(entity.cORdCode) || string.IsNullOrWhiteSpace(GetRdStyleName(entity.cORdCode)))
            {
                return string.Format("U8中不存在收发类别编码:{0}!", entity.cORdCode);
            }
            // 入库类别编码         
            if (string.IsNullOrWhiteSpace(entity.cIRdCode) && string.IsNullOrWhiteSpace(GetRdStyleName(entity.cIRdCode, 1)))
            {
                return string.Format("U8中不存在收发类别编码:{0}!", entity.cIRdCode);
            }
            #endregion

            entity.cSysbarCode = "||st18|" + entity.cCVCode;
            //获取存货档案信息
            var inventorys = GetInventorys("'" + string.Join("','", entity.Details.Select(p => p.cInvCode).Distinct()) + "'");
            //获取库存信息
            EntityCurrentStock xclEntity = new EntityCurrentStock();
            xclEntity.cWhCode = entity.cWhCode;
            xclEntity.cInvCode = string.Join(",", entity.Details.Select(p => p.cInvCode).Distinct());
            CurrentStockBll csBll = new CurrentStockBll(StrAccID, AccYear, UserCode, PlainPassword);
            var csData = csBll.GetCurrentStock(xclEntity, out msg);

            for (int i = 0; i < entity.Details.Count; i++)
            {
                EntityCheckVouchBody body = entity.Details[i];
                #region 验证单据明细

                var invInfo = inventorys.FirstOrDefault(p => p.cInvCode == body.cInvCode);
                // 存货编号               
                if (invInfo == null)
                {
                    return string.Format("U8中不存在存货编码:{0}!", body.cInvCode);
                }

                if (!string.IsNullOrWhiteSpace(body.cBatch) && invInfo.bInvBatch == "0")
                {
                    return string.Format("U8中存货编码:{0}未启用批次管理，批次信息必须为空!", body.cInvCode);
                }
                if (string.IsNullOrWhiteSpace(body.cBatch) && invInfo.bInvBatch == "1")
                {
                    return string.Format("U8中存货编码:{0}启用批次管理，批次信息不能为空!", body.cInvCode);
                }
                // int bInvType = GetbInvType(body.cInvCode);

                if (body.iCVCQuantity < 0)
                {
                    return string.Format("盘点数量不能为小于0!");
                }


                #endregion

                #region 明细栏目计算             
                body.iCVQuantity = csData.Where(p => p.cInvCode == body.cInvCode).Sum(p => p.iQuantity);
                body.iAdInQuantity = 0;
                body.iAdOutQuantity = 0;
                body.iActualWaste = 0; //盈亏比例%：=盈亏数量/账面调整数*100

                #endregion
            }

            // 设置默认值           
            entity.cMaker = string.IsNullOrWhiteSpace(entity.cMaker) ? bll.GetUserName(UserCode) : entity.cMaker;

            string id = InsertCheckVouch(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1; ;
            return id;
        }

        /// <summary>
        /// 新增盘点单
        /// </summary>
        /// <param name="checkVouchHead"></param>
        /// <returns></returns>
        public string InsertCheckVouch(EntityCheckVouchHead checkVouchHead)
        {
            int? checkVouchHeadId;

            if (checkVouchHead == null || checkVouchHead.Details == null || checkVouchHead.Details.Count == 0)
            {
                throw new JRException("盘点单新增失败!没有盘点单数据!");
            }
            #region 验证
            int num = 0;
            string cmdText = "select count(*) as Num from CheckVouch where cCVCode=@cCVCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCVCode",checkVouchHead.cCVCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }

            if (num > 0)
            {
                throw new JRException("数据传递有误，已存在该盘点单号!");
            }

            #endregion
            // 得到主表ID和从表ID
            int detailCount = checkVouchHead.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "ch", detailCount, out head_id, out body_end_Id);

            string sql = string.Format(@"
Insert Into CheckVouch(ccvcode,dcvdate,cdepcode,cpersoncode,cirdcode,cordcode,cwhcode,ccvbatch,ccvmemo,cdefine1,cdefine2,cdefine3,cdefine4,cdefine5,
cdefine6,cdefine7,cdefine8,cdefine9,cdefine10,caccounter,cmaker,cposition,dacdate,id,vt_id,btransflag,cdefine11,cdefine12,cdefine13,cdefine14,
cdefine15,cdefine16,ccvtype,ccvperiod,csource,bposcheck,ireturncount,iverifystate,iswfcontrolled,cmodifyperson,dmodifydate,dnmaketime,dnmodifytime,
dnverifytime,dveridate,cbustype,csourcecodels,iprintcount,csysbarcode,ccurrentauditor)
Values (@ccvcode,@dcvdate,Null,Null,@cirdcode,@cordcode,@cwhcode,Null,@ccvmemo,Null,Null,Null,Null,Null,
Null,Null,Null,Null,Null,Null,@cmaker,Null,@dacdate,@id,29,0,Null,Null,Null,Null,
Null,Null,N'普通仓库盘点',Null,N'1',N'0',Null,Null,0,Null,Null,getdate(),Null ,Null ,Null,Null,Null,0,@csysbarcode,Null)
");
            SqlParameter[] para = { 
                                      new SqlParameter("@ccvcode",checkVouchHead.cCVCode),
                                      new SqlParameter("@dcvdate",checkVouchHead.dCVDate.ToShortDateString()),
                                      new SqlParameter("@cirdcode",GetDBValue(checkVouchHead.cIRdCode)),
                                      new SqlParameter("@cordcode",GetDBValue(checkVouchHead.cORdCode)),
                                      new SqlParameter("@cwhcode",checkVouchHead.cWhCode),
                                      new SqlParameter("@ccvmemo",GetDBValue(checkVouchHead.cCVMemo)),
                                      new SqlParameter("@cmaker",checkVouchHead.cMaker),
                                      new SqlParameter("@dacdate",checkVouchHead.dACDate),
                                      new SqlParameter("@id",head_id),  
                                      new SqlParameter("@csysbarcode",checkVouchHead.cSysbarCode),  
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();                    
                    checkVouchHeadId = GetCheckVouchID(checkVouchHead.cCVCode);
                    if (checkVouchHeadId == null)
                    {
                        throw new JRException("盘点单表头新增失败!");
                    }

                    // 组装SQL语句
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntityCheckVouchBody body = checkVouchHead.Details[i];

                        string bodySql = string.Format(@"
Insert Into CheckVouchs(ccvcode,cinvcode,rdsid,icvnum,icvquantity,icvcnum,icvcquantity,ccvbatch,cfree1,cfree2,ccvreason,ddisdate,ijhdj,ijhje,isjdj,
isjje,cposition,cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,citemcode,citem_class,cname,citemcname,autoid,id,cbarcode,iadinquantity,
iadinnum,iadoutquantity,iadoutnum,iallowwaste,iactualwaste,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cdefine28,cdefine29,cdefine30,
cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,cassunit,cbvencode,cinvouchcode,imassdate,dmadedate,cmassunit,isotype,isodid,
cvmivencode,iinvexchrate,iexpiratdatecalcu,cexpirationdate,dexpirationdate,cbatchproperty1,cbatchproperty2,cbatchproperty3,cbatchproperty4,
cbatchproperty5,cbatchproperty6,cbatchproperty7,cbatchproperty8,cbatchproperty9,cbatchproperty10,cciqbookcode,cbmemo,cwhpersoncode,cwhpersonname,
irowno,cinvouchtype,strowguid,cbsysbarcode,bneedrecheck,recheckstatus,checkcode,checkautoid)
Values (@ccvcode,@cinvcode,Null,0,@icvquantity,0,@icvcquantity,@ccvbatch,Null,Null,@ccvreason,Null,Null,Null,Null,
Null,Null,Null,Null,Null,Null,@cdefine26,NULL,Null,Null,Null,Null,@autoid,@id,Null,@iadinquantity,
0,@iadoutquantity,0,Null,@iactualwaste,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,
Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,
Null,Null,0,Null,Null,Null,Null,Null,Null,
Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,
@irowno,Null,Null,@cbsysbarcode,Null,Null,Null,Null)
");
                        SqlParameter[] bodyPara = { 
                                      new SqlParameter("@ccvcode",checkVouchHead.cCVCode),
                                      new SqlParameter("@cinvcode",body.cInvCode),
                                      new SqlParameter("@icvquantity",body.iCVQuantity),
                                      new SqlParameter("@cdefine26",body.cDefine26),
                                      new SqlParameter("@icvcquantity",body.iCVCQuantity),
                                      new SqlParameter("@ccvreason",GetDBValue(body.cCVReason)),
                                      new SqlParameter("@autoid",(body_end_Id - detailCount + i + 1)),
                                      new SqlParameter("@id",checkVouchHeadId),                                
                                      new SqlParameter("@iadinquantity",body.iAdInQuantity),
                                      new SqlParameter("@iadoutquantity",body.iAdOutQuantity),
                                      new SqlParameter("@iactualwaste",body.iActualWaste),
                                      new SqlParameter("@irowno",i+1),                                      
                                      new SqlParameter("@cbsysbarcode",checkVouchHead.cSysbarCode + "|"+ (i+1)),  
                                      new SqlParameter("@ccvbatch",GetDBValue(body.cBatch)),                                      
                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });
                        
                    }


                    // 执行SQL
                    this.DB_SqlHelper.ExecuteNonQuery(sqlList);                    
                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚
                    DeleteCheckVouch(checkVouchHead.cCVCode);

                    throw ex;
                }
            }
            else
            {
                throw new JRException("盘点单表头新增失败!");
            }

            return (checkVouchHeadId == null ? null : checkVouchHeadId.ToString());
        }

        /// <summary>
        /// 删除盘点单
        /// </summary>
        /// <param name="cCVCode"></param>
        /// <returns></returns>
        public int DeleteCheckVouch(string cCVCode)
        {
            if (string.IsNullOrWhiteSpace(cCVCode))
                return 0;

            List<ExecuteHelp> list = new List<ExecuteHelp>();

            string sql = "DELETE CheckVouch WHERE cCVCode = @cCVCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCVCode",cCVCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });

            string delBodySql = "DELETE CheckVouchs where cCVCode = @cCVCode";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@cCVCode",cCVCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }

        /// <summary>
        /// 获取盘点单表头ID
        /// </summary>
        /// <param name="cCVCode"></param>
        /// <returns></returns>
        public int? GetCheckVouchID(string cCVCode)
        {
            string sql = "SELECT ID FROM CheckVouch WHERE cCVCode = @cCVCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCVCode",cCVCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["ID"].ToString());
            }

            return null;
        }


        /// <summary>
        /// 查询盘点单号是否存在
        /// </summary>
        /// <param name="cCVCode"></param>
        /// <returns></returns>
        public int QueryCheckVouch(string cCVCode)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from CheckVouch where cCVCode = @cCVCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCVCode",cCVCode)
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
