using JR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using JR.DAL;
using JR.HL.Security;

namespace JR.BLL
{
    /// <summary>
    /// U8业务基类
    /// </summary>
    public class U8BllBase
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        protected string DBName { get; set; }

        /// <summary>
        /// 账套号
        /// </summary>
        protected string U8AccID { get; set; }

        /// <summary>
        /// DB操作对象
        /// </summary>
        protected SqlHelper DB_SqlHelper;



        /// <summary>
        /// 初始化基类
        /// </summary>
        /// <param name="accID">账套号</param>
        /// <param name="beginYear">开始年度</param>
        /// <param name="userID">U8用户名</param>
        /// <param name="password">U8密码（秘文）</param>
        public U8BllBase(string accID, int beginYear, string userID, string password)
        {
            U8AccID = accID;
            DBName = U8DBHelp.GetAccountDataBase(accID, new SqlHelper());
            DB_SqlHelper = new SqlHelper();
            DB_SqlHelper.SQL_DATA_BASE = DBName;

            string msg = "";
            if (!U8Login(userID, password, out msg))
            {
                throw new Exception(msg);
            }
        }
        /// <summary>
        /// 通过账套号获取起始年度
        /// </summary>
        /// <param name="accID"></param>
        /// <returns></returns>
        public static int GetBeginYear(string accID)
        {
            string SQL = "select ISNULL(MAX(iYear),0) iYear from UA_Account where cAcc_Id = @cAcc_Id";
            SqlParameter[] para = { 
                                      new SqlParameter("@cAcc_Id",accID)
                                  };

            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.SQL_DATA_BASE = "UFSystem";
            DataTable dt = sqlhelp.ExecuteDataTable(SQL, para);
            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["iYear"].ToString());
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 验证值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        protected object GetDBValue(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return DBNull.Value;
            }
            else
            {
                return str;
            }
        }

        protected object SqlNull(object obj)
        {
            if (obj == null)
                return DBNull.Value;
            return obj;
        }

        public decimal ConvertToDecimal(object obj)
        {
            if (obj == DBNull.Value)
                return 0;
            return Convert.ToDecimal(obj);
        }

        /// <summary>
        /// 登录到U8
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="password">密码(明文)</param>
        /// <param name="message">返回错误消息</param>
        /// <returns>登录成功标识</returns>
        public bool U8Login(string userID, string password, out string message)
        {
            message = "";
            bool loginSuccess = true;

            string sqlLogin = "SELECT cUser_Name,cPassword,nState FROM UA_User WHERE cUser_ID = @cUser_ID";
            SqlParameter[] para = { new SqlParameter("@cUser_ID", userID) };

            SqlHelper sqlhelp = new SqlHelper();
            sqlhelp.SQL_DATA_BASE = "UFSystem";
            DataTable dt = sqlhelp.ExecuteDataTable(sqlLogin, para);
            if (dt != null && dt.Rows.Count > 0)
            {
                // 校验密码
                string getPassword = dt.Rows[0]["cPassword"].ToString().Trim();
                SecurityPolicy ObjUFPassword = new SecurityPolicy();
                string strSecurityPassword = ObjUFPassword.EnPassWord(password.Trim());
                if (getPassword != strSecurityPassword)
                {
                    message = "密码不正确!";
                    loginSuccess = false;
                }

                // 校验状态
                string strState = dt.Rows[0]["nState"].ToString();
                if (!string.IsNullOrEmpty(strState) && strState != "0")
                {
                    message = "用户已停用!";
                    loginSuccess = false;
                }
            }
            else
            {
                message = "用户不存在!";
                loginSuccess = false;
            }


            return loginSuccess;
        }

        #region 验证

        /// <summary>
        /// 获取客户名称
        /// </summary>
        /// <param name="cCusCode"></param>
        /// <returns></returns>
        public string GetCustomerName(string cCusCode)
        {
            if (string.IsNullOrEmpty(cCusCode))
            {
                return string.Empty;
            }

            string sql = "select cCusName  from Customer where cCusCode = @cCusCode";
            SqlParameter[] para = { 
                                      new SqlParameter("@cCusCode",cCusCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCusName"].ToString().Trim();
            }
            return string.Empty;

        }
        /// <summary>
        /// 根据供应商编码获取供应商名称
        /// </summary>
        /// <param name="cVenCode"></param>
        /// <returns></returns>
        public string GetVendorName(string cVenCode)
        {
            if (string.IsNullOrEmpty(cVenCode))
            {
                return string.Empty;
            }

            string sql = "select cVenName  from Vendor where cVenCode = @cVenCode";
            SqlParameter[] para = { 
                                      new SqlParameter("@cVenCode",cVenCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cVenName"].ToString().Trim();
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取仓库名称
        /// </summary>
        /// <param name="cWhCode"></param>
        /// <returns></returns>

        public string GetWarehouseName(string cWhCode)
        {
            if (string.IsNullOrEmpty(cWhCode))
            {
                return string.Empty;
            }

            string sql = "select cWhName  from Warehouse where cWhCode = @cWhCode  ";
            SqlParameter[] para = { 
                                      new SqlParameter("@cWhCode",cWhCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cWhName"].ToString().Trim();
            }
            return string.Empty;

        }

        /// <summary>
        /// 获取部门名称
        /// </summary>
        /// <param name="cWhCode"></param>
        /// <returns></returns>

        public string GetDepartmentName(string cDepCode)
        {
            if (string.IsNullOrEmpty(cDepCode))
            {
                return string.Empty;
            }

            string sql = "select cDepName  from Department where bDepEnd = 1 and cDepCode = @cDepCode  ";
            SqlParameter[] para = { 
                                      new SqlParameter("@cDepCode",cDepCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cDepName"].ToString().Trim();
            }
            return string.Empty;

        }

        /// <summary>
        /// 获取零售单价（找“存货价格表列表”.批发价1）
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public decimal GetSA_InvPrice(string cInvCode)
        {
            if (string.IsNullOrEmpty(cInvCode))
            {
                return decimal.Zero;
            }

            string sql = "select iuprice1 from SA_InvPriceJustList where cinvcode = @cInvCode  ";
            SqlParameter[] para = { 
                                      new SqlParameter("@cInvCode",cInvCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                string price = dt.Rows[0]["iuprice1"].ToString().Trim();
                decimal dec_tmp;
                decimal.TryParse(price, out dec_tmp);
                return dec_tmp;
            }
            return decimal.Zero;

        }

        /// <summary>
        /// 获取存货档案参考成本 
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public decimal GetInvSPrice(string cInvCode)
        {
            if (string.IsNullOrEmpty(cInvCode))
            {
                return decimal.Zero;
            }

            string sql = "select iInvSPrice from inventory where cInvCode = @cInvCode  ";
            SqlParameter[] para = { 
                                      new SqlParameter("@cInvCode",cInvCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                string price = dt.Rows[0]["iInvSPrice"].ToString().Trim();
                decimal dec_tmp;
                decimal.TryParse(price, out dec_tmp);
                return dec_tmp;
            }
            return decimal.Zero;

        }

        /// <summary>
        /// 获取存货名称、条形码、装箱规格、是否启用批次管理
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public string GetInventoryName(string cInvCode, int kind = 1)
        {
            if (string.IsNullOrEmpty(cInvCode))
            {
                return string.Empty;
            }

            string sql = "select cInvName,isnull(cBarCode,'') as cBarCode,isnull(cPackingType,'') as cPackingType ,bInvBatch from Inventory where cInvCode = @cInvCode  ";
            SqlParameter[] para = { 
                                      new SqlParameter("@cInvCode",cInvCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                if (kind == 1)
                {
                    return dt.Rows[0]["cInvName"].ToString().Trim();
                }
                else if (kind == 2)
                {
                    return dt.Rows[0]["cBarCode"].ToString().Trim();
                }
                else if (kind == 3)
                {
                    return dt.Rows[0]["cPackingType"].ToString().Trim();
                }
                else if (kind == 4)
                {
                    return Convert.ToBoolean(dt.Rows[0]["bInvBatch"]) ? "1" : "0";
                }
            }


            return string.Empty;

        }

        public List<EntityInventory> GetInventorys(string cInvCode)
        {
            var result = new List<EntityInventory>();
            if (string.IsNullOrEmpty(cInvCode))
            {
                return result;
            }

            string sql = "select cInvCode,cInvName,isnull(cBarCode,'') as cBarCode,isnull(cPackingType,'') as cPackingType ,bInvBatch,bInvType from Inventory where cInvCode in (" + cInvCode + ")";
            //SqlParameter[] para = { 
            //                          new SqlParameter("@cInvCode",cInvCode),
            //                      };
            SqlDataReader dataReader = this.DB_SqlHelper.ExecuteReader(CommandType.Text, sql);
            while (dataReader.Read())
            {
                var entity = new EntityInventory();
                entity.cInvCode = dataReader["cInvCode"].ToString();
                entity.cInvName = dataReader["cInvName"].ToString();
                entity.cBarCode = dataReader["cBarCode"].ToString();
                entity.cPackingType = dataReader["cPackingType"].ToString();
                entity.bInvBatch = Convert.ToBoolean(dataReader["bInvBatch"]) ? "1" : "0";
                entity.bInvType = Convert.ToBoolean(dataReader["bInvType"]) ? "1" : "0";
                result.Add(entity);
            }
            dataReader.Close();
            return result;

        }

        /// <summary>
        /// 获取末级科目
        /// </summary>
        /// <param name="cCode"></param>
        /// <param name="iYear"></param>
        /// <returns></returns>
        public string GetcCode(string cCode, int iYear)
        {
            if (string.IsNullOrEmpty(cCode))
            {
                return string.Empty;
            }

            string sql = "select cCode from code where bend=1 and ccode = @cCode and iyear = @iYear ";
            SqlParameter[] para = { 
                                       new SqlParameter("@cCode",cCode),     
                                       new SqlParameter("@iYear",iYear),  
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCode"].ToString().Trim();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取收发类别档案名称
        /// </summary>
        /// <param name="cRdCode"> 收发类别编码</param>
        /// <param name="bRdFlag"> 收发标识 1-收 0-发</param>
        /// <returns></returns>
        public string GetRdStyleName(string cRdCode, int bRdFlag = 0)
        {
            if (string.IsNullOrEmpty(cRdCode))
            {
                return string.Empty;
            }

            string sql = "select cRdName from Rd_Style where bRdFlag = @bRdFlag and bRdEnd = 1 and cRdCode = @cRdCode  ";
            SqlParameter[] para = { 
                                       new SqlParameter("@bRdFlag",bRdFlag),
                                      new SqlParameter("@cRdCode",cRdCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cRdName"].ToString().Trim();
            }
            return string.Empty;

        }


        /// <summary>
        /// 获取结算方式名称
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public string GetSettleStyleName(string cSSCode)
        {
            if (string.IsNullOrEmpty(cSSCode))
            {
                return string.Empty;
            }

            string sql = "select cSSName from SettleStyle where cSSCode = @cSSCode  ";
            SqlParameter[] para = { 
                                      new SqlParameter("@cSSCode",cSSCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cSSName"].ToString().Trim();
            }
            return string.Empty;

        }
        /// <summary>
        ///  获取操作员名称
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public string GetUserName(string UserCode)
        {
            if (string.IsNullOrEmpty(UserCode))
            {
                return string.Empty;
            }

            string sql = "select cUser_Name from UFSystem..UA_User where cUser_Id=@UserCode ";
            SqlParameter[] para = {
                                      new SqlParameter("@UserCode",UserCode),
                                  };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, para);

            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cUser_Name"].ToString().Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// 创建订货平台ID字段（如果存在则不操作）
        /// </summary>
        protected void CreateDHColumn(string tableName)
        {
            string sql = string.Format(@"
                if NOT exists(select * from syscolumns where id=object_id('{0}') and name='DHID')
                BEGIN
	                ALTER TABLE {0} ADD DHID nvarchar(60) 
                END
                ", tableName);
            this.DB_SqlHelper.ExecuteNonQuery(sql, null);

        }

        /// <summary>
        /// 获取主从表ID
        /// </summary>
        /// <param name="remoteid">前缀ID,必须是数字</param>
        /// <param name="cVouchType">单据类型</param>
        /// <param name="detailCount">从表数量</param>
        /// <param name="headId">主表ID</param>
        /// <param name="childEndId">从表结束ID</param>
        protected void GetId(string remoteid, string cVouchType, int detailCount, out int headId, out int childEndId)
        {
            string SQL_StoredProcedure = "sp_getID";
            SqlParameter[] sp_para = new SqlParameter[6];
            sp_para[0] = new SqlParameter("@RemoteId", remoteid);
            sp_para[1] = new SqlParameter("@cAcc_Id", U8AccID);
            sp_para[2] = new SqlParameter("@cVouchType", cVouchType);
            sp_para[3] = new SqlParameter("@iAmount", detailCount);
            sp_para[4] = new SqlParameter("@iFatherId", SqlDbType.Int);
            sp_para[4].Direction = ParameterDirection.Output;

            sp_para[5] = new SqlParameter("@iChildId", SqlDbType.Int);
            sp_para[5].Direction = ParameterDirection.Output;
            this.DB_SqlHelper.ExecuteNonQuery(CommandType.StoredProcedure, SQL_StoredProcedure, sp_para);
            headId = int.Parse(sp_para[4].Value.ToString());
            childEndId = int.Parse(sp_para[5].Value.ToString());
        }

        /// <summary>
        /// 获取结算方式档案
        /// </summary>
        /// <returns></returns>
        public List<EntitySettleStyle> GetSettleStyle()
        {
            List<EntitySettleStyle> list = new List<EntitySettleStyle>();
            string sql = @"select cSSCode,cSSCode+'_'+cSSName as cSSName from SettleStyle where bSSEnd=1";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                EntitySettleStyle ess = new EntitySettleStyle();
                ess.cSSCode = datareader["cSSCode"].ToString();
                ess.cSSName = datareader["cSSName"].ToString();
                list.Add(ess);
            }
            return list;
        }
        /// <summary>
        /// 获取入库类别（收发类别档案）
        /// </summary>
        /// <returns></returns>
        public List<Entityrd_Style> Getrd_Style()
        {
            List<Entityrd_Style> list = new List<Entityrd_Style>();
            string sql = @"select cRdCode,cRdCode+'_'+cRdName as cRdName from rd_Style where bRdFlag=1 and bRdEnd=1";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                Entityrd_Style es = new Entityrd_Style();
                es.cRdCode = datareader["cRdCode"].ToString();
                es.cRdName = datareader["cRdName"].ToString();
                list.Add(es);
            }
            return list;
        }

        /// <summary>
        /// 获取客户档案
        /// </summary>
        /// <returns></returns>
        public List<EntityCustomer> GetCustomer(string ccuscode)
        {
            List<EntityCustomer> list = new List<EntityCustomer>();
            string sql = string.Empty;
            DataTable dt = new DataTable();
            if (string.IsNullOrEmpty(ccuscode))
            {
                sql = @"select cCusCode,cCusCode+'_'+cCusName as cCusName,cCusDepart,cCusDefine1,cCusDepart+'_'+d.cDepName as cCusDepName,cCCCode,cCusWhCode,cMemo from Customer c 
                           left join Department d on c.cCusDepart=d.cDepCode where (dEndDate >= getdate() or dEndDate is null) ";
                dt = this.DB_SqlHelper.ExecuteDataTable(sql);
            }
            else
            {
                sql = @"select cCusCode,cCusCode+'_'+cCusName as cCusName,cCusDepart,cCusDefine1,cCusDepart+'_'+d.cDepName as cCusDepName,cCCCode,cCusWhCode,cMemo from Customer c 
                           left join Department d on c.cCusDepart=d.cDepCode where (dEndDate >= getdate() or dEndDate is null) and cCusCode=@cCusCode";

                SqlParameter[] paras = {
                                       new SqlParameter("@cCusCode",ccuscode),                                    
                                   };
                dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    EntityCustomer ec = new EntityCustomer();
                    ec.cCusCode = dt.Rows[i]["cCusCode"].ToString();
                    ec.cCusName = dt.Rows[i]["cCusName"].ToString();
                    ec.cCusDepart = dt.Rows[i]["cCusDepart"].ToString();
                    ec.cCusDepName = dt.Rows[i]["cCusDepName"].ToString();
                    ec.cCusDefine1 = dt.Rows[i]["cCusDefine1"].ToString();
                    ec.cCCCode = dt.Rows[i]["cCCCode"].ToString();
                    ec.cCusWhCode = dt.Rows[i]["cCusWhCode"].ToString();
                    ec.cMemo = dt.Rows[i]["cMemo"].ToString();
                    list.Add(ec);
                }
            }
            return list;
        }
        /// <summary>
        /// 获取客户档案是否存在
        /// </summary>
        /// <param name="cCusCode"></param>
        /// <returns></returns>
        public string IsGetCustomer(string cCusCode)
        {
            string sql = "select cCusCode from Customer where cCusCode=@cCusCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCusCode",cCusCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCusCode"].ToString();
            }
            return null;
        }
        /// <summary>
        /// 获取客户分类是否存在
        /// </summary>
        /// <param name="cCCCode"></param>
        /// <returns></returns>
        public string GetCustomerClass(string cCCCode)
        {
            string sql = "select cCCCode  from CustomerClass where bCCEnd = 1 AND cCCCode =@cCCCode ";
            SqlParameter[] paras = {
                                       new SqlParameter("@cCCCode ",cCCCode )
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCCCode"].ToString();
            }
            return null;
        }

        public string GetCustomerProperty(string cuscode)
        {
            string sql = string.Format(@"select cCusDefine1 from Customer where cCusCode='{0}'", cuscode);
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCusDefine1"].ToString();
            }
            return string.Empty;
        }

        public string GetCustomercCusAbbName(string cCusAbbName, string cCusCode)
        {
            string sql = string.Format(@"select cCusAbbName from Customer where cCusAbbName='{0}' AND cCusCode <> '{1}'", cCusAbbName, cCusCode);
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCusAbbName"].ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取仓库档案
        /// </summary>
        /// <returns></returns>
        public List<EntityWareHouse> GetWareHouse()
        {
            List<EntityWareHouse> list = new List<EntityWareHouse>();
            string sql = @"select cWhCode,cWhCode+'_'+cWhName as cWhName from WareHouse where dWhEndDate>=GETDATE() or dWhEndDate is null";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                EntityWareHouse ew = new EntityWareHouse();
                ew.cWhCode = datareader["cWhCode"].ToString();
                ew.cWhName = datareader["cWhName"].ToString();
                list.Add(ew);
            }
            return list;
        }
        /// <summary>
        /// 获取仓库档案是否存在
        /// </summary>
        /// <param name="cWhCode"></param>
        /// <returns></returns>
        public string GetWareHouse(string cWhCode)
        {
            string sql = "select cWhCode from WareHouse where cWhCode=@cWhCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cWhCode",cWhCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cWhCode"].ToString();
            }
            return null;
        }      
        /// <summary>
        /// 获取存货档案+'_'+cInvName as cInvCode
        /// </summary>
        /// <returns></returns>
        public List<EntityInventory> GetInventory(DateTime dModifyDate )
        {
            List<EntityInventory> list = new List<EntityInventory>();
            string sql = @"select cInvCode,cInvName,cInvStd from Inventory where (dSDate > getdate() and  dEDate < getdate() or dEDate is null)  and convert(nvarchar(10),dModifyDate,121)>= '" + dModifyDate.ToString("yyyy-MM-dd")+"'";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                EntityInventory ei = new EntityInventory();
                ei.cInvCode = datareader["cInvCode"].ToString();
                ei.cInvName = datareader["cInvName"].ToString();
                ei.cInvStd = datareader["cInvStd"].ToString();
                list.Add(ei);
            }
            return list;
        }
        /// <summary>
        ///无参数
        /// </summary>
        /// <returns></returns>
        public List<EntityInventory> GetInventory()
        {
            List<EntityInventory> list = new List<EntityInventory>();
            string sql = @"select cInvCode,cInvName,cInvStd from Inventory where (dSDate > getdate() and  dEDate < getdate() or dEDate is null)  ";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                EntityInventory ei = new EntityInventory();
                ei.cInvCode = datareader["cInvCode"].ToString();
                ei.cInvName = datareader["cInvName"].ToString();
                ei.cInvStd = datareader["cInvStd"].ToString();
                list.Add(ei);
            }
            return list;
        }
        /// <summary>
        /// 获取存货是否存在
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public string GetInventory(string cInvCode)
        {
            string sql = "select cInvCode from Inventory where cInvCode=@cInvCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cInvCode",cInvCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cInvCode"].ToString();
            }
            return null;
        }
        /// <summary>
        /// 存货是否折扣属性
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public int GetbInvType(string cInvCode)
        {
            string sql = "SELECT bInvType FROM Inventory WHERE cInvCode = @cInvCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cInvCode",cInvCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                var bInv = dt.Rows[0]["bInvType"];
                return Convert.ToInt32(bInv);
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取采购类型档案
        /// </summary>
        /// <returns></returns>
        public List<EntityPurchaseType> GetPurchaseType()
        {
            List<EntityPurchaseType> list = new List<EntityPurchaseType>();
            string sql = @"select cPTCode,cPTCode+'_'+cPTName as cPTName from PurchaseType";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                EntityPurchaseType ept = new EntityPurchaseType();
                ept.cPTCode = datareader["cPTCode"].ToString();
                ept.cPTName = datareader["cPTName"].ToString();
                list.Add(ept);
            }
            return list;
        }
        /// <summary>
        /// 获取本单位银行对应结算科目
        /// </summary>
        /// <param name="cSettleStyle">结算方式</param>
        /// <param name="cbaccount">本单位银行账号</param>
        /// <param name="iYear">年度</param>
        /// <returns></returns>
        public string GetcSSCode(string cSettleStyle, string cBaccount, string sYear)
        {
            string sql = " SELECT cCode FROM ap_sstylecode WHERE cSettleStyle = @cSettleStyle AND cBAccount = @cBAccount AND iYear = @iYear";
            SqlParameter[] paras = {
                                       new SqlParameter("@cSettleStyle",cSettleStyle),
                                       new SqlParameter("@cBAccount",cBaccount),
                                       new SqlParameter("@iYear",sYear),
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cCode"].ToString(); ;
            }
            return null;
        }
        /// <summary>
        /// 获取供应商分类
        /// </summary>
        /// <param name="cvccode"></param>
        /// <returns></returns>
        public string GetcVcCode(string cvccode)
        {
            string sql = " select cVcCode from Vendorclass where BVCEnd=1 and CVCCode =@cvccode ";
            SqlParameter[] paras = {
                                       new SqlParameter("@cvccode",cvccode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cVcCode"].ToString();
            }
            return null;
        }
        /// <summary>
        /// 获取存货分类
        /// </summary>
        /// <param name="cInvCCode"></param>
        /// <returns></returns>
        public string GetInventoryClass(string cInvCCode, bool bEnd = false)
        {
            string sql = "select cInvCCode from InventoryClass where cInvCCode=@cInvCCode";
            if (bEnd) sql = "select cInvCCode from InventoryClass where bInvCEnd = 1 AND cInvCCode=@cInvCCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cInvCCode",cInvCCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cInvCCode"].ToString();
            }
            return null;
        }
        /// <summary>
        /// 获取供应商档案
        /// </summary>
        /// <returns></returns>
        public List<EntityVendor> GetVendor()
        {
            List<EntityVendor> list = new List<EntityVendor>();
            string sql = @"select cVenCode,cVenCode+'_'+cVenName as cVenName from Vendor where dEndDate >= getdate() or dEndDate is null";
            SqlDataReader datareader = DB_SqlHelper.ExecuteReader(CommandType.Text, sql, null);
            while (datareader.Read())
            {
                EntityVendor ev = new EntityVendor();
                ev.cVenCode = datareader["cVenCode"].ToString();
                ev.cVenName = datareader["cVenName"].ToString();
                list.Add(ev);
            }
            return list;
        }
        /// <summary>
        /// 获取U8单据类型与收发类别对照表中出库和入库类别
        /// </summary>
        /// <param name="cVBTID">单据编号12：调拨单、15：形态转换单、18：盘点单、62：调拨申请单</param>
        /// <param name="bRDFlag">收发标识1：入库 、2：出库</param>
        /// <returns></returns>
        public string GetRdCode(int cId, int bRDFlag)
        {
            string sql = @"select VouchRdContrapose.cVBTID,cVTChName,cBTChName,cVRRCode,cVRSCode
                        from VouchRdContrapose left join vouchTypeDic on VouchRdContrapose.cVBTID=VouchTypeDic.cVBTID where VouchRdContrapose.cVBTID = @cVBTID";
            SqlParameter[] paras = {
                                       new SqlParameter("@cVBTID",cId)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Format("请配置U8单据类型与收发类别对照表");
            }
            else
            {
                string cRdCode = bRDFlag == 1 ? dt.Rows[0]["cVRRCode"].ToString() : dt.Rows[0]["cVRSCode"].ToString();
                return cRdCode;
            }
        }
        /// <summary>
        /// 获取销售订单是否存在
        /// </summary>
        /// <param name="cSOCode"></param>
        /// <returns></returns>
        public int GetSoMain(string cSOCode)
        {
            int num = 0;
            string cmdText = "select count(*) as Num from SO_SOMain where cSOCode=@cSOCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cSOCode",cSOCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(cmdText, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                num = int.Parse(dt.Rows[0]["Num"].ToString());
            }

            return num;
        }
        /// <summary>
        /// 获取存货入库含税单价
        /// </summary>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public EntityInventoryPrice GetInventoryPrice(string cVenCode, string cInvCode)
        {
            EntityInventoryPrice result = new EntityInventoryPrice();

            string sql = string.Format(@"select cInvCode,case when sum(noSettleTaxCost)<> 0 then  sum(noSettleTaxCost) else  sum(SettleTaxCost) end 
                                        as noSettleTaxCost,sum(SettleTaxCost) as SettleTaxCost ,SUM(venInvPrice) AS venInvPrice from 
                                        (select cInvCode,noSettleTaxCost,SettleTaxCost,venInvPrice from 
                                        (select top 1 rds.cInvCode, isnull(iOriTaxCost,0.00) as noSettleTaxCost ,0.00 as SettleTaxCost,0.00 as venInvPrice  from rdrecords01 rds
                                        inner join rdrecord01 rd on rds.ID = rd.ID
                                        where rds.dSDate is null AND  rds.cInvCode=	@cInvCode AND rd.cVenCode=@cVenCode
                                        order by rd.dDate asc) a
                                        union 
                                        select * from 
                                        (select top 1 rds.cInvCode,0.00 as noSettleTaxCost , isnull(iOriTaxCost,0.00) as SettleTaxCost,0.00 as venInvPrice from rdrecords01 rds
                                        inner join rdrecord01 rd on rds.ID = rd.ID
                                        where rds.dSDate is not null AND  rds.cInvCode=@cInvCode AND rd.cVenCode=@cVenCode
                                        order by rds.dSDate desc) b 
                                        UNION
                                        SELECT * FROM  
                                        (SELECT TOP 1 cInvCode,0.00 as noSettleTaxCost ,0.00 as SettleTaxCost, ISNULL(iTaxUnitPrice,0.00) AS venInvPrice FROM  Ven_Inv_Price WHERE
                                        GETDATE()>=dEnableDate AND (dDisableDate IS NULL OR  dDisableDate<=GETDATE()) AND 
                                        cInvCode= @cInvCode AND cVenCode=@cVenCode 
                                        ORDER BY  dEnableDate DESC  ) c ) T
                                        group by cInvCode");
            SqlParameter[] paras = {
                                       new SqlParameter("@cInvCode",cInvCode),
                                       new SqlParameter("@cVenCode",cVenCode)
                                   };

            SqlDataReader datareader = this.DB_SqlHelper.ExecuteReader(CommandType.Text, sql, paras);
            if (datareader.Read())
            {
                result.cVenCode = cVenCode;
                result.cInvCode = datareader["cInvCode"].ToString();
                result.noSettleTaxCost = Convert.ToDecimal(datareader["noSettleTaxCost"]);
                result.SettleTaxCost = Convert.ToDecimal(datareader["SettleTaxCost"]);
                result.VenInvPrice = Convert.ToDecimal(datareader["venInvPrice"]);
            }
            return result;

        }

        /// <summary>
        /// 获取结账标识
        /// </summary>
        /// <param name="iYear"></param>
        /// <param name="iMonth"></param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        public bool GetGlmendFlag(int iYear, int iMonth, string field)
        {
            string sql = string.Format(@"select {0} from GL_mend where iyear=@iyear and iperiod=@iperiod", field);
            SqlParameter[] para1 = {
                                       new SqlParameter("@iyear",iYear),
                                       new SqlParameter("@iperiod",iMonth),
                                   };
            var result = this.DB_SqlHelper.ExecuteScalar(CommandType.Text, sql, para1);
            return result == null ? false : Convert.ToBoolean(result);
        }
        /// <summary>
        /// 获取销售类型
        /// </summary>
        /// <param name="cSTCode"></param>
        /// <returns></returns>
        public string GetStName(string cSTCode)
        {
            string sql = "select cSTName from SaleType  where cSTCode=@cSTCode";
            SqlParameter[] paras = {
                                       new SqlParameter("@cSTCode",cSTCode)
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cSTName"].ToString();
            }
            return null;
        }
        /// <summary>
        /// 获取现存量处理SQL语句,kind=-1用于删除单据时扣减现存量数据
        /// </summary>
        /// <param name="cVouchType">单据类型</param>
        /// <param name="kind">1：新增、-1 ：撤销</param>
        /// <returns></returns>
        public string GetCurrentStockSql(string cVouchType, int kind = 1)
        {
            string sql = "";
            if (cVouchType == "Dis")
                sql = string.Format(@"IF EXISTS(SELECT 1 FROM CurrentStock WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and ISNULL(cBatch,'') = ISNULL(@cBatch,''))
                                  BEGIN
                                    IF @iQuantity > 0 
                                    Update CurrentStock set fOutQuantity = fOutQuantity + @iQuantity * {0}
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and ISNULL(cBatch,'') =  ISNULL(@cBatch,'')
                                    ELSE
                                    Update CurrentStock set fInQuantity = fInQuantity - @iQuantity * {0}
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and ISNULL(cBatch,'') =  ISNULL(@cBatch,'');
                                  END
                                  ELSE IF NOT EXISTS (SELECT 1 FROM Inventory WHERE bInvType = 1 AND  cInvCode = @cInvCode)
                                  BEGIN
                                    INSERT  INTO  CurrentStock(cwhcode,cinvcode,itemId,cbatch,cVMIVenCode,isotype,isodid,iquantity,inum,
                                    fstopquantity,fstopnum,fdisablequantity,fdisablenum,finquantity,finnum,ftransinquantity,  
                                    ftransinnum,foutquantity,foutnum,ftransoutquantity,ftransoutnum,cfree1,cfree2,cfree3,cfree4,cfree5,  
                                    cfree6,cfree7,cfree8,cfree9,cfree10,dmdate,dvdate,cmassunit,imassdate,cExpirationdate,dExpirationdate,iExpiratDateCalcu)
                                    VALUES (@cWhCode,@cInvCode,'',ISNULL(@cBatch,''),'',0,'',0,0,0,0,0,0,0,0,0,0,@iQuantity * {0},0,0,0,'','','','','','','','','','',NULL,NULL,0,NULL,NULL,NULL,0);
                                   
                                    IF NOT EXISTS (SELECT 1 FROM SCM_Item WHERE cInvCode=@cInvCode)
                                    INSERT INTO [SCM_Item]
                                    ([cInvCode],[cFree1],[cFree2],[cFree3],[cFree4],[cFree5],[cFree6],[cFree7],[cFree8],[cFree9],[cFree10],[PartId])
                                    VALUES(@cInvCode,'','','','','','','','','','',0);
                                    UPDATE CurrentStock SET ItemID = (SELECT TOP 1 ID FROM SCM_Item WHERE cInvCode =CurrentStock.cInvCode) 
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'');
                                  END ;", kind);
            if (cVouchType == "Rd01")
                sql = string.Format(@"IF EXISTS(SELECT * FROM AccInformation WHERE cSysID='ST' AND cName='bPurchaseInCheck' AND cValue='True')
                                    BEGIN
                                    IF EXISTS(SELECT 1 FROM CurrentStock WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') = ISNULL(@cBatch,''))
                                    BEGIN                    
                                    Update CurrentStock set fInQuantity = fInQuantity + @iQuantity * {0}
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') = ISNULL(@cBatch,'')     
                                    END
                                    ELSE
                                    BEGIN
                                    INSERT  INTO  CurrentStock(cwhcode,cinvcode,itemId,cbatch,cVMIVenCode,isotype,isodid,iquantity,inum,  
                                    fstopquantity,fstopnum,fdisablequantity,fdisablenum,finquantity,finnum,ftransinquantity,  
                                    ftransinnum,foutquantity,foutnum,ftransoutquantity,ftransoutnum,cfree1,cfree2,cfree3,cfree4,cfree5,  
                                    cfree6,cfree7,cfree8,cfree9,cfree10,dmdate,dvdate,cmassunit,imassdate,cExpirationdate,dExpirationdate,iExpiratDateCalcu)
                                    VALUES (@cWhCode,@cInvCode,'',ISNULL(@cBatch,''),'',0,'',0,0,0,0,0,0,@iQuantity * {0},0,0,0,0,0,0,0,'','','','','','','','','','',NULL,NULL,0,NULL,NULL,NULL,0);
                                    IF NOT EXISTS (SELECT 1 FROM SCM_Item WHERE cInvCode=@cInvCode)
                                    INSERT INTO [SCM_Item]
                                    ([cInvCode],[cFree1],[cFree2],[cFree3],[cFree4],[cFree5],[cFree6],[cFree7],[cFree8],[cFree9],[cFree10],[PartId])
                                    VALUES(@cInvCode,'','','','','','','','','','',0);
                                    UPDATE CurrentStock SET ItemID = (SELECT TOP 1 ID FROM SCM_Item WHERE cInvCode =CurrentStock.cInvCode) WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'');
                                    END
                                    END
                                    IF EXISTS(SELECT * FROM AccInformation WHERE cSysID='ST' AND cName='bPurchaseInCheck' AND cValue='False')
                                    BEGIN
                                    IF EXISTS(SELECT 1 FROM CurrentStock WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') = ISNULL(@cBatch,''))
                                    BEGIN                    
                                    Update CurrentStock set iQuantity = iQuantity + @iQuantity * {0}
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'')      
                                    END
                                    ELSE
                                    BEGIN
                                    INSERT  INTO  CurrentStock(cwhcode,cinvcode,itemId,cbatch,cVMIVenCode,isotype,isodid,iquantity,inum,  
                                    fstopquantity,fstopnum,fdisablequantity,fdisablenum,finquantity,finnum,ftransinquantity,  
                                    ftransinnum,foutquantity,foutnum,ftransoutquantity,ftransoutnum,cfree1,cfree2,cfree3,cfree4,cfree5,  
                                    cfree6,cfree7,cfree8,cfree9,cfree10,dmdate,dvdate,cmassunit,imassdate,cExpirationdate,dExpirationdate,iExpiratDateCalcu)
                                    VALUES (@cWhCode,@cInvCode,'',ISNULL(@cBatch,''),'',0,'',@iQuantity * {0},0,0,0,0,0,0,0,0,0,0,0,0,0,'','','','','','','','','','',NULL,NULL,0,NULL,NULL,NULL,0);
                                    IF NOT EXISTS (SELECT 1 FROM SCM_Item WHERE cInvCode=@cInvCode)
                                    INSERT INTO [SCM_Item]
                                    ([cInvCode],[cFree1],[cFree2],[cFree3],[cFree4],[cFree5],[cFree6],[cFree7],[cFree8],[cFree9],[cFree10],[PartId])
                                    VALUES(@cInvCode,'','','','','','','','','','',0);                                                        
                                    UPDATE CurrentStock SET ItemID = (SELECT TOP 1 ID FROM SCM_Item WHERE cInvCode =CurrentStock.cInvCode) WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and ISNULL(cBatch,'') =  ISNULL(@cBatch,'');
                                    END
                                    END;", kind);
            if (cVouchType == "Rd09")
                sql = string.Format(@"IF EXISTS(SELECT * FROM AccInformation WHERE cSysID='ST' AND cName='bOtherInCheck' AND cValue='True')
                                    BEGIN
                                    IF EXISTS(SELECT 1 FROM CurrentStock WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') = ISNULL(@cBatch,''))
                                    BEGIN                    
                                    Update CurrentStock set fOutQuantity = fOutQuantity + case when @iQuantity >0 THEN @iQuantity * {0} ELSE 0 END ,fInQuantity = fInQuantity + case when @iQuantity >0 THEN 0 ELSE -@iQuantity * {0} END
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'')      
                                    END
                                    ELSE
                                    BEGIN
                                    INSERT  INTO  CurrentStock(cwhcode,cinvcode,itemId,cbatch,cVMIVenCode,isotype,isodid,iquantity,inum,  
                                    fstopquantity,fstopnum,fdisablequantity,fdisablenum,finquantity,finnum,ftransinquantity,  
                                    ftransinnum,foutquantity,foutnum,ftransoutquantity,ftransoutnum,cfree1,cfree2,cfree3,cfree4,cfree5,  
                                    cfree6,cfree7,cfree8,cfree9,cfree10,dmdate,dvdate,cmassunit,imassdate,cExpirationdate,dExpirationdate,iExpiratDateCalcu)
                                    VALUES (@cWhCode,@cInvCode,'',ISNULL(@cBatch,''),'',0,'',0,0,0,0,0,0,case when @iQuantity >0 THEN 0 ELSE -@iQuantity * {0} END,0,0,0,case when @iQuantity >0 THEN @iQuantity * {0} ELSE 0 END ,0,0,0,'','','','','','','','','','',NULL,NULL,0,NULL,NULL,NULL,0);
                                    IF NOT EXISTS (SELECT 1 FROM SCM_Item WHERE cInvCode=@cInvCode)
                                    INSERT INTO [SCM_Item]
                                    ([cInvCode],[cFree1],[cFree2],[cFree3],[cFree4],[cFree5],[cFree6],[cFree7],[cFree8],[cFree9],[cFree10],[PartId])
                                    VALUES(@cInvCode,'','','','','','','','','','',0);                                                        
                                    UPDATE CurrentStock SET ItemID = (SELECT TOP 1 ID FROM SCM_Item WHERE cInvCode =CurrentStock.cInvCode) WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'');
                                    END
                                    END
                                    IF EXISTS(SELECT * FROM AccInformation WHERE cSysID='ST' AND cName='bOtherInCheck' AND cValue='False')
                                    BEGIN
                                    IF EXISTS(SELECT 1 FROM CurrentStock WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') = ISNULL(@cBatch,''))
                                    BEGIN                    
                                    Update CurrentStock set iQuantity = iQuantity - @iQuantity * {0}
                                    WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'')      
                                    END
                                    ELSE
                                    BEGIN
                                    INSERT  INTO  CurrentStock(cwhcode,cinvcode,itemId,cbatch,cVMIVenCode,isotype,isodid,iquantity,inum,  
                                    fstopquantity,fstopnum,fdisablequantity,fdisablenum,finquantity,finnum,ftransinquantity,  
                                    ftransinnum,foutquantity,foutnum,ftransoutquantity,ftransoutnum,cfree1,cfree2,cfree3,cfree4,cfree5,  
                                    cfree6,cfree7,cfree8,cfree9,cfree10,dmdate,dvdate,cmassunit,imassdate,cExpirationdate,dExpirationdate,iExpiratDateCalcu)
                                    VALUES (@cWhCode,@cInvCode,'',ISNULL(@cBatch,''),'',0,'',-@iQuantity * {0},0,0,0,0,0,0,0,0,0,0,0,0,0,'','','','','','','','','','',NULL,NULL,0,NULL,NULL,NULL,0);
                                    IF NOT EXISTS (SELECT 1 FROM SCM_Item WHERE cInvCode=@cInvCode)
                                    INSERT INTO [SCM_Item]
                                    ([cInvCode],[cFree1],[cFree2],[cFree3],[cFree4],[cFree5],[cFree6],[cFree7],[cFree8],[cFree9],[cFree10],[PartId])
                                    VALUES(@cInvCode,'','','','','','','','','','',0);                                                       
                                    UPDATE CurrentStock SET ItemID = (SELECT TOP 1 ID FROM SCM_Item WHERE cInvCode =CurrentStock.cInvCode) WHERE cWhCode = @cWhCode and cInvCode = @cInvCode and isnull(cBatch,'') =  ISNULL(@cBatch,'');
                                    END
                                    END;", kind);
            return sql;
        }

        /// <summary>
        /// 获取存货现存量的批次信息
        /// </summary>
        /// <param name="cWhCode"></param>
        /// <param name="cInvCode"></param>
        /// <returns></returns>
        public string GetInvBatch(string cWhCode, string cInvCode)
        {
            string sql = string.Format(@" SELECT cBatch FROM Currentstock WHERE cWhCode = @cWhCode AND cInvCode=@cInvCode
                                        AND ISNULL (bstopflag,0)=0 AND  isnull(bgspstop,0)=0 AND isnull(iquantity,0)>0
                                        ORDER  BY  isodid DESC,dvdate,autoid", cWhCode, cInvCode);
            SqlParameter[] paras = {
                                       new SqlParameter("@cWhCode",cWhCode),
                                       new SqlParameter("@cInvCode",cInvCode),
                                   };
            DataTable dt = this.DB_SqlHelper.ExecuteDataTable(sql, paras);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["cBatch"].ToString();
            }
            return null;
        }
        #endregion
    }
}
