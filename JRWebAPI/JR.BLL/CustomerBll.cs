using JR.HL;
using JR.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.BLL
{
    public class CustomerBll : U8BllBase
    {
        public CustomerBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        public string AddCustomer(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityCustomer entity, out int success)
        {
            success = 0;
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            if (string.IsNullOrWhiteSpace(GetCustomerClass(entity.cCCCode)))
            {
                return string.Format("U8中不存在客户分类编码或分类编码非末级:{0}!", entity.cCCCode);
            }
            if (string.IsNullOrWhiteSpace(GetDepartmentName(entity.cCusDepart)))
            {
                return string.Format("U8中不存在部门编码:{0},或者部门编码非末级!", entity.cCusDepart);
            }
            if (!string.IsNullOrWhiteSpace(GetCustomercCusAbbName(entity.cCusAbbName, entity.cCusCode)))
            {
                return string.Format("U8中已存在客户简称:{0}!", entity.cCusAbbName);
            }
            #endregion
            entity.cMaker = GetUserName(UserCode);
            string id = "";
            if (!string.IsNullOrWhiteSpace(IsGetCustomer(entity.cCusCode)))
            {
                id = UpdateCustomer(entity);
            }
            else
            {
                id = InsertCustomer(entity);
            }
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1; ;
            return id;
        }
        public string InsertCustomer(EntityCustomer entity)
        {
            if (entity == null)
            {
                throw new JRException("客户档案新增失败!没有数据!");
            }
            string sql = string.Format(@"insert into Customer(cCusCode,cCusName,cCusAbbName,cCCCode,cDCCode,cTrade,
cCusAddress,cCusPostCode,cCusRegCode,dCusDevDate,cCusLPerson,cCusEmail,cCusPerson,cCusPhone,cCusFax,cCusBP,cCusHand,
cCusPPerson,iCusDisRate,cCusCreGrade,iCusCreLine,iCusCreDate,cCusPayCond,cCusOType,cCusHeadCode,cCusWhCode,cCusDepart,
iARMoney,dLastDate,iLastMoney,dLRDate,iLRMoney,dEndDate,iFrequency,cCusDefine1,cCusDefine2,cCusDefine3,iCostGrade,
cCreatePerson,cModifyPerson,dModifyDate,cRelVendor,iId,cPriceGroup,cOfferGrade,iOfferRate,cCusDefine4,cCusDefine5,
cCusDefine6,cCusDefine7,cCusDefine8,cCusDefine9,cCusDefine10,cCusDefine11,cCusDefine12,cCusDefine13,cCusDefine14,
cCusDefine15,cCusDefine16,cInvoiceCompany,bCredit,bCreditDate,dLicenceSDate,dLicenceEDate,iLicenceADays,dBusinessSDate,
dBusinessEDate,iBusinessADays,dProxySDate,dProxyEDate,iProxyADays,cMemo,bLimitSale,cCusCountryCode,cCusEnName,
cCusEnAddr1,cCusEnAddr2,cCusEnAddr3,cCusEnAddr4,cCusPortCode,cPrimaryVen,fCommisionRate,fInsueRate,bHomeBranch,
cBranchAddr,cBranchPhone,cBranchPerson,cCusTradeCCode,CustomerKCode,bCusState,bShop,cCusExch_name,bCusDomestic,
bCusOverseas,cCusCreditCompany,cCusSAProtocol,cCusExProtocol,cCusOtherProtocol,fCusDiscountRate,cCusSSCode,
cCusCMProtocol,dCusCreateDatetime,cCusMnemCode,fAdvancePaymentRatio,bServiceAttribute,bRequestSign,bOnGPinStore,
cCusMngTypeCode,account_type,cCusImAgentProtocol) values
(@cCusCode,@cCusName,@cCusAbbName,@cCCCode,NULL,NULL,@cCusAddress,NULL,NULL,GetDate(),
NULL,NULL,@cCusPerson,NULL,NULL,NULL,@cCusHand,NULL,0,NULL,0,0,NULL,NULL,@cCusCode,NULL,@cCusDepart,0,NULL,0,NULL,0,
NULL,0,@cCusDefine1,NULL,NULL,-1,@cMaker,@cMaker,GetDate(),NULL,@cCusCode,NULL,NULL,NULL,NULL,
NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,@cCusCode,0,0,NULL,NULL,NULL,NULL,NULL,
NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,
0,0,N'人民币',1,0,@cCusCode,NULL,NULL,NULL,NULL,NULL,NULL,GetDate(),NULL,NULL,0,0,0,N'999',26,NULL);
insert into sa_invoicecustomers(ccuscode,cinvoicecompany,bdefault) values (@cCusCode,@cCusCode,1 );
insert into Customer_extradefine(cCusCode) values (@cCusCode);");
            SqlParameter[] para = { 
                                      new SqlParameter("@cCusCode",GetDBValue(entity.cCusCode)),
                                      new SqlParameter("@cCusName",GetDBValue(entity.cCusName)),
                                      new SqlParameter("@cCusAbbName",GetDBValue(entity.cCusAbbName)),
                                      new SqlParameter("@cCCCode",GetDBValue(entity.cCCCode)),
                                      new SqlParameter("@cMaker",GetDBValue(entity.cMaker)),                      
                                      new SqlParameter("@cCusAddress",GetDBValue(entity.cCusAddress)),    
                                      new SqlParameter("@cCusHand",GetDBValue(entity.cCusHand)),    
                                      new SqlParameter("@cCusPerson",GetDBValue(entity.cCusPerson)),    
                                      new SqlParameter("@cCusDefine1",GetDBValue(entity.cCusDefine1)),   
                                      new SqlParameter("@cCusDepart",GetDBValue(entity.cCusDepart)), 
                                  };

            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (entity.cCusCode);
        }
        /// <summary>
        /// 更新客户档案
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string UpdateCustomer(EntityCustomer entity)
        {
            if (entity == null)
            {
                throw new JRException("客户档案更新失败!没有数据!");
            }
            string sql = string.Format(@"Update Customer Set cCusName=@cCusName,cCusAbbName=@cCusAbbName,cCCCode=@cCCCode,cCusAddress=@cCusAddress
                                       ,cCusHand=@cCusHand,cCusPerson=@cCusPerson,cCusDepart=@cCusDepart,cCusDefine1=@cCusDefine1
                                        Where cCusCode=@cCusCode");
            SqlParameter[] para = { 
                                      new SqlParameter("@cCusCode",GetDBValue(entity.cCusCode)),
                                      new SqlParameter("@cCusName",GetDBValue(entity.cCusName)),
                                      new SqlParameter("@cCusAbbName",GetDBValue(entity.cCusAbbName)),
                                      new SqlParameter("@cCCCode",GetDBValue(entity.cCCCode)),
                                      new SqlParameter("@cMaker",GetDBValue(entity.cMaker)),                      
                                      new SqlParameter("@cCusAddress",GetDBValue(entity.cCusAddress)),    
                                      new SqlParameter("@cCusHand",GetDBValue(entity.cCusHand)),    
                                      new SqlParameter("@cCusPerson",GetDBValue(entity.cCusPerson)),    
                                      new SqlParameter("@cCusDefine1",GetDBValue(entity.cCusDefine1)),   
                                      new SqlParameter("@cCusDepart",GetDBValue(entity.cCusDepart)),
                                  };

            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (entity.cCusCode);
        }
    }
}
