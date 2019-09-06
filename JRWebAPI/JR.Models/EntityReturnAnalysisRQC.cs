using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityReturnAnalysisRQC
    {
        public string DateBeg { get; set; }//单据日期
        public string DateEed { get; set; }//单据日期
        //public string Depart1 { get; set; }//部门编码
        //public string Depart2 { get; set; }//部门编码
        public string cCusCode { get; set; }//代理商编码
        //public string cCusCode2 { get; set; }//代理商编码
        //public string cCusDefine11 { get; set; }//经营属性
        //public string cCusDefine12 { get; set; }//经营属性
        //public string ICategory1 { get; set; }//存货分类编码
        // public string ICategory2 { get; set; }//存货分类编码
        public string InvCode { get; set; }//存货编码
        //public string InvCode2 { get; set; }//存货编码
        //public decimal? SumQuantity1 { get; set; }//累计退货数量
        // public decimal? SumQuantity2 { get; set; }//累计退货数量
        //public decimal? SumMoney1 { get; set; }//累计退货金额
        // public decimal? SumMoney2 { get; set; }//累计退货金额
        //public string ReProportion1 { get; set; }//退货比例
        //public string ReProportion2 { get; set; }//退货比例
    }
}
