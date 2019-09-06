using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityInventory : BaseEntity
    {
        /// <summary>
        /// 存货编码
        /// </summary>
       [EntityCheck(Name = "存货编码", IsMust = true)]
        public string cInvCode { get; set; }
        /// <summary>
        /// 存货名称
        /// </summary>
       [EntityCheck(Name = "存货名称", IsMust = true)]
        public string cInvName { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string cInvStd { get; set; }
        /// <summary>
        /// 存货分类
        /// </summary>
        [EntityCheck(Name = "存货分类", IsMust = true)]
        public string cInvCCode { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 条形码管理
        /// </summary>
        public int? bBarCode { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string cBarCode { get; set; }
        /// <summary>
        /// 参考成本
        /// </summary>
        public decimal? iInvSPrice { get; set; }
        /// <summary>
        /// 零售价格
        /// </summary>
        public decimal? fRetailPrice { get; set; }
        /// <summary>
        /// 包装规格
        /// </summary>
        public string cPackingType { get; set; }
        /// <summary>
        /// 计量单位组
        /// </summary>
        public string cGroupCode { get; set; }
        /// <summary>
        /// 计量单位编码
        /// </summary>
        public string cComUnitCode { get; set; }
        /// <summary>
        /// 计价方式
        /// </summary>
        public string cValueType { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string bInvBatch { get; set; }
        /// <summary>
        /// 存货代码
        /// </summary>
        public string cInvAddCode { get; set; }
        /// <summary>
        /// 折扣属性
        /// </summary>
        public string bInvType { get; set; }
        public string cInvDefine1 { get; set; }
        public string cInvDefine2 { get; set; }
        public string cInvDefine3 { get; set; }
        public string cInvDefine4 { get; set; }
        public string cInvDefine5 { get; set; }
        public string cInvDefine6 { get; set; }
        public string cInvDefine7 { get; set; }
        public string cInvDefine8 { get; set; }
        public string cInvDefine9 { get; set; }
        public string cInvDefine10 { get; set; }
        public int? cInvDefine11 { get; set; }
        public int? cInvDefine12 { get; set; }
        public decimal? cInvDefine13 { get; set; }

       // public decimal? cInvDefine14 { get; set; }  2019-03-14 取消该字段的对接
    }
}
