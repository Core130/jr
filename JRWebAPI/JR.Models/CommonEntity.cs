using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public enum ModelsType
    {
        /// <summary>
        /// 销售订单
        /// </summary>
        Sale = 8001,
        /// <summary>
        /// 配送出库单(其他出库单)
        /// </summary>
        RdRecord09 = 8002,
        /// <summary>
        /// 销售退货单
        /// </summary>
        DispatchList1 = 8003,
        /// <summary>
        /// 销售发货单
        /// </summary>
        DispatchList2 = 8005,
        /// <summary>
        /// 收付款单（汇款通知单）
        /// </summary>
        Ap_CloseBill = 8004,
        /// <summary>
        /// 库存量
        /// </summary>
        CurrentStock = 8006,
        /// <summary>
        /// 退货分析
        /// </summary>
        ReturnAnalysis = 8007,
        /// <summary>
        /// 客户档案
        /// </summary>
        Customer=8008,
        /// <summary>
        /// 供应商档案
        /// </summary>
        Vendor=8009,
        /// <summary>
        /// 采购类型
        /// </summary>
        PurchaseType=8010,
        /// <summary>
        /// 其他应付款
        /// </summary>
        Ap_Vouch=8011,
        /// <summary>
        /// 存货档案
        /// </summary>
        Inventory=8012,
        /// <summary>
        /// 仓库档案
        /// </summary>
        WareHouse=8013,
        /// <summary>
        /// 收发类别档案（入库类别）
        /// </summary>
        rd_Style=8014,
        /// <summary>
        /// 采购订单
        /// </summary>
        PO_Pomain=8015,
        /// <summary>
        /// 采购入库单
        /// </summary>
        RdRecord01=8016,
        /// <summary>
        /// 结算方式档案
        /// </summary>
        SettleStyle=8017,
        /// <summary>
        /// 付款单（同应付应收：Ap_CloseBill）
        /// </summary>
        PayAp_CloseBill=8018,
        /// <summary>
        /// 客户经营属性
        /// </summary>
        CustomerProperty=8019,

        /// <summary>
        /// 调拨申请单
        /// </summary>
        ST_AppTransVouch = 8020,

        /// <summary>
        /// 调拨单
        /// </summary>
        TransVouch = 8021,

        /// <summary>
        /// 盘点单
        /// </summary>
        CheckVouch = 8022,
        /// <summary>
        /// 存货分类
        /// </summary>
        InventoryClass=8023,

        /// <summary>
        /// 存货入库含税单价
        /// </summary>
        InventoryPrice=8024,
        /// <summary>
        /// 委托代销发货单
        /// </summary>
        DispatchList3 =8025,
        /// <summary>
        /// 委托代销退货单
        /// </summary>
        DispatchList4 = 8026,

        /// <summary>
        /// 红字采购入库单
        /// </summary>
        RdRecord01T =8027
       
    }

    /// <summary>
    /// 返回信息
    /// </summary>
    public class ReturnInfo
    {
        /// <summary>
        /// 成功标识(1:成功 0:失败)
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// 返回信息(成功:返回ID号，失败:失败原因)
        /// </summary>
        public string Message { get; set; }  
        public object Entity { get; set; }
      
    }
}
