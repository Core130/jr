using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JRWebAPI.WF.TEST
{
    /// <summary>
    /// 销售发货单表头实体
    /// </summary>
    public class EntityDispatchListHead 
    {
        /// <summary>
        /// 发货退货单主表标识
        /// </summary>
        public int DLID { get; set; }
        /// <summary>
        /// 单据类型编码
        /// </summary>
        public string cVouchType { get; set; }
        /// <summary>
        /// 销售类型编码
        /// </summary>
        
        public string cSTCode { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
       
        public DateTime dDate { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        
        public string cDepCode { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string cexch_name { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public float iExchRate { get; set; }
        /// <summary>
        /// 销售期初标志
        /// </summary>
        public int bFirst { get; set; }
        /// <summary>
        /// 退货标志
        /// </summary>
        public int bReturnFlag { get; set; }
        /// <summary>
        /// 结算标志
        /// </summary>
        public int bSettleAll{get;set;}
        /// <summary>
        /// 单据模板号
        /// </summary>
        public int iVTid { get; set; }
        /// <summary>
        /// 客户编码
        /// </summary>
        
        public string cCusCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string cCusName { get; set; }
        /// <summary>
        /// 业务类型
        /// </summary>
        public string cBusType { get; set; }
        /// <summary>
        /// 退货单号（唯一）
        /// </summary>
        
        public string cDLCode { get; set; }
        /// <summary>
        /// 销售订单号
        /// </summary>
        public string cSOCode { get; set; }


        // *************************************                   

        /// <summary>
        /// 支持依据
        /// </summary>
        
        public string cDefine1 { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 表头单据条码
        /// </summary>
        public string cSysBarCode { get; set; }
        /// <summary>
        /// 计入任务（是和否）
        /// </summary>
        public string cDefine3 { get; set; }

        public IList<EntityDispatchListBody> Details { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string cMemo { get;set;}
         
    }
    /// <summary>
    /// 销售发货单表体实体
    /// </summary>
    public class EntityDispatchListBody 
    {

        /// <summary>
        /// 报价
        /// </summary>
        
        public decimal iQuotedPrice { get; set; }

        /// <summary>
        /// 原币含税单价
        /// </summary>
        public decimal iTaxUnitPrice { get; set; }

        /// <summary>
        /// 原币无税金额
        /// </summary>
        public decimal iMoney { get; set; }
        /// <summary>
        /// 原币税额
        /// </summary>
        public decimal iTax { get; set; }

        /// <summary>
        /// 本币无税单价
        /// </summary>
        public decimal iNatUnitPrice { get; set; }

        /// <summary>
        /// 本币无税金额
        /// </summary>
        public decimal iNatMoney { get; set; }

        /// <summary>
        /// 本币税额
        /// </summary>
        public decimal iNatTax { get; set; }

        /// <summary>
        /// 本币价税合计
        /// </summary>
        public decimal iNatSum { get; set; }
        /// <summary>
        /// 本币折扣额
        /// </summary>
        public decimal iNatDisCount { get; set; }
        /// <summary>
        /// 折扣额
        /// </summary>
        public decimal iDisCount { get; set; }
        /// <summary>
        /// 原币价税合计
        /// </summary>
        
        public decimal iSum { get; set; }
        /// <summary>
        /// 扣率
        /// </summary>
        public decimal KL { get; set; }

        /// <summary>
        /// 二次扣率
        /// </summary>
        public decimal KL2 { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        
        public decimal iTaxRate { get; set; }


        /// <summary>
        /// 零售单价
        /// </summary>
        public decimal fSaleCost { get; set; }

        /// <summary>
        /// 零售金额
        /// </summary>
        public decimal fSalePrice { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int iRowNo { get; set; }

        /// <summary>
        /// 单据行条码
        /// </summary>
        public string cbSysBarCode { get; set; }

        //////////////////////////////////////////////

        /// <summary>
        /// 是否记账
        /// </summary>
        public int bcosting { get; set; }
        /// <summary>
        /// 发货退货单子表标识
        /// </summary>
        public int AutoID { get; set; }
        /// <summary>
        /// 发货退货单主表标识
        /// </summary>
        public int DLID { get; set; }
        /// <summary>
        /// 存货编码
        /// </summary>
        
        public string cInvCode { get; set; }
        /// <summary>
        /// 存货名称
        /// </summary>
        public string cInvName { get; set; }
        /// <summary>
        /// 结算标志
        /// </summary>
        public int bSettleAll { get; set; }
        /// <summary>
        /// 发货退货单字表标识2
        /// </summary>
        public int iDLsID { get; set; }
        /// <summary>
        /// 是否质检
        /// </summary>
        public int bQANeedCheck { get; set; }
        /// <summary>
        /// 是否急料
        /// </summary>
        public int bQAUrgency { get; set; }
        /// <summary>
        /// 是否再检
        /// </summary>
        public int bQAChecking { get; set; }
        /// <summary>
        /// 是否报检
        /// </summary>
        public int bQAChecked { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        
        public string cWhCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        
        public decimal iQuantity { get; set; }
        /// <summary>
        /// 原币无税单价
        /// </summary>
        public decimal iUnitPrice { get; set; }
        /// <summary>
        /// 订货平台明细ID
        /// </summary>
        
        public string DHID { get; set; }
    }
}
