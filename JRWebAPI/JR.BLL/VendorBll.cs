using JR.HL;
using JR.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.BLL
{
    /// <summary>
    /// 供应商处理
    /// </summary>
    public class VendorBll:U8BllBase
    {
        public VendorBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }

        public string AddVendor(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityVendor entity, out int success)
        {
            success = 0;
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }

            
            if (!string.IsNullOrWhiteSpace(GetVendorName(entity.cVenCode)))
            {
                return string.Format("U8中已存在供应商编码:{0}!", entity.cVenCode);
            }

            if (string.IsNullOrWhiteSpace(GetcVcCode(entity.cVcCode)))
            {
                return string.Format("U8中不存在供应商分类编码:{0}!", entity.cVcCode);
            }

            #endregion
            entity.cMaker = GetUserName(UserCode);
            string id = InsertVendor(entity);
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1; ;
            return id;
        }
        public string InsertVendor(EntityVendor vendor)
        {
            
            if (vendor == null)
            {
                throw new JRException("供应商新增失败!没有数据!");
            }
            string sql = string.Format(@"
INSERT INTO  Vendor  ( cVenCode , cVenName , cVenAbbName , cVCCode , cDCCode , cTrade , cVenAddress , cVenPostCode , cVenRegCode ,
cVenBank , cVenAccount , dVenDevDate , cVenLPerson , cVenPhone , cVenFax , cVenEmail , cVenPerson , cVenBP , cVenHand , cVenPPerson ,
iVenDisRate , iVenCreGrade , iVenCreLine , iVenCreDate , cVenPayCond , cVenIAddress , cVenIType , cVenHeadCode , cVenWhCode , 
cVenDepart , iAPMoney , dLastDate , iLastMoney , dLRDate , iLRMoney , dEndDate , iFrequency , bVenTax , cVenDefine1 , cVenDefine2 , 
cVenDefine3 , cCreatePerson , cModifyPerson , dModifyDate , cRelCustomer , cBarCode , cVenDefine4 , cVenDefine5 , cVenDefine6 , 
cVenDefine7 , cVenDefine8 , cVenDefine9 , cVenDefine10 , cVenDefine11 , cVenDefine12 , cVenDefine13 , cVenDefine14 , cVenDefine15 , 
cVenDefine16 , fRegistFund , iEmployeeNum , iGradeABC , cMemo , dLicenceSDate , dLicenceEDate , iLicenceADays , dBusinessSDate , 
dBusinessEDate , iBusinessADays , dProxySDate , dProxyEDate , iProxyADays , bVenCargo , bProxyForeign , bVenService , cVenTradeCCode , 
cVenBankCode , cVenExch_name , iVenGSPType , iVenGSPAuth , cVenGSPAuthNo , cVenBusinessNo , cVenLicenceNo , bVenOverseas , 
bVenAccPeriodMng , cVenPUOMProtocol , cVenOtherProtocol , cVenCountryCode , cVenEnName , cVenEnAddr1 , cVenEnAddr2 , cVenEnAddr3 , 
cVenEnAddr4 , cVenPortCode , cVenPrimaryVen , fVenCommisionRate , fVenInsueRate , bVenHomeBranch , cVenBranchAddr , cVenBranchPhone , 
cVenBranchPerson , cVenSSCode , cOMWhCode , cVenCMProtocol , cVenIMProtocol , iVenTaxRate , dVenCreateDatetime , cVenMnemCode ) 
VALUES (@cVenCode,@cVenName,@cVenAbbName,@cVCCode,NULL,NULL,NULL,NULL,NULL,NULL,NULL,GETDATE(),NULL,NULL,NULL,NULL,NULL,NULL,NULL,
NULL,0,NULL,0,0,NULL,NULL,NULL,@cVenCode,NULL,NULL,0,NULL,0,NULL,0,NULL,0,1,NULL,NULL,NULL,@cCreatePerson,@cModifyPerson,GETDATE(),NULL,NULL,NULL,
NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,-1,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,0,0,
NULL,NULL,N'人民币',0,-1,NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,
NULL,NULL,NULL,GETDATE(),NULL)
");
            SqlParameter[] para = { 
                                      new SqlParameter("@cVenCode",vendor.cVenCode),
                                      new SqlParameter("@cVenName",vendor.cVenName),
                                      new SqlParameter("@cVenAbbName",vendor.cVenAbbName),
                                      new SqlParameter("@cVCCode",vendor.cVcCode),
                                      new SqlParameter("@cCreatePerson",vendor.cMaker),
                                      new SqlParameter("@cModifyPerson",vendor.cMaker),                              

                                  };

            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (vendor.cVenCode);

        }
    }
}
