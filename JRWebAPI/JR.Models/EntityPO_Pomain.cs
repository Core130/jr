using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
   /// <summary>
   /// 采购订单表头实体类
   /// </summary>
    public class EntityPO_Pomain : BaseEntity
    {
        /// <summary>
        /// 采购订单主表标识
        /// </summary>
        public int POID { get; set; }
        /// <summary>
        /// 采购订单号
        /// </summary>
       [EntityCheck(Name = "订单号", IsMust = true)]
        public string cPOID { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
       [EntityCheck(Name = "单据日期", IsMust = true)]
        public DateTime dPODate { get; set; }
        /// <summary>
        /// 供应商编码
        /// </summary>
       [EntityCheck(Name = "供应商编码", IsMust = true)]
        public string cVenCode { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string cexch_name { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? nflat { get; set; }
        /// <summary>
        /// 表头税率
        /// </summary>
        public decimal? iTaxRate { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 单据模板号
        /// </summary>
        public int iVTid { get; set; }
        /// <summary>
        /// 业务类型（普通采购）
        /// </summary>
        public string cBusType { get; set; }
        /// <summary>
        /// 扣税类别（0应税外加，1应税内含）
        /// </summary>
        public int iDiscountTaxType { get; set; }
        /// <summary>
        /// 制单时间
        /// </summary>
        public DateTime? cmaketime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int cState { get; set; }
        /// <summary>
        /// 单据审核状态
        /// </summary>
        public int iVerifyStateex { get; set; }
        /// <summary>
        /// 单据内码
        /// </summary>
        public string cSysbarCode { get; set; }

        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntityPO_PoDetails> Details { get; set; }
    }
    /// <summary>
    /// 采购订单表体实体类
    /// </summary>
    public class EntityPO_PoDetails : BaseEntity
    {
        /// <summary>
        /// 采购订单子表标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 存货编码
        /// </summary>
        [EntityCheck(Name="存货编码",IsMust=true)]
        public string cInvCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [EntityCheck(Name = "数量", IsMust = true)]
        public decimal iQuantity { get; set; }
        /// <summary>
        /// 原币无税单价
        /// </summary>
        public decimal? iUnitPrice { get; set; }
        /// <summary>
        /// 原币无税金额 
        /// </summary>
        public decimal? iMoney { get; set; }
        /// <summary>
        /// 原币税额
        /// </summary>
        public decimal? iTax { get; set; }
        /// <summary>
        /// 原币价税合计
        /// </summary>
        public decimal? iSum { get; set; }
        /// <summary>
        /// 本币无税单价
        /// </summary>
        public decimal? iNatUnitPrice { get; set; }
        /// <summary>
        /// 本币无税金额
        /// </summary>
        public decimal? iNatMoney { get; set; }
        /// <summary>
        /// 本币税额
        /// </summary>
        public decimal? iNatTax { get; set; }
        /// <summary>
        /// 本币价税合计
        /// </summary>
        public decimal? iNatSum { get; set; }
        /// <summary>
        /// 计划到货日期
        /// </summary>
        public DateTime? dArriveDate { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        [EntityCheck(Name = "税率", IsMust = true)]
        public decimal? iPerTaxRate { get; set; }
        /// <summary>
        /// 采购订单主表标识
        /// </summary>
        public int POID { get; set; }
        /// <summary>
        /// 原币含税单价
        /// </summary>
        [EntityCheck(Name = "原币含税单价", IsMust = true)]
        public decimal? iTaxPrice { get; set; }
    }
}
