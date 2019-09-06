using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 销售订单表头实体
    /// </summary>
    public class EntitySaleHead : BaseEntity
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [EntityCheck(Name = "订单号", IsMust = true)]
        public string cSOCode { get; set; }
        /// <summary>
        /// 订单日期
        /// </summary>
        [EntityCheck(Name = "订单日期", IsMust = true)]
        public DateTime dDate { get; set; }


        /// <summary>
        /// 业务类型
        /// </summary>
        public string cBusType { get; set; }
        /// <summary>
        /// 销售类型
        /// </summary>
        [EntityCheck(Name = "销售类型", IsMust = true)]
        public string cSTCode { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [EntityCheck(Name = "客户编号", IsMust = true)]
        public string cCusCode { get; set; }

        /// <summary>
        /// 客户简称
        /// </summary>
        public string cCusName { get; set; }
        /// <summary>
        /// 付款条件
        /// </summary>
        public string cPayCode { get; set; }
        /// <summary>
        /// 销售部门
        /// </summary>
        [EntityCheck(Name = "销售部门", IsMust = true)]
        public string cDepCode { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public float iTaxRate { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string cexch_name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string cMemo { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public float iExchRate { get; set; }
        /// <summary>
        /// 单据模版号 
        /// </summary>
        public string iVTid { get; set; }
        /// <summary>
        /// 退货标志（1-退货；0-正常）
        /// </summary>
        public int bReturnFlag { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int iStatus { get; set; }
        /// <summary>
        /// 支持依据名称
        /// </summary>
        [EntityCheck(Name = "支持依据名称", IsMust = true)]
        public string cDefine1 { get; set; }
        /// <summary>
        /// 运费结算名称
        /// </summary>
        [EntityCheck(Name = "运费结算名称", IsMust = true)]
        public string cDefine8 { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        [EntityCheck(Name = "仓库编码", IsMust = true)]
        public string cDefine12 { get; set; }
        /// <summary>
        /// 发货地址
        /// </summary>
        [EntityCheck(Name = "发货地址", IsMust = true)]
        public string cCusOAddress { get; set; }

        /// <summary>
        /// 表头单据条码
        /// </summary>
        public string cSysBarCode { get; set; }
        /// <summary>
        /// 促销政策
        /// </summary>
        public string cDefine14 { get; set; }
        /// <summary>
        /// 计入任务（是和否）
        /// </summary>
        public string cDefine3 { get; set; }

        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntitySaleBody> Details { get; set; }

    }

    /// <summary>
    /// 销售订单表体实体
    /// </summary>
    public class EntitySaleBody : BaseEntity
    {
        /// <summary>
        /// 订货平台明细ID
        /// </summary>
        [EntityCheck(Name = "订货平台明细ID", IsMust = true)]
        public string DHID { get; set; }
        /// <summary>
        /// 预发货日期
        /// </summary>
        public DateTime dPreDate { get; set; }


        /// <summary>
        /// 是否订单BOM
        /// </summary>
        public int bOrderBOM { get; set; }

        /// <summary>
        /// 存货编码 
        /// </summary>
        [EntityCheck(Name = "存货编码", IsMust = true)]
        public string cInvCode { get; set; }

        /// <summary>
        /// 存货名称
        /// </summary>
        public string cInvName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal iQuantity { get; set; }

        /// <summary>
        /// 原币价税合计
        /// </summary>
        public decimal iSum { get; set; }

        /// <summary>
        /// 报价
        /// </summary>
        public decimal iQuotedPrice { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal iTaxRate { get; set; }

        /// <summary>
        /// 辅计量数量
        /// </summary>
        public decimal iNum { get; set; }

        /// <summary>
        /// 原币无税单价
        /// </summary>
        public decimal iUnitPrice { get; set; }

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
        /// 累计发货辅计量数量
        /// </summary>
        public decimal iFHNum { get; set; }

        /// <summary>
        /// 累计发货数量
        /// </summary>
        public decimal iFHQuantity { get; set; }

        /// <summary>
        /// 累计原币发货金额
        /// </summary>
        public decimal iFHMoney { get; set; }

        /// <summary>
        /// 累计开票数量
        /// </summary>
        public decimal iKPQuantity { get; set; }

        /// <summary>
        /// 累计开票辅计量数量
        /// </summary>
        public decimal iKPNum { get; set; }

        /// <summary>
        /// 累计原币开票金额
        /// </summary>
        public decimal iKPMoney { get; set; }

        /// <summary>
        /// 扣率
        /// </summary>
        public decimal KL { get; set; }

        /// <summary>
        /// 二次扣率
        /// </summary>
        public decimal KL2 { get; set; }

        /// <summary>
        /// 折扣额
        /// </summary>
        public decimal iDisCount { get; set; }
        /// <summary>
        /// 零售单价
        /// </summary>
        public decimal fSaleCost { get; set; }

        /// <summary>
        /// 零售金额
        /// </summary>
        public decimal fSalePrice { get; set; }

        /// <summary>
        /// 预完工日期
        /// </summary>
        public DateTime dPreMoDate { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int iRowNo { get; set; }

        /// <summary>
        /// 客户最低售价 
        /// </summary>
        public decimal fcusminprice { get; set; }

        /// <summary>
        /// 是否全部采购
        /// </summary>
        public int ballpurchase { get; set; }

        /// <summary>
        /// 订单BOM是否完成
        /// </summary>
        public int bOrderBOMOver { get; set; }

        /// <summary>
        /// 使用客户BOM
        /// </summary>
        public int busecusbom { get; set; }

        /// <summary>
        /// 报价含税标识
        /// </summary>
        public int bsaleprice { get; set; }

        /// <summary>
        /// 赠品标识 
        /// </summary>
        public int bgift { get; set; }

        /// <summary>
        /// 单据行条码
        /// </summary>
        public string cbSysBarCode { get; set; }

        /// <summary>
        /// 表体自定义项1-条形码
        /// </summary>
        public string cDefine22 { get; set; }
        /// <summary>
        /// 表体自定义项2-装箱规格
        /// </summary>
        public string cDefine23 { get; set; }

    }
}
