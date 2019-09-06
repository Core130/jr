using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityRdRecord01Head : BaseEntity
    {
        /// <summary>
        /// 收发记录主表标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 收发标志
        /// </summary>
        public int bRdFlag { get; set; }
        /// <summary>
        /// 单据类型编码（01）
        /// </summary>
        public string cVouchType { get; set; }
        /// <summary>
        /// 业务类型（普通采购）
        /// </summary>
        public string cBusType { get; set; }
        /// <summary>
        /// 采购类型
        /// </summary>
        [EntityCheck(Name = "采购类型", IsMust = true)]
        public string cPTCode { get; set; }
        /// <summary>
        /// 单据来源（库存）
        /// </summary>
        public string cSource { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        [EntityCheck(Name = "仓库编码", IsMust = true)]
        public string cWhCode { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [EntityCheck(Name = "单据日期", IsMust = true)]
        public DateTime dDate { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        [EntityCheck(Name = "单据号", IsMust = true)]
        public string cCode { get; set; }
        /// <summary>
        /// 收发类别编码
        /// </summary>
        public string cRdCode { get; set; }
        /// <summary>
        /// 供应商编码
        /// </summary>
        [EntityCheck(Name = "供应商编码", IsMust = true)]
        public string cVenCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string cMemo { get; set; }
        /// <summary>
        /// 是否传递（0）
        /// </summary>
        public int bTransFlag { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 单据模板号（27）
        /// </summary>
        public int VT_ID { get; set; }
        /// <summary>
        /// 库存期初标志（0）
        /// </summary>
        public int bIsSTQc { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal? iTaxRate { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? iExchRate { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string cExch_Name { get; set; }
        /// <summary>
        /// 采购订单号
        /// </summary>   
        public string cOrderCode { get; set; }

        /// <summary>
        /// 内部标识号
        /// </summary>
        public string cSysbarCode { get; set; }

        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<Entityrdrecords01Body> Details { get; set; }

    }
    public class Entityrdrecords01Body : BaseEntity
    {
        /// <summary>
        /// 收发记录子表标识
        /// </summary>
        public int AutoID { get; set; }
        /// <summary>
        /// 收发记录主表标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 存货编码
        /// </summary>
       [EntityCheck(Name = "存货编码", IsMust = true)]
        public string cInvCode { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [EntityCheck(Name = "数量", IsMust = true)]
        public decimal iQuantity { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal? iUnitCost { get; set; }
        /// <summary>
        /// 金额
        /// </summary>        
        public decimal? iPrice { get; set; }
        /// <summary>
        /// 暂估金额
        /// </summary>
        public decimal? iAPrice { get; set; }
        /// <summary>
        /// 标志（0）
        /// </summary>
        public int iFlag { get; set; }
        /// <summary>
        /// 暂估单价
        /// </summary>
        public decimal? fACost { get; set; }
        /// <summary>
        /// 表头供应商
        /// </summary>
        public string chVencode { get; set; }
        /// <summary>
        /// 原币含税单价
        /// </summary>
       [EntityCheck(Name = "原币含税单价", IsMust = true)]
        public decimal iOriTaxCost { get; set; }
        /// <summary>
        /// 原币无税单价
        /// </summary>
        public decimal? iOriCost { get; set; }
        /// <summary>
        /// 原币无税金额
        /// </summary>
        public decimal? iOriMoney { get; set; }
        /// <summary>
        /// 原币税额
        /// </summary>
        public decimal? iOriTaxPrice { get; set; }
        /// <summary>
        /// 原币价税合计
        /// </summary>
        [EntityCheck(Name = "原币价税合计", IsMust = true)]
        public decimal ioriSum { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        [EntityCheck(Name = "税率", IsMust = true)]
        public decimal iTaxRate { get; set; }
        /// <summary>
        /// 本币税额
        /// </summary>
        public decimal? iTaxPrice { get; set; }
        /// <summary>
        /// 本币价税合计
        /// </summary>
        public decimal? iSum { get; set; }

        /// <summary>
        /// 关联订单明细ID
        /// </summary>
        public string iPOsID { get; set; }

        /// <summary>
        /// 是否成本核算
        /// </summary>
        public int bCosting { get; set; }
        /// <summary>
        /// 单据结算次数
        /// </summary>
        public int iBillSettleCount { get; set; }
        /// <summary>
        /// 结算状态
        /// </summary>
        public int iMatSettleState { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string cBatch { get; set; }

    }
}
