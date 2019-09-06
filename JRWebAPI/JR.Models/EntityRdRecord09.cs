using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 其它出库单(配送出库单)
    /// </summary>
    public class EntityRdRecord09Head : BaseEntity
    {
        /// <summary>
        /// 单据类型：其他出库
        /// </summary>
        public string cBusType { get; set; }
        /// <summary>
        /// 收发类别编码
        /// </summary>
        [EntityCheck(Name = "收发类别编码", IsMust = true)]
        public string cRdCode { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        [EntityCheck(Name = "部门编码", IsMust = true)]
        public string cDepCode { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }

        /// <summary>
        /// 支持依据
        /// </summary>
        [EntityCheck(Name = "支持依据", IsMust = true)]
        public string cDefine1 { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        [EntityCheck(Name = "客户编码", IsMust = true)]
        public string cCusCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string cDefine11 { get; set; }

        /// <summary>
        /// 运费结算
        /// </summary>
        [EntityCheck(Name = "运费结算", IsMust = true)]
        public string cDefine8 { get; set; }

        /// <summary>
        /// 签报编号
        /// </summary>
        [EntityCheck(Name = "签报编号", IsMust = false)]
        public string cDefine10 { get; set; }

        /// <summary>
        /// 模板id:85
        /// </summary>
        public int VT_ID { get; set; }
        /// <summary>
        /// 发货地址
        /// </summary>
        [EntityCheck(Name = "发货地址", IsMust = false)]
        public string cShipAddress { get; set; }
        /// <summary>
        /// 委外期初
        /// </summary>
        public int bOMFirst { get; set; }

        /// <summary>
        /// 制单时间 
        /// </summary>
        public DateTime dnmaketime { get; set; }

        /// <summary>
        /// 单据条码
        /// </summary>
        public string csysbarcode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string cMemo { get; set; }


        /// <summary>
        /// 收发标志
        /// </summary>
        public int bRdFlag { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        [EntityCheck(Name = "仓库编码", IsMust = true)]
        public string cWhCode { get; set; }
        /// <summary>
        /// 库存期初标志
        /// </summary>
        public int blsSTQc { get; set; }
        /// <summary>
        /// 收发记录主表标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 单据类型编码
        /// </summary>
        public string cVouchType { get; set; }
        /// <summary>
        /// 单据来源
        /// </summary>
        public string cSource { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [EntityCheck(Name = "单据日期", IsMust = true)]
        public DateTime dDate { get; set; }
        /// <summary>
        /// 收发单据号
        /// </summary>
        [EntityCheck(Name = "单据号", IsMust = true)]
        public string cCode { get; set; }
        /// <summary>
        /// 是否传递
        /// </summary>
        public int bTransFlag { get; set; }

        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntityRdRecord09Body> Details { get; set; }

    }
    /// <summary>
    /// 配送出库单表体实体
    /// </summary>
    public class EntityRdRecord09Body : BaseEntity
    {
        /// <summary>
        /// 单价(取存货档案参考成本)
        /// </summary>
        public decimal iUnitCost { get; set; }

        /// <summary>
        /// 暂估金额
        /// </summary>
        public decimal iAPrice { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int irowno { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal cDefine26 { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal cDefine27 { get; set; }

        /// <summary>
        /// 单据行条码
        /// </summary>
        public string cbsysbarcode { get; set; }

        /// <summary>
        /// 标志
        /// </summary>
        public int iFlag { get; set; }
        /// <summary>
        /// 存货编码
        /// </summary>
        [EntityCheck(Name = "存货编码", IsMust = true)]
        public string cInvCode { get; set; }
        /// <summary>
        /// 收发记录字表标识
        /// </summary>
        public int AutoID { get; set; }
        /// <summary>
        /// 收发记录主表标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal iQuantity { get; set; }
        /// <summary>
        /// 订货平台明细ID
        /// </summary>
        [EntityCheck(Name = "订货平台明细ID", IsMust = true)]
        public string DHID { get; set; }

        /// <summary>
        /// 表体自定义项1-条形码
        /// </summary>
        public string cDefine22 { get; set; }

        /// <summary>
        /// 表体自定义项2-装箱规格
        /// </summary>
        public string cDefine23 { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string cBatch { get; set; }
    }
}
