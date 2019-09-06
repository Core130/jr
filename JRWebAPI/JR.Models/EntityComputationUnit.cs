using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityComputationUnit : BaseEntity
    {
        /// <summary>
        /// 单位编码
        /// </summary>
        public string cComUnitCode { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string cComUnitName { get; set; }
        /// <summary>
        /// 单位组编码
        /// </summary>
        public string cGroupCode { get; set; }
    }
}
