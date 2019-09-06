using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 存货分类
    /// </summary>
    public class EntityInventoryClass : BaseEntity
    {
        /// <summary>
        /// 存货大类编码
        /// </summary>
        [EntityCheck(Name = "存货大类编码", IsMust = true)]
        public string cInvCCode { get; set; }
        /// <summary>
        /// 存货大类名称
        /// </summary>
        [EntityCheck(Name = "存货大类名称", IsMust = true)]
        public string cInvCName { get; set; }
        /// <summary>
        /// 编码级次
        /// </summary>
        public int? iInvCGrade { get; set; }
    }
}
