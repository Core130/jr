using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityInventoryPrice
    {
        /// <summary>
        /// 供应商编码
        /// </summary>
        public string cVenCode { get; set; }
        /// <summary>
        /// 存货编码
        /// </summary>
        public string cInvCode { get; set; }

        /// <summary>
        /// 未结算含税单价
        /// </summary>
        public decimal noSettleTaxCost { get; set; }

        /// <summary>
        /// 结算含税单价
        /// </summary>
        public decimal SettleTaxCost { get; set; }
        /// <summary>
        /// 供应商存货价格表含税价
        /// </summary>
        public decimal VenInvPrice { get; set; }
    }
}
