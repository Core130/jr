using JR.HL;
using JR.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.BLL
{
    public class WareHouseBll : U8BllBase
    {
        public WareHouseBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }

        public string AddWareHouse(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityWareHouse entity, out int success)
        {
            success = 0;
            WareHouseBll bll = new WareHouseBll(StrAccID, AccYear, UserCode, PlainPassword);
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            // 部门 
            if (!string.IsNullOrWhiteSpace(entity.cDepCode))
            {
                string deptName = bll.GetDepartmentName(entity.cDepCode);
                if (string.IsNullOrWhiteSpace(deptName))
                {
                    return string.Format("U8不存在部门编码:{0},或者部门编码非末级!", entity.cDepCode);
                }
            }
            #endregion
            entity.cWhValueStyle = string.IsNullOrWhiteSpace(entity.cWhValueStyle) ? "全月平均法" : entity.cWhValueStyle;
            string id = "";
            if (!string.IsNullOrWhiteSpace(GetWareHouse(entity.cWhCode)))
            {
                id = UpdateWareHouse(entity);
            }
            else
            {
                id = InsertWareHouse(entity);
            }
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }
        /// <summary>
        /// 新增仓库档案
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string InsertWareHouse(EntityWareHouse entity)
        {
            if (entity == null)
            {
                throw new JRException("仓库档案新增失败!没有数据!");
            }
            string sql = string.Format(@"insert into WareHouse(cWhCode,cWhName,cDepCode,cWhAddress,cWhPhone,cWhPerson,cWhValueStyle,
                                        bWhPos,iWhFundQuota,cWhMemo,cBarCode,bMRP,bROP,iWHProperty,bShop,bControlSerial,bInCost,
                                        bInAvailCalcu,bProxyWh,iSAConMode,iEXConMode,iSTConMode,bBondedWh,bWhAsset,fWhQuota,
                                        dWhEndDate,bCheckSubitemCost,cPickPos,bEB,dModifyDate)
                                        values(@cWhCode,@cWhName,@cDepCode,@cWhAddress,@cWhPhone,@cWhPerson,@cWhValueStyle,
                                        0,NULL,NULL,@cWhCode,1,1,0,@bShop,1,1,
                                        1,0,0,0,0,0,0,NULL,
                                        NULL,1,NULL,0,GetDate())");
            SqlParameter[] para = { 
                                      new SqlParameter("@cWhCode",entity.cWhCode),
                                      new SqlParameter("@cWhName",entity.cWhName),
                                      new SqlParameter("@cDepCode",GetDBValue(entity.cDepCode)),
                                      new SqlParameter("@cWhPhone",entity.cWhPhone),
                                      new SqlParameter("@cWhPerson",entity.cWhPerson),
                                      new SqlParameter("@cWhAddress",entity.cWhAddress), 
                                      new SqlParameter("@bShop",entity.bShop.ToString()),
                                      new SqlParameter("@cWhValueStyle",entity.cWhValueStyle),
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (entity.cWhCode);
        }
        /// <summary>
        /// 更新仓库档案
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string UpdateWareHouse(EntityWareHouse entity)
        {
            if (entity == null)
            {
                throw new JRException("仓库档案更新失败!没有数据!");
            }
            string sql = string.Format(@"Update WareHouse Set cWhName=@cWhName,cWhPhone=@cWhPhone,cWhPerson=@cWhPerson,cWhAddress=@cWhAddress,cDepCode=@cDepCode
                                        Where cWhCode=@cWhCode");
            SqlParameter[] para = { 
                                      new SqlParameter("@cWhCode",entity.cWhCode),
                                      new SqlParameter("@cWhName",entity.cWhName),
                                      new SqlParameter("@cWhPhone",entity.cWhPhone),
                                      new SqlParameter("@cWhPerson",entity.cWhPerson),
                                      new SqlParameter("@cWhAddress",entity.cWhAddress), 
                                      new SqlParameter("@bShop",entity.bShop.ToString()),  
                                      new SqlParameter("@cDepCode",GetDBValue(entity.cDepCode)),
                                  };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (entity.cWhCode);
        }
    }
}
