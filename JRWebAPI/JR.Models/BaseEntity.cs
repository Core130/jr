using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JR.Models
{
    public class BaseEntity
    {
        /// <summary>
        /// 验证必填项
        /// </summary>
        /// <param name="be"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckEntity(out string msg)
        {
            msg = "";
            bool result = true;
            Type type = this.GetType();
            foreach (var item in type.GetProperties())
            {
                object[] objAttr = item.GetCustomAttributes(typeof(EntityCheckAttribute), true);

                if (objAttr != null && objAttr.Length > 0)
                {
                    EntityCheckAttribute ecAttr = (EntityCheckAttribute)objAttr[0];
                    if (ecAttr != null)
                    {
                        if (ecAttr.IsMust)
                        {
                            object val = item.GetValue(this,null);
                            if (val == null)
                            {
                                msg = string.Format("【{0}】必须传值，不能为空", ecAttr.Name);
                                return false;
                            }

                            Type valType = val.GetType();
                            switch (valType.Name.ToLower())
                            {
                                case "datetime":
                                    DateTime dt = (DateTime)val;
                                    if (dt.Year == 1 && dt.Month == 1 && dt.Day == 1)
                                    {
                                        msg = string.Format("【{0}】必须传值，不能为空", ecAttr.Name);
                                        return false;
                                    }
                                    break;
                                case "string":
                                    if (string.IsNullOrWhiteSpace(val.ToString()))
                                    {
                                        msg = string.Format("【{0}】必须传值，不能为空", ecAttr.Name);
                                        return false;
                                    }
                                    break;
                                case "int":
                                    break;
                                case "decimal":
                                case "float":
                                    break;

                            }

                        }
                    }
                }

            }


            return result;
        }

    }
}





