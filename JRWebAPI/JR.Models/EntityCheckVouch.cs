using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityCheckVouchHead : BaseEntity
    {
        /// <summary>
        /// 盘点单号
        /// </summary>
        [EntityCheck(Name = "盘点单号", IsMust = true)]
        public string cCVCode { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        [EntityCheck(Name = "单据日期", IsMust = true)]
        public DateTime dCVDate { get; set; }
        /// <summary>
        /// 入库类别编码
        /// </summary>
        public string cIRdCode { get; set; }
        /// <summary>
        /// 出库类别编码
        /// </summary>
        public string cORdCode { get; set; }

        /// <summary>
        /// 盘点仓库编码
        /// </summary>
        [EntityCheck(Name = "盘点仓库编码", IsMust = true)]
        public string cWhCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string cCVMemo { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 账面日期
        /// </summary>
        [EntityCheck(Name = "账面日期", IsMust = true)]
        public DateTime dACDate { get; set; }
        /// <summary>
        /// 内部标识号
        /// </summary>
        public string cSysbarCode { get; set; }

        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntityCheckVouchBody> Details { get; set; }
    }

    public class EntityCheckVouchBody : BaseEntity
    {
        /// <summary>
        /// 存货编码
        /// </summary>
        [EntityCheck(Name = "存货编码", IsMust = true)]
        public string cInvCode { get; set; }
        /// <summary>
        /// 账面数量
        /// </summary>
        [EntityCheck(Name = "账面数量", IsMust = true)]
        public decimal iCVQuantity { get; set; }
        /// <summary>
        /// 表体自定义项目5（第三方传入账面数）
        /// </summary>
        public decimal cDefine26 { get; set; }
        /// <summary>
        /// 盘点数量
        /// </summary>
        [EntityCheck(Name = "盘点数量", IsMust = true)]
        public decimal iCVCQuantity { get; set; }
        /// <summary>
        /// 盘点原因
        /// </summary>
        public string cCVReason { get; set; }
        /// <summary>
        /// 调整入库数量
        /// </summary>
        public decimal iAdInQuantity { get; set; }
        /// <summary>
        /// 调整出库数量
        /// </summary>
        public decimal iAdOutQuantity { get; set; }
        /// <summary>
        /// 盈亏比例%
        /// </summary>
        public decimal iActualWaste { get; set; }       
        /// 批号
        /// </summary>
        public string cBatch { get; set; }
    }
}
