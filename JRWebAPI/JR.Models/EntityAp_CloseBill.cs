using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 收付款单表头实体
    /// </summary>
    public class EntityAp_CloseBillHead : BaseEntity
    {
        
        /// <summary>
        /// 摘要
        /// </summary>
        public string cDigest { get; set; }
        /// <summary>
        /// 本币金额
        /// </summary>
        [EntityCheck(Name = "金额", IsMust = true)]
        public decimal iAmount { get; set; }
        /// <summary>
        /// 原币金额
        /// </summary>
        public decimal iAmount_f { get; set; }
        /// <summary>
        /// 本币余额
        /// </summary>
        public decimal iRAmount { get; set; }
        /// <summary>
        /// 原币余额
        /// </summary>
        public decimal iRAmount_f { get; set; }
        /// <summary>
        /// 本单位银行名称
        /// </summary>
        public string cNatBank { get; set; }
        /// <summary>
        /// 本单位银行账号 
        /// </summary>
        public string cNatBankAccount { get; set; }
        /// <summary>
        /// 收款类型 
        /// </summary>
        [EntityCheck(Name = "收款类型", IsMust = false)]
        public string cDefine2 { get; set; }
        /// <summary>
        /// 来源单据类型 
        /// </summary>
        [EntityCheck(Name = "来源单据类型", IsMust = true)]
        public string cDefine9 { get; set; }
        /// <summary>
        /// 来源单据号 
        /// </summary>
        [EntityCheck(Name = "来源单据号", IsMust = true)]
        public string cDefine10 { get; set; }
        /// <summary>
        /// 制单时间 
        /// </summary>
        public DateTime dcreatesystime { get; set; }
        /// <summary>
        /// 部门编码 
        /// </summary>
        [EntityCheck(Name = "部门编码", IsMust = true)]
        public string cDeptCode { get; set; }


        /// <summary>
        /// 付款类型
        /// </summary>
        public int iPayType { get; set; }
        /// <summary>
        /// 单据类型编码
        /// </summary>
        [EntityCheck(Name = "单据类型编码", IsMust = true)]
        public string cVouchType { get; set; }
        /// <summary>
        /// 单据编号
        /// </summary>
        [EntityCheck(Name = "单据编号", IsMust = true)]
        public string cVouchID { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        [EntityCheck(Name = "单据日期", IsMust = true)]
        public DateTime dVouchDate { get; set; }
        /// <summary>
        /// 会计期间
        /// </summary>
        public int iPeriod { get; set; }
        /// <summary>
        /// 单位编码
        /// </summary>
        [EntityCheck(Name = "单位编码", IsMust = true)]
        public string cDwCode { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string cexch_name { get; set; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string cOperator { get; set; }
        /// <summary>
        /// 应收应付标志
        /// </summary>
        public string cFlag { get; set; }
        /// <summary>
        /// 主表标识
        /// </summary>
        public int iID { get; set; }
        /// <summary>
        /// 银行导入标志
        /// </summary>
        public int bFromBank { get; set; }
        /// <summary>
        /// 导出银行标志
        /// </summary>
        public int bToBank { get; set; }
        /// <summary>
        /// 银行确认标志
        /// </summary>
        public int bSure { get; set; }
        /// <summary>
        /// 单据模板号
        /// </summary>
        public int VT_ID { get; set; }
        /// <summary>
        /// 结算方式编码
        /// </summary>
        [EntityCheck(Name = "结算方式编码", IsMust = true)]
        public string cSSCode { get; set; }
        /// <summary>
        /// 本单位银行编码
        /// </summary>
        public string cNatBankCode { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal iExchRate { get; set; }
        /// <summary>
        /// 预收预付标志
        /// </summary>
        public int bPrePay { get; set; }
        /// <summary>
        /// 系统内部标识
        /// </summary>
        public string csysbarcode { get; set; }
        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntityAp_CloseBillBody> Details { get; set; }
    }
    /// <summary>
    /// 收付款单表体实体
    /// </summary>
    public class EntityAp_CloseBillBody : BaseEntity
    {
        /// <summary>
        /// 款项用途
        /// </summary>
        [EntityCheck(Name = "款项用途", IsMust = true)]
        public string cDefine22 { get; set; }
        /// <summary>
        /// 是否开票
        /// </summary>
        public string cDefine25 { get; set; }
        /// <summary>
        /// 促销政策
        /// </summary>
        public string cDefine24 { get; set; }

        /// <summary>
        /// 费用结算金额
        /// </summary>
        public decimal ifaresettled_f { get; set; }
        /// <summary>
        /// 主表标识
        /// </summary>
        public int iID { get; set; }
        /// <summary>
         /// 发货退货单主表标识
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 款项类型号
        /// </summary>
        public int? iType { get; set; }
        /// <summary>
        /// 预收预付标志
        /// </summary>
        public int? bPrePay { get; set; }
        /// <summary>
        /// 客户或供应商编码
        /// </summary>
        public string cCusVen { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string cDepCode { get; set; }
        /// <summary>
        /// 原币金额
        /// </summary>
        public decimal iAmt_f { get; set; }
        /// <summary>
        /// 本币金额
        /// </summary>
        [EntityCheck(Name = "本币金额", IsMust = true)]
        public decimal iAmt { get; set; }
        /// <summary>
        /// 原币余额
        /// </summary>
        public decimal iRAmt_f { get; set; }
        /// <summary>
        /// 本币余额
        /// </summary>
        [EntityCheck(Name = "本币余额", IsMust = true)]
        public decimal iRAmt { get; set; }
        /// <summary>
        /// 订货平台明细ID
        /// </summary>
        [EntityCheck(Name = "订货平台明细ID", IsMust = true)]
        public string DHID { get; set; }
    }
}
