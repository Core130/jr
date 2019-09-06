using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    /// <summary>
    /// 库存表实体
    /// </summary>
    public class EntityCurrentStock : BaseEntity
    {
        public string cInvCode { get; set; }//存货编码
        public string cWhCode { get; set; }//仓库编码
        public string cBatch { get; set; }//批次
        public decimal iQuantity { get; set; }//数量
    }
}
