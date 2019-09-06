using JR.HL;
using JR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.BLL
{
    /// <summary>
    /// 存货分类处理
    /// </summary>
    public class InventoryClassBll : U8BllBase
    {
        public InventoryClassBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        public string AddInventoryClass(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityInventoryClass entity, out int success)
        {
            success = 0;
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }
            if (!string.IsNullOrWhiteSpace(GetInventoryClass(entity.cInvCCode)))
            {
                return string.Format("U8中已存在存货分类编码:{0}!", entity.cInvCCode);
            }
            #endregion
            #region 验证存货分类编码规范
            int length = entity.cInvCCode.Length;
            var code = string.Empty;//
            var gradeDef = StringSpilt(GetGradeDef());
            for (int i = 0; i < gradeDef.Count; i++)
            {
                if (length == gradeDef[i])
                {
                    if (i == 0)
                    {
                        code = entity.cInvCCode;
                    }
                    else
                    {
                        code = entity.cInvCCode.Substring(0, gradeDef[i - 1]);
                    }

                    entity.iInvCGrade = i + 1;//编码级次
                    break;
                }
            }
            if (string.IsNullOrEmpty(code))
            {
                return string.Format("存货分类编码不合法:{0}!", entity.cInvCCode);
            }
            if (entity.iInvCGrade > 1)
            {
                if (string.IsNullOrWhiteSpace(GetInventoryClass(code)))
                    return string.Format("存货分类编码有误,上级编码不存在!", entity.cInvCCode);
            }
            #endregion
            string id = InsertInventoryClass(entity, code);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }
        public string InsertInventoryClass(EntityInventoryClass entity,string code)
        {
            if (entity == null)
            {
                throw new JRException("存货大类新增失败!没有数据!");
            }
            string sql = string.Format(@"insert into InventoryClass(cInvCCode,cInvCName,iInvCGrade,cBarCode)
                                         values(@cInvCCode,@cInvCName,@iInvCGrade,NULL);
                                         update InventoryClass set bInvCEnd=0 where cInvCCode=@code");
            SqlParameter[] para = { 
                                      new SqlParameter("@cInvCCode",entity.cInvCCode),
                                      new SqlParameter("@cInvCName",entity.cInvCName),
                                      new SqlParameter("@iInvCGrade",entity.iInvCGrade),
                                      new SqlParameter("@code",code),
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return entity.cInvCCode;
        }
        /// <summary>
        /// 获取存货分类编码规则
        /// </summary>
        /// <returns></returns>
        public string GetGradeDef()
        {
            string rule = string.Empty;
            string sql = string.Format(@"select top 1 CODINGRULE from GradeDef where keyword=N'Inventoryclass'");
            DataTable dt = DB_SqlHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rule = dt.Rows[0]["CODINGRULE"].ToString();
            }
            return rule;
        }
        /// <summary>
        /// 编码规则分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<int> StringSpilt(string str)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < str.Length; i++)
            {              
                int temp = Convert.ToInt32(str[i].ToString());
                if (list.Count > 0) temp += list.Last();
                list.Add(temp);
            }
            return list;
        }
    }
}
