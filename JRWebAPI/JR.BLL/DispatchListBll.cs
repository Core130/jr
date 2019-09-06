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
    public class DispatchListBll : U8BllBase
    {
        public DispatchListBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        /// <summary>
        /// 新增退货单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="Kind">0：普通销售 1：委托代销</param>
        /// <param name="entity"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public string AddDispatchList(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, int Kind, EntityDispatchListHead entity, out int success)
        {
            success = 0;

            DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, PlainPassword);

            // 必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }
            foreach (EntityDispatchListBody entitybody in entity.Details)
            {
                if (!entity.CheckEntity(out msg))
                {
                    return msg;
                }
            }
            #region 验证

            if (bll.GetGlmendFlag(entity.dDate.Year, entity.dDate.Month, "bflag_SA") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cDLCode);
            }

            // 客户编号  
            string cusName = bll.GetCustomerName(entity.cCusCode);

            if (string.IsNullOrWhiteSpace(cusName))
            {
                return string.Format("U8中不存在客户编号:{0}!", entity.cCusCode);
            }
            entity.cCusName = cusName;

            // 部门 
            string deptName = bll.GetDepartmentName(entity.cDepCode);
            if (string.IsNullOrWhiteSpace(deptName))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cDepCode);
            }
            //销售类型
            if (string.IsNullOrWhiteSpace(GetStName(entity.cSTCode)))
            {
                return string.Format("U8中不存在销售类型编码:{0}!", entity.cSTCode);
            }

            if (GetDispatchList(entity.cDLCode) != 0)
            {
                return string.Format("U8中已存在该退货单号{0}!", entity.cDLCode);
            }

            #endregion

            if (entity.Details == null || entity.Details.Count == 0)
            {
                return "表体中存货信息不能为空，至少要有一条记录!";
            }

            var inventorys = GetInventorys("'" + string.Join("','", entity.Details.Select(p => p.cInvCode).Distinct()) + "'");

            for (int i = 0; i < entity.Details.Count; i++)
            {
                EntityDispatchListBody body = entity.Details[i];

                #region 验证单据明细
                var invInfo = inventorys.FirstOrDefault(p => p.cInvCode == body.cInvCode);
                // 存货编号 
                string invName = bll.GetInventoryName(body.cInvCode);
                if (invInfo == null)
                {
                    return string.Format("U8中不存在存货编码:{0}!", body.cInvCode);
                }

                if (body.iQuantity > 0)
                {
                    return "退货数量不能大于0!";
                }

                if (body.iQuantity != 0 && body.iSum > 0)
                {
                    return "退货单价税合计不能大于0";
                }
                if (!string.IsNullOrWhiteSpace(body.cBatch) && bll.GetInventoryName(body.cInvCode, 4) == "0")
                {
                    return string.Format("U8中存货编码:{0}未启用批次管理，批次信息必须为空!", body.cInvCode);
                }
                if (string.IsNullOrWhiteSpace(body.cBatch) && bll.GetInventoryName(body.cInvCode, 4) == "1")
                {
                    body.cBatch = bll.GetInvBatch(body.cWhCode, body.cInvCode);
                    if (string.IsNullOrWhiteSpace(body.cBatch))
                        return string.Format("U8中存货编码:{0}启用批次管理，批次信息不能为空!", body.cInvCode);
                }
                body.cInvName = invName;
                int bInvType = GetbInvType(body.cInvCode);
                if (body.iQuantity == 0 && bInvType == 0)
                {
                    return string.Format("非折扣属性商品发货数量不能等于0");
                }

                #endregion

                #region 单据明细计算
                body.bcosting = 1;//标志 是否记账
                if (bInvType == 1) { body.cWhCode = null; body.bcosting = 0; body.iQuantity = 0; body.iQuotedPrice = 0; }
                body.iRowNo = (i + 1);
                body.KL = 100;
                //当数量为正数时，求其绝对值转换为负数
                if (body.iQuantity > 0)
                {
                    body.iQuantity = -body.iQuantity;
                }
                body.bSettleAll = 0;//结算标志
                body.bQAChecked = 0;//是否报险
                body.bQAUrgency = 0;//是否急料
                body.bQAChecking = 0;//是否再检
                body.bQAChecked = 0;//是否报险
                body.bQANeedCheck = 0; //是否质检
                decimal isum = body.iSum.HasValue ? body.iSum.Value : 0;
                body.cbSysBarCode = string.Format("||SA03|{0}|{1}", entity.cDLCode, body.iRowNo);
                body.iTaxUnitPrice = body.iQuantity == 0 ? 0 : isum / body.iQuantity;
                body.iUnitPrice = body.iQuantity == 0 ? 0 : Math.Round(body.iTaxUnitPrice / ((body.iTaxRate / 100) + 1), 2);
                body.iMoney = Math.Round(isum / ((body.iTaxRate / 100) + 1), 2);
                body.iTax = isum - body.iMoney;
                body.KL2 = body.iQuotedPrice == 0 || !body.iQuotedPrice.HasValue ? 100 : (isum / (body.iQuotedPrice.Value * body.iQuantity)) * 100;
                body.fSaleCost = body.fSaleCost != 0 ? body.fSaleCost : bll.GetSA_InvPrice(body.cInvCode);
                body.fSalePrice = body.fSaleCost * body.iQuantity;
                body.iDisCount = body.iQuotedPrice == 0 || !body.iQuotedPrice.HasValue ? 0 : body.iQuotedPrice.Value * body.iQuantity - isum;
                body.iNatMoney = body.iMoney;
                body.iNatSum = isum;
                body.iNatTax = body.iTax;
                body.iNatUnitPrice = body.iUnitPrice;
                body.iNatDisCount = body.iDisCount;

                #endregion


            }
            //设置默认值
            entity.bFirst = 0;//销售期初标志
            entity.bReturnFlag = 1;//退货标志
            entity.bSettleAll = 0;//结算标志
            entity.iExchRate = 1;//汇率
            entity.cBusType = Kind == 0 ? "普通销售" : "委托代销";  // 业务类型
            entity.cexch_name = "人民币";//币种名称
            entity.cVouchType = Kind == 0 ? "05" : "06";//单据类型编码
            entity.iVTid = Kind == 0 ? 75 : 51;//单据模板号
            entity.cSysBarCode = Kind == 0 ? string.Format("||SA03|{0}", entity.cDLCode) : string.Format("||SA54|{0}", entity.cDLCode);
            entity.cMaker = bll.GetUserName(UserCode);

            string id = string.Empty;
            id = bll.InsertDispatchList(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }
        /// <summary>
        /// 新增发货单
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="ModelType"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="Kind">0：普通销售 1：委托代销</param>
        /// <param name="entity"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public string AddDispatchList(string UserCode, string PlainPassword, int ModelType,
                                    string StrAccID, int AccYear, string Act, int Kind, EntityDispatchListHead entity, out int success)
        {
            success = 0;
            DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, PlainPassword);
            entity.bFirst = 0;//销售期初标志
            entity.bReturnFlag = 0;//退货标志
            entity.bSettleAll = 0;//结算标志
            entity.iExchRate = 1;//汇率
            entity.cBusType = Kind == 0 ? "普通销售" : "委托代销";  // 业务类型
            entity.cexch_name = "人民币";//币种名称
            entity.cVouchType = Kind == 0 ? "05" : "06";//单据类型编码
            entity.iVTid = Kind == 0 ? 71 : 79;//单据模板号
            entity.cSysBarCode = Kind == 0 ? string.Format("||SA01|{0}", entity.cDLCode) : string.Format("||SA53|{0}", entity.cDLCode);
            entity.cMaker = bll.GetUserName(UserCode);

            #region 验证

            if (bll.GetGlmendFlag(entity.dDate.Year, entity.dDate.Month, "bflag_SA") == true)
            {
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cDLCode);
            }

            // 客户编号  
            string cusName = bll.GetCustomerName(entity.cCusCode);

            if (string.IsNullOrWhiteSpace(cusName))
            {
                return string.Format("U8中不存在客户编号:{0}!", entity.cCusCode);
            }
            entity.cCusName = cusName;

            // 部门 
            string deptName = bll.GetDepartmentName(entity.cDepCode);
            if (string.IsNullOrWhiteSpace(deptName))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cDepCode);
            }
            //销售类型
            if (string.IsNullOrWhiteSpace(GetStName(entity.cSTCode)))
            {
                return string.Format("U8中不存在销售类型编码:{0}!", entity.cSTCode);
            }
            if (GetDispatchList(entity.cDLCode) != 0)
            {
                return string.Format("U8中已存在该发货单号{0}!", entity.cDLCode);
            }

            #endregion

            if (entity.Details == null || entity.Details.Count == 0)
            {
                return "表体中存货信息不能为空，至少要有一条记录!";
            }


            // 必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }
            foreach (EntityDispatchListBody entitybody in entity.Details)
            {
                if (!entitybody.CheckEntity(out msg))
                {
                    return msg;
                }
            }
            //获取存货档案信息
            var inventorys = GetInventorys("'" + string.Join("','", entity.Details.Select(p => p.cInvCode).Distinct()) + "'");
            //获取库存信息
            EntityCurrentStock xclEntity = new EntityCurrentStock();
            xclEntity.cWhCode = string.Join(",", entity.Details.Select(p => p.cWhCode).Distinct());
            xclEntity.cInvCode = string.Join(",", entity.Details.Select(p => p.cInvCode).Distinct());
            CurrentStockBll csBll = new CurrentStockBll(StrAccID, AccYear, UserCode, PlainPassword);
            var csData = csBll.GetCurrentStock(xclEntity, out msg);

            for (int i = 0; i < entity.Details.Count; i++)
            {
                EntityDispatchListBody body = entity.Details[i];
                var invInfo = inventorys.FirstOrDefault(p => p.cInvCode == body.cInvCode);

                #region 验证

                // 存货编号
                if (invInfo == null)
                {
                    return string.Format("U8中不存在存货编码:{0}!", body.cInvCode);
                }

                if (body.iQuantity < 0)
                {
                    return "发货数量不能小于0!";
                }

                if (body.iQuantity != 0 && body.iSum < 0)  //折扣类存货数量为0，金额为负数
                {
                    return "发货单价税合计不能小于0!";
                }
                if (!string.IsNullOrWhiteSpace(body.cBatch) && invInfo.bInvBatch == "0")
                {
                    return string.Format("U8中存货编码:{0}未启用批次管理，批次信息必须为空!", body.cInvCode);
                }
                if (string.IsNullOrWhiteSpace(body.cBatch) && invInfo.bInvBatch == "1")
                {
                    body.cBatch = bll.GetInvBatch(body.cWhCode, body.cInvCode);
                    if (string.IsNullOrWhiteSpace(body.cBatch))
                        return string.Format("U8中存货编码:{0}启用批次管理，批次信息不能为空!", body.cInvCode);
                }
                body.cInvName = invInfo.cInvName;
                int bInvType = Convert.ToInt32(invInfo.bInvType);
                if (body.iQuantity == 0 && invInfo.bInvType == "0")
                {
                    return string.Format("非折扣属性商品发货数量不能等于0");
                }
                #endregion

                #region 计算
                body.bcosting = 1;//标志 是否记账
                if (bInvType == 1) { body.cWhCode = null; body.bcosting = 0; body.iQuantity = 0; body.iQuotedPrice = 0; }
                body.iRowNo = (i + 1);
                body.KL = 100;

                body.bSettleAll = 0;//结算标志
                body.bQAChecked = 0;//是否报险
                body.bQAUrgency = 0;//是否急料
                body.bQAChecking = 0;//是否再检
                body.bQAChecked = 0;//是否报险
                body.bQANeedCheck = 0; //是否质检
                decimal isum = body.iSum.HasValue ? body.iSum.Value : 0;
                body.cbSysBarCode = string.Format("||SA01|{0}|{1}", entity.cDLCode, body.iRowNo);
                body.iTaxUnitPrice = body.iQuantity == 0 ? 0 : isum / body.iQuantity;
                body.iUnitPrice = body.iQuantity == 0 ? 0 : Math.Round(body.iTaxUnitPrice / ((body.iTaxRate / 100) + 1), 2);
                body.iMoney = Math.Round(isum / ((body.iTaxRate / 100) + 1), 2);
                body.iTax = isum - body.iMoney;
                body.KL2 = body.iQuotedPrice == 0 || !body.iQuotedPrice.HasValue ? 100 : (isum / (body.iQuotedPrice.Value * body.iQuantity)) * 100;
                body.fSaleCost = body.fSaleCost != 0 ? body.fSaleCost : bll.GetSA_InvPrice(body.cInvCode);
                body.fSalePrice = body.fSaleCost * body.iQuantity;
                body.iDisCount = body.iQuotedPrice == 0 || !body.iQuotedPrice.HasValue ? 0 : body.iQuotedPrice.Value * body.iQuantity - isum;
                body.iNatMoney = body.iMoney;
                body.iNatSum = isum;
                body.iNatTax = body.iTax;
                body.iNatUnitPrice = body.iUnitPrice;
                body.iNatDisCount = body.iDisCount;
                #endregion
            }
            string id = string.Empty;
            if (!string.IsNullOrEmpty(entity.cSOCode))
            {
                int count = 0;
                count = GetSoMain(entity.cSOCode);
                if (count <= 0)
                {
                    return string.Format("销售订单号{0}在U8中不存在!", entity.cSOCode);
                }
            }

            id = bll.InsertDispatchList(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }

        public string InsertDispatchList(EntityDispatchListHead DispatchListHead)
        {
            int? DispatchListHeadID;

            #region 验证
            if (DispatchListHead == null || DispatchListHead.Details == null || DispatchListHead.Details.Count == 0)
            {
                throw new JRException("新增失败!没有数据!");
            }

            CreateDHColumn("DispatchLists");

            #endregion

            // 得到主表ID和从表ID
            int detailCount = DispatchListHead.Details.Count;
            int head_id = 0;
            int body_end_Id = 0;
            GetId("00", "DISPATCH", detailCount, out head_id, out body_end_Id);

            string sql = string.Format(@"
                                insert into DispatchList(DLID,cVouchType,cSTCode,dDate,cDepCode,cexch_name,
                                iExchRate,bFirst,bReturnFlag,bSettleAll,iVTid,cCusCode,cBusType,cDLCode,cSOCode,
                                cDefine1,cMaker,cCusName,cSysBarCode,
                                iNetLock,ioutgolden,bCredit,ireturncount,bsigncreate,
                                bcashsale,bmustbook,bneedbill,baccswitchflag,bsaleoutcreatebill,iTaxRate,cDefine3,cMemo,cinvoicecompany,cDefine8,ccusperson,cShipAddress,cDefine14
                                )VALUES(@DLID,@cVouchType,@cSTCode,@dDate,@cDepCode,@cexch_name,
                                @iExchRate,@bFirst,@bReturnFlag,@bSettleAll,@iVTid,@cCusCode,@cBusType,@cDLCode,@cSOCode,
                                @cDefine1,@cMaker,@cCusName,@cSysBarCode,
                                null,null,0,null,0,
                                0,0,1,0,0,@iTaxRate,@cDefine3,left(@cMemo,255),@cCusCode,@cDefine8,@ccusperson,@cShipAddress,@cDefine14
                                )");
            SqlParameter[] para = {                               
                                      new SqlParameter("@cVouchType",GetDBValue(DispatchListHead.cVouchType)),
                                      new SqlParameter("@cSTCode",GetDBValue(DispatchListHead.cSTCode)),
                                      new SqlParameter("@dDate",DispatchListHead.dDate.ToShortDateString()),
                                      new SqlParameter("@cDepCode",DispatchListHead.cDepCode),
                                      new SqlParameter("@cexch_name",GetDBValue(DispatchListHead.cexch_name)),
                                      new SqlParameter("@iExchRate",DispatchListHead.iExchRate),
                                      new SqlParameter("@bFirst",DispatchListHead.bFirst),
                                      new SqlParameter("@bReturnFlag",DispatchListHead.bReturnFlag),
                                      new SqlParameter("@bSettleAll",DispatchListHead.bSettleAll),
                                      new SqlParameter("@iVTid",DispatchListHead.iVTid),
                                      new SqlParameter("@cCusCode",DispatchListHead.cCusCode),
                                      new SqlParameter("@cBusType",GetDBValue(DispatchListHead.cBusType)),
                                      new SqlParameter("@cDLCode",DispatchListHead.cDLCode),
                                      new SqlParameter("@cSOCode",GetDBValue(DispatchListHead.cSOCode)),
                                      new SqlParameter("@cDefine1",GetDBValue(DispatchListHead.cDefine1)),
                                      new SqlParameter("@cMaker",GetDBValue(DispatchListHead.cMaker)),
                                      new SqlParameter("@cCusName",GetDBValue(DispatchListHead.cCusName)),
                                      new SqlParameter("@cSysBarCode",GetDBValue(DispatchListHead.cSysBarCode)),
                                      new SqlParameter("@DLID",head_id), 
                                      new SqlParameter("@iTaxRate",DispatchListHead.Details[0].iTaxRate),
                                      new SqlParameter("@cDefine3",GetDBValue(DispatchListHead.cDefine3)),//计入任务
                                      new SqlParameter("@cMemo",GetDBValue(DispatchListHead.cMemo)),
                                      new SqlParameter("@cDefine8",GetDBValue(DispatchListHead.cDefine8)),
                                      new SqlParameter("@ccusperson",GetDBValue(DispatchListHead.ccusperson)),
                                      new SqlParameter("@cShipAddress",GetDBValue(DispatchListHead.cShipAddress)),
                                      new SqlParameter("@cDefine14",GetDBValue(DispatchListHead.cDefine14)),
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            if (headCount > 0)
            {
                try
                {
                    List<ExecuteHelp> sqlList = new List<ExecuteHelp>();

                    DispatchListHeadID = GetDispatchListID(DispatchListHead.cDLCode);
                    if (DispatchListHeadID == null)
                    {
                        throw new JRException("表头新增失败!");
                    }

                    string getCurrentStockSql = GetCurrentStockSql("Dis");
                    // 组装SQL语句
                    for (int i = 0; i < detailCount; i++)
                    {
                        EntityDispatchListBody body = DispatchListHead.Details[i];

                        string bodySql = string.Format(@"
                                    insert into DispatchLists(bcosting,DLID,cInvCode,bSettleAll,iDLsID,bQANeedCheck,bQAUrgency,bQAChecking,bQAChecked,
                                    cWhCode,iQuantity,iUnitPrice,
                                    iSum,iTaxRate,iTaxUnitPrice,iTax,iNatUnitPrice,iNatMoney,
                                    iNatTax,iNatSum,iNatDisCount,iDisCount,
                                    iQuotedPrice,cInvName,iMoney,KL,KL2,fSaleCost,fSalePrice,irowno,cbSysBarCode,
                                    iCorID,iNum,iSettleNum,iSettleQuantity,iBatch,TBQuantity,iSOsID,fOutQuantity,
                                    fOutNum,fEnSettleQuan,fEnSettleSum,iSettlePrice,cMassUnit,iQAQuantity,iQANum,fsumsignquantity,
                                    fsumsignnum,fcusminprice,iExpiratDateCalcu,bneedsign,bsignover,bneedloss,
                                    frlossqty,bsaleprice,bgift,bmpforderclosed,bIAcreatebill,DHID,cSoCode,cBatch,cDefine28
                                    )
                                    VALUES(@bcosting,@DLID,@cInvCode,@bSettleAll,@iDLsID,@bQANeedCheck,@bQAUrgency,@bQAChecking,@bQAChecked,
                                    @cWhCode,@iQuantity,@iUnitPrice,
                                    @iSum,@iTaxRate,@iTaxUnitPrice,@iTax,@iNatUnitPrice,@iNatMoney,
                                    @iNatTax,@iNatSum,@iNatDisCount,@iDisCount,
                                    @iQuotedPrice,@cInvName,@iMoney,@KL,@KL2,@fSaleCost,@fSalePrice,@irowno,@cbSysBarCode,
                                    null,null,null,null,null,0,null,null,
                                    null,null,null,0,0,0,0,null,
                                    null,0,0,0,0,0,
                                    0,1,0,0,0,@DHID,@cSOCode,@cBatch,@cDefine28
                                    );
                                    {0}
                                    IF (@bReturnFlag=0 AND ISNULL(@cSOCode,'') <>'')
                                    BEGIN
                                    UPDATE  SO_SODetails SET iFHQuantity=isnull(iFHQuantity,0)+@iQuantity,iFHMoney=isnull(iFHMoney,0)+@iSum  WHERE cSOCode=@cSOCode and cInvCode=@cInvCode and DHID=@DHID
                                    UPDATE  a SET  a.iSOsID=b.iSOsID FROM   DispatchLists a inner join SO_SODetails b on a.cSOCode=b.cSOCode and a.cInvCode=b.cInvCode and a.DHID=b.DHID WHERE  a.cSOCode=@cSOCode
                                    END
                                    ", getCurrentStockSql);

                        SqlParameter[] bodyPara = { 
                                      new SqlParameter("@bcosting",body.bcosting),
                                      new SqlParameter("@DLID",DispatchListHeadID),
                                      new SqlParameter("@cInvCode",body.cInvCode),
                                      new SqlParameter("@bSettleAll",body.bSettleAll),
                                      new SqlParameter("@iDLsID",(body_end_Id - detailCount + i + 1)),
                                      new SqlParameter("@bQANeedCheck",body.bQANeedCheck),
                                      new SqlParameter("@bQAUrgency",body.bQAUrgency),
                                      new SqlParameter("@bQAChecking",body.bQAChecking),
                                      new SqlParameter("@bQAChecked",body.bQAChecked),
                                      new SqlParameter("@cWhCode",GetDBValue(body.cWhCode)),
                                      new SqlParameter("@iQuantity",body.iQuantity),
                                      new SqlParameter("@iUnitPrice",body.iUnitPrice),
                                      new SqlParameter("@DHID",GetDBValue(body.DHID)),
                                      new SqlParameter("@iSum",ConvertToDecimal(body.iSum)),
                                      new SqlParameter("@iTaxRate",body.iTaxRate),                                      
                                      new SqlParameter("@iTaxUnitPrice",body.iTaxUnitPrice),
                                      new SqlParameter("@iTax",body.iTax),
                                      new SqlParameter("@iNatUnitPrice",body.iNatUnitPrice),
                                      new SqlParameter("@iNatMoney",body.iNatMoney),
                                      new SqlParameter("@iNatTax",body.iNatTax),
                                      new SqlParameter("@iNatSum",body.iNatSum),
                                      new SqlParameter("@iNatDisCount",body.iNatDisCount),
                                      new SqlParameter("@iDisCount",body.iDisCount),
                                      new SqlParameter("@iQuotedPrice",ConvertToDecimal(body.iQuotedPrice)),
                                      new SqlParameter("@cInvName",GetDBValue(body.cInvName)),
                                      new SqlParameter("@KL",body.KL),
                                      new SqlParameter("@KL2",body.KL2),
                                      new SqlParameter("@iMoney",body.iMoney),
                                      new SqlParameter("@fSaleCost",body.fSaleCost),
                                      new SqlParameter("@fSalePrice",body.fSalePrice),
                                      new SqlParameter("@irowno",body.iRowNo),
                                      new SqlParameter("@cbSysBarCode",body.cbSysBarCode),
                                      new SqlParameter("@bReturnFlag",DispatchListHead.bReturnFlag),
                                      new SqlParameter("@cSOCode",GetDBValue(DispatchListHead.cSOCode)),
                                      new SqlParameter("@cBatch",GetDBValue(body.cBatch)),
                                      new SqlParameter("@cDefine28",GetDBValue(body.cDefine28))
                                  };
                        sqlList.Add(new ExecuteHelp() { SQL = bodySql, Parameters = bodyPara });

                    }
                    // 执行SQL
                    int bodyCount = this.DB_SqlHelper.ExecuteNonQuery(sqlList);

                }
                catch (Exception ex)
                {
                    // 表体执行错误，表头也要回滚
                    DeleteDispatchList(DispatchListHead.cDLCode);

                    throw ex;
                }
            }
            else
            {
                throw new JRException("表头新增失败!");
            }

            return (DispatchListHeadID == null ? null : DispatchListHeadID.ToString());
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
        public string DelDispatchList(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityDispatchListHead entity, out int success)
        {
            success = 1;
            string msg = "";

            DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, PlainPassword);
            if (bll.GetGlmendFlag(entity.dDate.Year, entity.dDate.Month, "bflag_SA") == true)
            {
                success = 0;
                return string.Format("U8单据{0}日期所在月份已经结账!", entity.cDLCode);
            }
            if (GetDispatchList(entity.cDLCode) != 0)
            {
                if (DispatchListChecked(entity.cDLCode) == 0)
                {
                    msg = string.Format("U8中发/退货单:{0} 已审核,不允许删除!", entity.cDLCode);
                    success = 0;
                }
                else
                {
                    //执行删除发货单操作
                    DeleteDispatchList(entity.cDLCode);
                    msg = string.Format("U8中发/退货单:{0} 已删除!", entity.cDLCode);
                    success = 1;
                }
            }
            else
            {
                msg = string.Format("U8中发/退货单:{0} 不存在!", entity.cDLCode);
            }

            return msg;
        }

        /// <summary>
        /// 删除发货单
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int DeleteDispatchList(string cDLCode)
        {
            if (string.IsNullOrWhiteSpace(cDLCode))
                return 0;

            //处理现存量数据
            var stockSql = GetCurrentStockSql("Dis", -1);
            var head = GetEntityDispatchListHead(cDLCode);
            if (head.Details.Count != 0)
            {
                var sqlList = head.Details.Select(p => new ExecuteHelp
            {
                SQL = stockSql,
                Parameters = new SqlParameter[] { 
                                      new SqlParameter("@cWhCode",GetDBValue(p.cWhCode)),
                                      new SqlParameter("@cInvCode",GetDBValue(p.cInvCode)),
                                      new SqlParameter("@cBatch",GetDBValue(p.cBatch)),  
                                      new SqlParameter("@iQuantity",p.iQuantity),        
                     }
            }).ToList();
                // 执行处理现存量操作语句
                this.DB_SqlHelper.ExecuteNonQuery(sqlList);
            }

            List<ExecuteHelp> list = new List<ExecuteHelp>();

            string sql = "DELETE DispatchList WHERE cDLCode = @cDLCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cDLCode",cDLCode)
                                   };
            list.Add(new ExecuteHelp() { SQL = sql, Parameters = paras });
            int? DispatchListHeadID = GetDispatchListID(cDLCode);
            string delBodySql = "DELETE DispatchLists where DLID = @DLID";
            SqlParameter[] bodyParas = {
                                       new SqlParameter("@DLID",DispatchListHeadID)
                                   };
            list.Add(new ExecuteHelp() { SQL = delBodySql, Parameters = bodyParas });

            return this.DB_SqlHelper.ExecuteNonQuery(list);
        }
        /// <summary>
        /// 判断该单号是否存在： 0 不存在 否则存在
        /// </summary>
        /// <param name="cDLCode"></param>
        /// <returns></returns>
        public int GetDispatchList(string cDLCode)
        {
            string sql = "select count(*) as Num from DispatchList where cDLCode=@cDLCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cDLCode",cDLCode)
                                   };
            return Convert.ToInt32(this.DB_SqlHelper.ExecuteScalar(CommandType.Text, sql, paras).ToString());
        }

        public int? GetDispatchListID(string cDLCode)
        {
            string sql = "SELECT DLID FROM DispatchList WHERE cDLCode = @cDLCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cDLCode",cDLCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["DLID"].ToString());
            }

            return null;
        }
        /// <summary>
        /// 查询销售发货退货单号是否存在
        /// </summary>
        /// <param name="DispatchListHead"></param>
        /// <returns></returns>
        public int QueryDispatchList(string cdlcode)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from DispatchList where cDLCode=@cDLCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cDLCode",cdlcode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }
            return num;
        }

        /// <summary>
        /// 单据有没审核，返回 0 已审核，否则未审核
        /// </summary>
        /// <param name="cCode"></param>
        /// <returns></returns>
        public int DispatchListChecked(string cDLCode)
        {
            string sql = "select count(*) as Num from DispatchList where ISNULL(cVerifier ,'')='' and  cDLCode=@cDLCode ";
            SqlParameter[] paras = {
                                       new SqlParameter("@cDLCode",cDLCode)
                                   };
            return Convert.ToInt32(this.DB_SqlHelper.ExecuteScalar(CommandType.Text, sql, paras).ToString());
        }

        /// <summary>
        /// 获取发货单实体类数据
        /// </summary>
        /// <param name="cDLCode"></param>
        /// <returns></returns>
        public EntityDispatchListHead GetEntityDispatchListHead(string cDLCode)
        {

            EntityDispatchListHead entity = new EntityDispatchListHead();
            List<EntityDispatchListBody> listBody = new List<EntityDispatchListBody>();
            string sqlHead = "SELECT cDLCode ,dDate,cDepCode,cCusCode,cDefine1,cSTCode,cDefine14,ccusperson,cShipAddress,cDefine8,cDefine3,cMemo FROM DispatchList WHERE cDLCode=@cDLCode ";
            string sqlBody = "SELECT cInvCode ,cWhCode ,iQuantity,iQuotedPrice,iSum,iTaxRate,cBatch FROM DispatchLists where DLID = (SELECT DLID FROM DispatchList WHERE cDLCode = @cDLCode )";
            SqlParameter[] paras = {
                                       new SqlParameter("@cDLCode",cDLCode)
                                   };
            SqlDataReader drHead = DB_SqlHelper.ExecuteReader(CommandType.Text, sqlHead, paras);
            SqlDataReader drBody = DB_SqlHelper.ExecuteReader(CommandType.Text, sqlBody, paras);
            while (drBody.Read())
            {
                EntityDispatchListBody body = new EntityDispatchListBody();
                body.cInvCode = drBody["cInvCode"].ToString();
                body.cWhCode = drBody["cWhCode"].ToString();
                body.iQuantity = ConvertToDecimal(drBody["iQuantity"]);
                body.iQuotedPrice = ConvertToDecimal(drBody["iQuotedPrice"]);
                body.iSum = ConvertToDecimal(drBody["iSum"]);
                body.iTaxRate = ConvertToDecimal(drBody["iTaxRate"]);
                body.cBatch = drBody["cBatch"].ToString();
                listBody.Add(body);
            }

            if (drHead.Read())
            {
                entity.cDLCode = cDLCode;
                entity.dDate = Convert.ToDateTime(drHead["dDate"]);
                entity.cDepCode = drHead["cDepCode"].ToString();
                entity.cCusCode = drHead["cCusCode"].ToString();
                entity.cDefine1 = drHead["cDefine1"].ToString();
                entity.cSTCode = drHead["cSTCode"].ToString();
                entity.cDefine14 = drHead["cDefine14"].ToString();
                entity.ccusperson = drHead["ccusperson"].ToString();
                entity.cShipAddress = drHead["cShipAddress"].ToString();
                entity.cDefine8 = drHead["cDefine8"].ToString();
                entity.cDefine3 = drHead["cDefine3"].ToString();
                entity.cMemo = drHead["cMemo"].ToString();
                entity.Details = listBody;
            }
            return entity;
        }
    }
}
