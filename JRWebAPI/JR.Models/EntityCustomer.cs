using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 客户档案实体类
    /// </summary>
    public class EntityCustomer:BaseEntity
    {
        /// <summary>
        /// 客户编码
        /// </summary>
        [EntityCheck(Name = "客户编码", IsMust = true)]
        public string cCusCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
       [EntityCheck(Name = "客户名称", IsMust = true)]
        public string cCusName { get; set; }
        /// <summary>
        /// 客户简称
        /// </summary>
       [EntityCheck(Name = "客户简称", IsMust = true)]
        public string cCusAbbName { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string cCusDepart { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string cCusDepName { get; set; }
        /// <summary>
        /// 经营属性
        /// </summary>
        public string cCusDefine1 { get; set; }        
        /// <summary>
        /// 客户分类编码
        /// </summary>
        [EntityCheck(Name = "客户分类编码", IsMust = true)]
        public string cCCCode { get; set; }
        /// <summary>
        /// 发货仓库编码
        /// </summary>
        public string cCusWhCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string cMemo { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string cMaker { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string cCusAddress { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string cCusHand { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string cCusPerson { get; set; }
    }
}
