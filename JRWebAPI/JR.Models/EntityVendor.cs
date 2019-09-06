using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 供应商档案
    /// </summary>
    public class EntityVendor:BaseEntity
    {
        /// <summary>
        /// 供应商编码
        /// </summary>
       [EntityCheck(Name = "供应商编码", IsMust = true)]
        public string cVenCode { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string cVenName { get; set; }
        /// <summary>
        /// 供应商简称
        /// </summary>
       [EntityCheck(Name = "供应商简称", IsMust = true)]
        public string cVenAbbName { get; set; }
        /// <summary>
        /// 供应商分类
        /// </summary>
        [EntityCheck(Name = "供应商分类", IsMust = true)]
        public string cVcCode { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string cMaker { get; set; }
    }
}
