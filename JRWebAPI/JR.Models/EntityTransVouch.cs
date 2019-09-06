using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 调拨单表头实体类
    /// </summary>
     public class EntityTransVouch:BaseEntity
    {
        /// <summary>
        /// 单据号
        /// </summary>
        [EntityCheck(Name = "单据号", IsMust = true)]
        public string cTvCode { get; set; }

        /// <summary>
        /// 单据日期
        /// </summary>
        [EntityCheck(Name = "单据日期", IsMust = true)]
        public DateTime dTvDate { get; set; }

        /// <summary>
        /// 调出仓库
        /// </summary>
        [EntityCheck(Name = "调出仓库", IsMust = true)]
        public string cOutWhCode { get; set; }

        /// <summary>
        /// 调入仓库
        /// </summary>
        [EntityCheck(Name = "调入仓库", IsMust = true)]
        public string cInWhCode { get; set; }

        /// <summary>
        /// 调出部门
        /// </summary>
        [EntityCheck(Name = "调出部门", IsMust = true)]
        public string cOutDepCode { get; set; }

        /// <summary>
        /// 调入部门
        /// </summary>
        [EntityCheck(Name = "调入部门", IsMust = true)]
        public string cInDepCode { get; set; }

        /// <summary>
        /// 入库类别
        /// </summary>
        public string cInRdCode { get; set; }

        /// <summary>
        /// 出库类别
        /// </summary>
        public string cOutRdCode { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string cTvMemo { get; set; }

        /// <summary>
        /// 制单人
        /// </summary>
        public string cMaker { get; set; }

        /// <summary>
        /// 客户编码
        /// </summary>
        public string cCusCode { get; set; }


        /// <summary>
        /// 调拨申请单表体
        /// </summary>
        [EntityCheck(Name = "单据明细", IsMust = true)]
        public IList<EntityTransVouchBody> Details { get; set; }
    }

     /// <summary>
     /// 调拨申请单表体实体类
     /// </summary>
     public class EntityTransVouchBody : BaseEntity
     {
         /// <summary>
         /// 存货编码
         /// </summary>
         [EntityCheck(Name = "存货编码", IsMust = true)]
         public string cInvCode { get; set; }

         /// <summary>
         /// 申请数量
         /// </summary>
         [EntityCheck(Name = "数量", IsMust = true)]
         public decimal iTvQuantity { get; set; }

         /// <summary>
         /// 单价
         /// </summary>
         [EntityCheck(Name = "单价", IsMust = true)]
         public decimal iTvaCost { get; set; }

         /// <summary>
         /// 金额
         /// </summary>
         [EntityCheck(Name = "金额", IsMust = true)]
         public decimal iTvaPrice { get; set; }


         /// <summary>
         /// 配送单价
         /// </summary>
         [EntityCheck(Name = "配送单价", IsMust = true)]
         public decimal cDefine26 { get; set; }

         /// <summary>
         /// 配送金额
         /// </summary>
         [EntityCheck(Name = "配送金额", IsMust = true)]
         public decimal cDefine27 { get; set; }
         /// <summary>
         /// 批号
         /// </summary>
         public string cBatch { get; set; }
     }
}
