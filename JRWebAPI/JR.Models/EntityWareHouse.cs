using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityWareHouse : BaseEntity
    {

        /// <summary>
        /// 仓库编码
        /// </summary>
        [EntityCheck(Name = "仓库编码", IsMust = true)]
        public string cWhCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        [EntityCheck(Name = "仓库名称", IsMust = true)]
        public string cWhName { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string cDepCode { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>      
        public string cWhPhone { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>       
        public string cWhPerson { get; set; }
        /// <summary>
        /// 仓库地址
        /// </summary>
        public string cWhAddress { get; set; }
        /// <summary>
        /// 是否门店
        /// </summary>
        public int? bShop { get; set; }
        /// <summary>
        /// 计价方式
        /// </summary>
        public string cWhValueStyle { get; set; }
    }
}
