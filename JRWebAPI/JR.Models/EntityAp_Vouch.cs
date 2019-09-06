using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 其他应付单表头实体类
    /// </summary>
    public class EntityAp_VouchHead : BaseEntity
    {
        /// <summary>
        /// 联结号
        /// </summary>
        public string cLink { get; set; }
        /// <summary>
        /// 单据类型编码
        /// </summary>
        public string cVouchType { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        [EntityCheck(Name="单据编号",IsMust=true)]
        public string cVouchID { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [EntityCheck(Name="单据日期",IsMust=true)]
        public DateTime dVouchDate { get; set; }
        /// <summary>
        /// 供应商编码
        /// </summary>
        [EntityCheck(Name = "供应商编码", IsMust = true)]
        public string cDwCode { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string cexch_name { get; set; }
        /// <summary>
        /// 本币余额
        /// </summary>
        public decimal? iRAmount { get; set; }
        /// <summary>
        /// 本币金额
        /// </summary>
        public decimal? iAmount { get; set; }
        /// <summary>
        /// 原币金额
        /// </summary>
        [EntityCheck(Name = "原币金额", IsMust = true)]
        public decimal? iAmount_f { get; set; }
        /// <summary>
        /// 原币余额
        /// </summary>
        public decimal? iRAmount_f { get; set; }
        /// <summary>
        /// 应收应付标志
        /// </summary>
        public string cFlag { get; set; }
        /// <summary>
        /// 单据模板号
        /// </summary>
        public int? VT_ID { get; set; }
        /// <summary>
        /// 结算单子表标识
        /// </summary>
        public int? iClosesID { get; set; }
        /// <summary>
        /// 对应结算单子表标识
        /// </summary>
        public int? iCoClosesID { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? iExchRate { get; set; }
        /// <summary>
        /// 借贷方向
        /// </summary>
        public int bd_c { get; set; }
        /// <summary>
        /// 制单人
        /// </summary>
        public string cOperator { get; set; }
        /// <summary>
        /// 制单时间
        /// </summary>
        public DateTime? dcreatesystime { get; set; }
        /// <summary>
        /// 表头单据条码
        /// </summary>
        public string cSysBarCode { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string cDigest { get; set; }
        /// <summary>
        /// 其他应付单表体
        /// </summary>
        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntityAp_VouchBody> Details { get; set; }
       
    }
    /// <summary>
    /// 其他应付单表体实体类
    /// </summary>
    public class EntityAp_VouchBody : BaseEntity
    {
        /// <summary>
        /// 联结号
        /// </summary>
        public string cLink { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string cexch_name { get; set; }
        /// <summary>
        /// 单位编码
        /// </summary>
        public string cDwCode { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal? iExchRate { get; set; }
        /// <summary>
        /// 借贷方向
        /// </summary>
        public int bd_c { get; set; }
        /// <summary>
        /// 本币金额
        /// </summary>
        public decimal? iAmount { get; set; }
        /// <summary>
        /// 原币金额
        /// </summary>
        public decimal? iAmount_f { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string cDigest { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public string cCode { get; set; }
    }
}
