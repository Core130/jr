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
    /// 调拨申请单处理类
    /// </summary>
    public class ST_AppTransVouchBll : U8BllBase
    {
        public ST_AppTransVouchBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }

        public string AddST_AppTransVouch(string UserCode, string PlainPassword,
                                   string StrAccID, int AccYear, string Act, EntityST_AppTransVouch entity, out int success)
        {
            success = 0;
            ST_AppTransVouchBll bll = new ST_AppTransVouchBll(StrAccID, AccYear, UserCode, PlainPassword);
            #region 验证
            if (entity == null || entity.Details == null || entity.Details.Count == 0)
            {
                throw new JRException("调拨申请单新增失败!没有数据!");
            }

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            foreach (EntityST_AppTransVouchBody entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }

            if (bll.GetGlmendFlag(entity.dTvDate.Year, entity.dTvDate.Month, "bflag_ST") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cTvCode);
            }
            //　调出仓库   
            if (string.IsNullOrWhiteSpace(GetWarehouseName(entity.cOutWhCode)))
            {
                return string.Format("U8中不存在仓库编码:{0}!", entity.cOutWhCode);
            }

            //　调入仓库   
            if (string.IsNullOrWhiteSpace(GetWarehouseName(entity.cInWhCode)))
            {
                return string.Format("U8中不存在仓库编码:{0}!", entity.cInWhCode);
            }

            //调出部门            
            if (string.IsNullOrWhiteSpace(GetDepartmentName(entity.cOutDepCode)))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cOutDepCode);
            }
            //调入部门            
            if (string.IsNullOrWhiteSpace(GetDepartmentName(entity.cInDepCode)))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cInDepCode);
            }

            entity.cOutRdCode = GetRdCode(62, 0);
            entity.cInRdCode = GetRdCode(62, 1);
            // 出库类别编码         
            if (string.IsNullOrWhiteSpace(entity.cOutRdCode) || string.IsNullOrWhiteSpace(GetRdStyleName(entity.cOutRdCode)))
            {
                return string.Format("U8中不存在收发类别编码:{0}!", entity.cOutRdCode);
            }

            // 入库类别编码         
            if (string.IsNullOrWhiteSpace(entity.cInRdCode) || string.IsNullOrWhiteSpace(GetRdStyleName(entity.cInRdCode,1)))
            {
                return string.Format("U8中不存在收发类别编码:{0}!", entity.cInRdCode);
            }

            // 客户     
            string cusName = GetCustomerName(entity.cCusCode);
            if (string.IsNullOrWhiteSpace(cusName))
            {
                return string.Format("U8中不存在客户编号:{0}!", entity.cCusCode);
            }

            int num = 0;
            string cmdText = "select count(*) as Num from ST_AppTransVouch where ctvcode=@ctvcode";
            SqlParameter[] paras = {
                                       new SqlParameter("@ctvcode",entity.cTvCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }

            if (num > 0)
            {
                throw new JRException("数据传递有误，ERP中已存在该调拨申请单号!");
            }

            var inventorys = GetInventorys("'" + string.Join("','", entity.Details.Select(p => p.cInvCode).Distinct()) + "'");

            for (int i = 0; i < entity.Details.Count; i++)
            {
                EntityST_AppTransVouchBody body = entity.Details[i];
                var invInfo = inventorys.FirstOrDefault(p => p.cInvCode == body.cInvCode);
                // 存货编号                
                if (invInfo == null)
                {
                    return string.Format("U8中不存在存货编码:{0}!", body.cInvCode);
                }

                if (body.iTvQuantity == 0)
                {
                    return string.Format("申请数量不能为0!");
                }

                if (body.iUnitCost * body.iTvQuantity != body.iPrice || body.iTvQuantity * body.cDefine26 != body.cDefine27)
                {
                    return string.Format("存货编码:{0}金额异常!", body.cInvCode);
                }
            }
            #endregion

            entity.cMaker = string.IsNullOrWhiteSpace(entity.cMaker) ? GetUserName(UserCode) : entity.cMaker;

            string id = InsertAppTransVouch(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }

        public string InsertAppTransVouch(EntityST_AppTransVouch Head)
        {

            // 得到主表ID和从表ID
            int detailCount = Head.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "at", detailCount, out head_id, out body_end_Id);
            string cSysBarCode = "||st62|" + Head.cTvCode;
            string sql = string.Format(@"
insert into ST_AppTransVouch(ctvcode,dtvdate,cowhcode,ciwhcode,codepcode,cidepcode,cpersoncode,cirdcode,cordcode,ctvmemo,cdefine1,cdefine2,cdefine3,cdefine4,
cdefine5,cdefine6,cdefine7,cdefine8,cdefine9,cdefine10,cmaker,id,vt_id,cverifyperson,dverifydate,cdefine11,cdefine12,cdefine13,cdefine14,cdefine15,cdefine16,
ccloser,csource,ireturncount,iverifystate,iswfcontrolled,cmodifyperson,dmodifydate,dnmaketime,dnmodifytime,dnverifytime,capprovar,dapprovedate,cbustype,
bislsquery,csourcecodels,iprintcount,csourceguid,csysbarcode,ccurrentauditor)
VALUES(@ctvcode,@dtvdate,@cowhcode,@ciwhcode,@codepcode,@cidepcode,Null,@cirdcode,@cordcode,@ctvmemo,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,
@cmaker,@id,37,Null,Null,@cdefine11,Null,Null,Null,Null,Null,Null,Null,Null,Null,0,Null,Null,getdate(),Null ,Null ,Null,Null,Null,Null,Null,0,Null,@csysbarcode,Null)
");
            SqlParameter[] para = { 
                                      new SqlParameter("@ctvcode",Head.cTvCode),
                                      new SqlParameter("@dtvdate",Head.dTvDate.ToShortDateString()),
                                      new SqlParameter("@cowhcode",Head.cOutWhCode),
                                      new SqlParameter("@ciwhcode",Head.cInWhCode),
                                      new SqlParameter("@codepcode",Head.cOutDepCode),
                                      new SqlParameter("@cidepcode",Head.cInDepCode),
                                      new SqlParameter("@cirdcode",GetDBValue(Head.cInRdCode)),
                                      new SqlParameter("@cordcode",GetDBValue(Head.cOutRdCode)),
                                      new SqlParameter("@ctvmemo",GetDBValue(Head.cTvMemo)),
                                      new SqlParameter("@cmaker",Head.cMaker),
                                      new SqlParameter("@id",head_id),
                                      new SqlParameter("@csysbarcode",cSysBarCode),
                                      new SqlParameter("@cdefine11",Head.cCusCode),
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();

                    // 组装SQL语句
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntityST_AppTransVouchBody body = Head.Details[i];

                        string bodySql = string.Format(@"
Insert Into ST_AppTransVouchs(ctvcode,cinvcode,itvnum,itvquantity,itvchknum,itvchkquantity,itvsumnum,itvsumquantity,itvpcost,itvpprice,ctvbatch,ddisdate,citemcode,
citem_class,iunitcost,iprice,fsalecost,fsaleprice,cname,citemcname,autoid,id,imassdate,dappdate,cbarcode,cassunit,cbvencode,cinvouchcode,dmadedate,cmassunit,cbcloser,
cfree1,cfree2,cfree3,cfree4,cfree5,cfree6,cfree7,cfree8,cfree9,cfree10,cdefine22,cdefine23,cdefine24,cdefine25,cdefine26,cdefine27,cdefine28,cdefine29,cdefine30,
cdefine31,cdefine32,cdefine33,cdefine34,cdefine35,cdefine36,cdefine37,iinvexchrate,iexpiratdatecalcu,cexpirationdate,dexpirationdate,cbmemo,irowno,cbsysbarcode)
Values (@ctvcode,@cinvcode,Null,@itvnum,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,@iunitcost,@iprice,0,0,Null,Null,@autoid,@id,Null,Null,Null,Null,
Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,Null,@cdefine26,@cdefine27,Null,Null,Null,Null,Null,
Null,Null,Null,Null,Null,Null,0,Null,Null,Null,@irowno,@cbsysbarcode)
");
                        SqlParameter[] bodyPara = { 
                                      new SqlParameter("@ctvcode",Head.cTvCode),
                                      new SqlParameter("@cinvcode",body.cInvCode),
                                      new SqlParameter("@itvnum",body.iTvQuantity),
                                      new SqlParameter("@iunitcost",body.iUnitCost),
                                      new SqlParameter("@iprice",body.iPrice),
                                      new SqlParameter("@autoid",body_end_Id - detailCount + i + 1),
                                      new SqlParameter("@id",head_id),
                                      new SqlParameter("@cdefine26",body.cDefine26),
                                      new SqlParameter("@cdefine27",body.cDefine27),
                                      new SqlParameter("@irowno",i+1),
                                      new SqlParameter("@cbsysbarcode",cSysBarCode + "|"+ (i + 1)),
                                                   };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });
                    }
                    // 执行SQL
                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);
                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚
                    DeleteTransVouch(Head.cTvCode);

                    throw ex;
                }

            }
            else
            {
                throw new JRException("调拨申请单新增失败!");
            }

            return (head_id == 0 ? null : head_id.ToString());           

        }


        /// <summary>
        /// 删除调拨申请单
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int DeleteTransVouch(string cTvCode)
        {
            if (string.IsNullOrWhiteSpace(cTvCode))
                return 0;

            List<ExecuteHelp> list = new List<ExecuteHelp>();

            string sql = "DELETE ST_AppTransVouch WHERE cTvCode = @cTvCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cTvCode",cTvCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });

            string delBodySql = "DELETE ST_AppTransVouchs where cTvCode = @cTvCode";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@cTvCode",cTvCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }
    }
}
