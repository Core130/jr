using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class EntityReturnAnalysisReport
    {
        public string cDepCode { get; set; }//部门编码
        public string cDepName { get; set; }//部门名称
        public string cCusCode { get; set; }//客户编码
        public string cCusName { get; set; }//客户名称
        public string cCusAbbName { get; set; }//客户简称
        public string cCusDefine1 { get; set; }//经营属性
        public string cInvCode { get; set; }//存货编码
        public string cInvName { get; set; }//存货名称
        public string cInvStd { get; set; }//规格型号
        public string cInvCCode { get; set; }//存货分类编码
        public string cInvCName { get; set; }//存货分类名称
        public string cComUnitCode { get; set; }//单位编码
        public string cComUnitName { get; set; }//单位名称
        public decimal thIQuantity { get; set; }//累计退货数量
        public decimal thINatSum { get; set; }//累计退货金额
        public decimal fhIQuantity { get; set; }//累计提货数量
        public decimal fhINatSum { get; set; }//累计提货金额
        public decimal actualQty { get; set; }//实际退货数量
        public decimal actualSum { get; set; }//实际退货金额
        //public string percentSum { get; set; }//退货占提货金额比例
    }
}
