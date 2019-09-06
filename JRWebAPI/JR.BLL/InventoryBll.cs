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
    public class InventoryBll : U8BllBase
    {
        public InventoryBll(string accID, int beginYear, string userID, string password)
            : base(accID, beginYear, userID, password)
        {

        }
        public string AddInventory(string UserCode, string PlainPassword,
                                    string StrAccID, int AccYear, string Act, EntityInventory entity, out int success)
        {
            success = 0;
            #region 验证

            // 字段必填项验证
            string msg = "";
            if (!entity.CheckEntity(out msg))
            {
                return msg;
            }


            if (string.IsNullOrWhiteSpace(GetInventoryClass(entity.cInvCCode, true)))
            {
                return string.Format("U8中不存在存货分类编码或非末级:{0}!", entity.cInvCCode);
            }

            EntityComputationUnit unit = new EntityComputationUnit();
            unit = GetComputationUnit(entity.cComUnitCode);
            if (string.IsNullOrWhiteSpace(unit.cComUnitCode))
            {
                return string.Format("U8中没有维护计量单位:{0}!", entity.cComUnitCode);
            }
            #endregion
            entity.cComUnitCode = unit.cComUnitCode;
            entity.cGroupCode = unit.cGroupCode;
            entity.cMaker = GetUserName(UserCode);
            entity.bBarCode = string.IsNullOrWhiteSpace(entity.cBarCode) ? 0 : 1;
            entity.cValueType = string.IsNullOrWhiteSpace(entity.cValueType) ? "全月平均法" : entity.cValueType;
            string id = "";
            if (!string.IsNullOrWhiteSpace(GetInventory(entity.cInvCode)))
            {
                id = UpdateInventory(entity);
            }
            else
            {
                id = InsertInventory(entity);
            }
            success = string.IsNullOrWhiteSpace(id) ? 0 : 1;
            return id;
        }
        public string InsertInventory(EntityInventory entity)
        {
            if (entity == null)
            {
                throw new JRException("存货档案新增失败!没有数据!");
            }
            string sql = string.Format(@"INSERT INTO Inventory(
cInvCode,cInvAddCode,cInvName,cInvStd,cInvCCode,cVenCode,cReplaceItem,cPosition,bSale,bPurchase,bSelf,bComsume,bProducing,bService,bAccessary,iTaxRate,
iInvWeight,iVolume,iInvRCost,iInvSPrice,iInvSCost,iInvLSCost,iInvNCost,iInvAdvance,iInvBatch,iSafeNum,iTopSum,iLowSum,iOverStock,cInvABC,bInvQuality,bInvBatch,bInvEntrust,bInvOverStock,dSDate,dEDate,bFree1,bFree2,cInvDefine1,cInvDefine2,cInvDefine3,bInvType,iInvMPCost,cQuality,bFree3,bFree4,bFree5,bFree6,bFree7,bFree8,bFree9,bFree10,
cCreatePerson,cModifyPerson,dModifyDate,fSubscribePoint,fVagQuantity,cValueType,fOutExcess,fInExcess,iMassDate,iWarnDays,fExpensesExch,bTrack,bSerial,bBarCode,cBarCode,
cInvDefine4,cInvDefine5,cInvDefine6,cInvDefine7,cInvDefine8,cInvDefine9,cInvDefine10,cInvDefine11,cInvDefine12,cInvDefine13,cInvDefine14,cInvDefine15,cInvDefine16,
iGroupType,cGroupCode,cComUnitCode,cAssComUnitCode,cSAComUnitCode,cPUComUnitCode,cSTComUnitCode,cCAComUnitCode,
cFrequency,iFrequency,iDays,dLastDate,iWastage,bSolitude,cEnterprise,cAddress,cFile,cLabel,cCheckOut,cLicence,bSpecialties,
cDefWareHouse,iHighPrice,iExpSaleRate,cPriceGroup,cOfferGrade,iOfferRate,cCurrencyName,cProduceAddress,cProduceNation,cRegisterNo,
cEnterNo,cPackingType,cEnglishName,bPropertyCheck,cPreparationType,cCommodity,iRecipeBatch,cNotPatentName,iROPMethod,iBatchRule,
iAssureProvideDays,iTestStyle,iDTMethod,fDTRate,fDTNum,cDTUnit,iDTStyle,iQTMethod,bPlanInv,bProxyForeign,bATOModel,bCheckItem,
bPTOModel,bEquipment,cProductUnit,fOrderUpLimit,cMassUnit,fRetailPrice,cInvDepCode,iAlterAdvance,fAlterBaseNum,cPlanMethod,bMPS,
bROP,bRePlan,cSRPolicy,bBillUnite,iSupplyDay,fSupplyMulti,fMinSupply,bCutMantissa,cInvPersonCode,iInvTfId,cEngineerFigNo,
bInTotalCost,iSupplyType,bConfigFree1,bConfigFree2,bConfigFree3,bConfigFree4,bConfigFree5,bConfigFree6,bConfigFree7,bConfigFree8,
bConfigFree9,bConfigFree10,iDTLevel,cDTAQL,bPeriodDT,cDTPeriod,iBigMonth,iBigDay,iSmallMonth,iSmallDay,bOutInvDT,bBackInvDT,
iEndDTStyle,bDTWarnInv,cCIQCode,cWGroupCode,cWUnit,fGrossW,cVGroupCode,cVUnit,fLength,fWidth,fHeight,cShopUnit,cPurPersonCode,
bImportMedicine,bFirstBusiMedicine,bForeExpland,cInvPlanCode,fConvertRate,dReplaceDate,bInvModel,bKCCutMantissa,bReceiptByDT,
iImpTaxRate,bExpSale,iDrawBatch,bCheckBSATP,cInvProjectCode,iTestRule,cRuleCode,bCheckFree1,bCheckFree2,bCheckFree3,bCheckFree4,
bCheckFree5,bCheckFree6,bCheckFree7,bCheckFree8,bCheckFree9,bCheckFree10,bBomMain,bBomSub,bProductBill,iCheckATP,iInvATPId,iPlanTfDay,
iOverlapDay,bPiece,bSrvItem,bSrvFittings,fMaxSupply,fMinSplit,bSpecialOrder,bTrackSaleBill,cInvMnemCode)
VALUES
(@cInvCode,@cInvAddCode,@cInvName,@cInvStd,@cInvCCode,NULL,NULL,NULL,1,1,1,1,0,0,0,13,
NULL,NULL,NULL,@iInvSPrice,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,0,0,0,
0,GetDate(),NULL,0,0,@cInvDefine1,@cInvDefine12,@cInvDefine3,0,NULL,NULL,0,0,0,0,0,0,0,0,
@cMaker,@cMaker,GetDate(),NULL,NULL,@cValueType,NULL,NULL,NULL,NULL,NULL,0,0,@bBarCode,@cBarCode,
@cInvDefine4,@cInvDefine5,@cInvDefine6,@cInvDefine7,@cInvDefine8,@cInvDefine9,@cInvDefine10,@cInvDefine11,@cInvDefine12,@cInvDefine13,NULL,NULL,NULL,
0,@cGroupCode,@cComUnitCode,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
@cPackingType,NULL,0,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0,0,0,0,0,NULL,NULL,
NULL,@fRetailPrice,NULL,NULL,NULL,N'R',0,0,0,N'PE',0,NULL,NULL,NULL,0,NULL,NULL,NULL,1,0,0,0,0,0,0,0,0,0,0,0,
NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,0,0,NULL,0,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,@cComUnitCode,
NULL,0,0,0,NULL,1,NULL,0,0,0,13,0,NULL,0,NULL,NULL,NULL,0,0,0,0,0,0,0,0,0,0,1,1,1,0,NULL,NULL,NULL,0,0,0,NULL,NULL,0,0,NULL);
                        INSERT INTO Inventory_Sub(cInvSubCode,fBuyExcess,iSurenessType,iDateType,iDateSum,iDynamicSurenessType,
iBestrowSum,iPercentumSum,bIsAttachFile,bInByProCheck,iRequireTrackStyle,iExpiratDateCalcu,iBOMExpandUnitType,bPurPriceFree1,
bPurPriceFree2,bPurPriceFree3,bPurPriceFree4,bPurPriceFree5,bPurPriceFree6,bPurPriceFree7,bPurPriceFree8,bPurPriceFree9,bPurPriceFree10,
bOMPriceFree1,bOMPriceFree2,bOMPriceFree3,bOMPriceFree4,bOMPriceFree5,bOMPriceFree6,bOMPriceFree7,bOMPriceFree8,bOMPriceFree9,
bOMPriceFree10,bSalePriceFree1,bSalePriceFree2,bSalePriceFree3,bSalePriceFree4,bSalePriceFree5,bSalePriceFree6,bSalePriceFree7,
bSalePriceFree8,bSalePriceFree9,bSalePriceFree10,fInvOutUpLimit,bBondedInv,bBatchCreate,bBatchProperty1,bBatchProperty2,bBatchProperty3,
bBatchProperty4,bBatchProperty5,bBatchProperty6,bBatchProperty7,bBatchProperty8,bBatchProperty9,bBatchProperty10,bControlFreeRange1,
bControlFreeRange2,bControlFreeRange3,bControlFreeRange4,bControlFreeRange5,bControlFreeRange6,bControlFreeRange7,bControlFreeRange8,
bControlFreeRange9,bControlFreeRange10,fInvCIQExch,iWarrantyPeriod,iWarrantyUnit,bInvKeyPart,iAcceptEarlyDays,fCurLLaborCost,
fCurLVarManuCost,fCurLFixManuCost,fCurLOMCost,fNextLLaborCost,fNextLVarManuCost,fNextLFixManuCost,fNextLOMCost,dInvCreateDatetime,
bPUQuota,bInvROHS,bPrjMat,fPrjMatLimit,bInvAsset,bSrvProduct,iAcceptDelayDays,iPlanCheckDay,iMaterialsCycle,iDrawType,bSCkeyProjections,
iSupplyPeriodType,iTimeBucketId,iAvailabilityDate,fMaterialCost,bImport,iNearRejectDays,bCheckSubitemCost,fRoundFactor,bConsiderFreeStock,bSuitRetail)
VALUES
(@cInvCode,NULL,1,NULL,NULL,NULL,NULL,NULL,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
NULL,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,NULL,0,1,999,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,
GetDate(),0,0,0,NULL,0,0,0,NULL,NULL,0,0,1,NULL,1,NULL,0,NULL,1,0,1,0);
                        INSERT INTO bas_part(partId,InvCode,bVirtual ,SafeQty,MinQty,MulQty,FixQty,cBasEngineerFigNo,
fBasMaxSupply,iSurenessType,iDateType,iDateSum,iDynamicSurenessType,iBestrowSum,iPercentumSum)
VALUES
(@PartId,@cInvCode,1,NULL,NULL,NULL,NULL,NULL,NULL,N'1',NULL,NULL,NULL,NULL,NULL)");
            SqlParameter[] para = { 
                                        new SqlParameter("@cInvName",entity.cInvName),
                                        new SqlParameter("@cInvCode",entity.cInvCode),
                                        new SqlParameter("@cInvCCode",entity.cInvCCode),
                                        new SqlParameter("@cGroupCode",entity.cGroupCode),  
                                        new SqlParameter("@cComUnitCode",GetDBValue(entity.cComUnitCode)),
                                        new SqlParameter("@cPackingType",GetDBValue(entity.cPackingType)),
                                        new SqlParameter("@cInvStd",GetDBValue(entity.cInvStd)),
                                        new SqlParameter("@cValueType",GetDBValue(entity.cValueType)),
                                        new SqlParameter("@iInvSPrice",entity.iInvSPrice),
                                        new SqlParameter("@fRetailPrice",entity.fRetailPrice),
                                        new SqlParameter("@cBarCode",GetDBValue(entity.cBarCode)),
                                        new SqlParameter("@bBarCode",entity.bBarCode),
                                        new SqlParameter("@PartId",GetPartId()+1),
                                        new SqlParameter("@cMaker",GetDBValue(entity.cMaker)),
                                        new SqlParameter("@cInvAddCode",GetDBValue(entity.cInvAddCode)),
                                        new SqlParameter("@cInvDefine1",GetDBValue(entity.cInvDefine1)),
                                        new SqlParameter("@cInvDefine2",GetDBValue(entity.cInvDefine2)),
                                        new SqlParameter("@cInvDefine3",GetDBValue(entity.cInvDefine3)),
                                        new SqlParameter("@cInvDefine4",GetDBValue(entity.cInvDefine4)),
                                        new SqlParameter("@cInvDefine5",GetDBValue(entity.cInvDefine5)),
                                        new SqlParameter("@cInvDefine6",GetDBValue(entity.cInvDefine6)),
                                        new SqlParameter("@cInvDefine7",GetDBValue(entity.cInvDefine7)),
                                        new SqlParameter("@cInvDefine8",GetDBValue(entity.cInvDefine8)),
                                        new SqlParameter("@cInvDefine9",GetDBValue(entity.cInvDefine9)),
                                        new SqlParameter("@cInvDefine10",GetDBValue(entity.cInvDefine10)),
                                        new SqlParameter("@cInvDefine11",SqlNull(entity.cInvDefine11)),
                                        new SqlParameter("@cInvDefine12",SqlNull(entity.cInvDefine12)),
                                        new SqlParameter("@cInvDefine13",entity.cInvDefine13==null?entity.fRetailPrice:entity.cInvDefine13),
                                        //new SqlParameter("@cInvDefine14",entity.cInvDefine14==null?entity.iInvSPrice:entity.cInvDefine14),
                                   };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (entity.cInvCode);

        }
        /// <summary>
        /// 更新存货档案
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="PlainPassword"></param>
        /// <param name="StrAccID"></param>
        /// <param name="AccYear"></param>
        /// <param name="Act"></param>
        /// <param name="entity"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        public string UpdateInventory(EntityInventory entity)
        {
            if (entity == null)
            {
                throw new JRException("存货档案更新失败!没有数据!");
            }
            string sql = string.Format(@" Update Inventory 
Set cInvAddCode=@cInvAddCode,cInvName =@cInvName ,cInvStd=@cInvStd,cInvCCode = @cInvCCode,cGroupCode=@cGroupCode,cComUnitCode=@cComUnitCode,cPackingType=@cPackingType,iInvSPrice=@iInvSPrice,fRetailPrice=@fRetailPrice,bBarCode=@bBarCode,cBarCode=@cBarCode ,
cInvDefine1=@cInvDefine1,cInvDefine2=@cInvDefine2,cInvDefine3=@cInvDefine3,cInvDefine4=@cInvDefine4,cInvDefine5=@cInvDefine5,cInvDefine6=@cInvDefine6,cInvDefine7=@cInvDefine7,cInvDefine8=@cInvDefine8,cInvDefine9=@cInvDefine9,
cInvDefine10=@cInvDefine10,cInvDefine11=@cInvDefine11,cInvDefine12=@cInvDefine12,cInvDefine13=@cInvDefine13,cModifyPerson=@cMaker,dModifyDate=GetDate()
                                        Where cInvCode = @cInvCode");
            SqlParameter[] para = { 
                                        new SqlParameter("@cInvName",entity.cInvName),
                                        new SqlParameter("@cInvCode",entity.cInvCode),
                                        new SqlParameter("@cInvCCode",entity.cInvCCode),
                                        new SqlParameter("@cGroupCode",entity.cGroupCode),  
                                        new SqlParameter("@cComUnitCode",GetDBValue(entity.cComUnitCode)),
                                        new SqlParameter("@cPackingType",GetDBValue(entity.cPackingType)),
                                        new SqlParameter("@cInvStd",GetDBValue(entity.cInvStd)),                                        
                                        new SqlParameter("@iInvSPrice",entity.iInvSPrice),
                                        new SqlParameter("@fRetailPrice",entity.fRetailPrice),
                                        new SqlParameter("@cBarCode",GetDBValue(entity.cBarCode)),
                                        new SqlParameter("@bBarCode",entity.bBarCode),  
                                        new SqlParameter("@cMaker",GetDBValue(entity.cMaker)),
                                        new SqlParameter("@cInvAddCode",GetDBValue(entity.cInvAddCode)),
                                        new SqlParameter("@cInvDefine1",GetDBValue(entity.cInvDefine1)),
                                        new SqlParameter("@cInvDefine2",GetDBValue(entity.cInvDefine2)),
                                        new SqlParameter("@cInvDefine3",GetDBValue(entity.cInvDefine3)),
                                        new SqlParameter("@cInvDefine4",GetDBValue(entity.cInvDefine4)),
                                        new SqlParameter("@cInvDefine5",GetDBValue(entity.cInvDefine5)),
                                        new SqlParameter("@cInvDefine6",GetDBValue(entity.cInvDefine6)),
                                        new SqlParameter("@cInvDefine7",GetDBValue(entity.cInvDefine7)),
                                        new SqlParameter("@cInvDefine8",GetDBValue(entity.cInvDefine8)),
                                        new SqlParameter("@cInvDefine9",GetDBValue(entity.cInvDefine9)),
                                        new SqlParameter("@cInvDefine10",GetDBValue(entity.cInvDefine10)),
                                        new SqlParameter("@cInvDefine11",SqlNull(entity.cInvDefine11)),
                                        new SqlParameter("@cInvDefine12",SqlNull(entity.cInvDefine12)),
                                        new SqlParameter("@cInvDefine13",entity.cInvDefine13==null?entity.fRetailPrice:entity.cInvDefine13),
                                        //new SqlParameter("@cInvDefine14",entity.cInvDefine14==null?entity.iInvSPrice:entity.cInvDefine14),
                                   };
            int headCount = this.DB_SqlHelper.ExecuteNonQuery(sql, para);
            return (entity.cInvCode); ;
        }
        /// <summary>
        /// 获取物料表（bas_part）最大ID
        /// </summary>
        /// <returns></returns>
        public int GetPartId()
        {
            int num = 0;
            string sql = @"select Max(PartId) as PartId from bas_part";
            DataTable dt = DB_SqlHelper.ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                return num = int.Parse(dt.Rows[0]["PartId"].ToString());
            }
            return num;
        }
        public EntityComputationUnit GetComputationUnit(string cComUnitName)
        {
            EntityComputationUnit entity = new EntityComputationUnit();
            string sql = @"select cComUnitCode,cGroupCode from ComputationUnit where cComUnitName=@cComUnitName";
            SqlParameter[] para = { 
                                   new SqlParameter("@cComUnitName",cComUnitName),
                                  };
            DataTable dt = DB_SqlHelper.ExecuteDataTable(sql, para);
            if (dt != null && dt.Rows.Count > 0)
            {
                entity.cComUnitCode = dt.Rows[0]["cComUnitCode"].ToString();
                entity.cGroupCode = dt.Rows[0]["cGroupCode"].ToString();
            }
            return entity;

        }
    }
}
