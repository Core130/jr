using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    [AttributeUsage(AttributeTargets.All)]
    public class EntityCheckAttribute : Attribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsMust { get; set; }
    }
}
