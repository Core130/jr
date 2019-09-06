using JR.BLL;
using JR.HL;
using JR.HL.Security;
using JR.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace JRWebAPI.Controllers
{
    public class U8ApiController : Controller
    {
        //
        // GET: /U8Api/
        private static object RdRecord09Lock = new object();
        private static object SaleLock = new object();
        private static object DispatchList1Lock = new object();
        private static object DispatchList2Lock = new object();
        private static object Ap_CloseBillLock = new object();
        private static object Ap_VouchLock = new object();
        private static object PO_PomainLock = new object();
        private static object RdRecord01Lock = new object();
        private static object PayAp_CloseBillLock = new object();
        private static object ST_AppTransVouchLock = new object();
        private static object TransVouchLock = new object();
        private static object VendorLock = new object();
        private static object InventoryClassLock = new object();
        private static object InventoryLock = new object();
        private static object WareHouseLock = new object();
        private static object CustomerLock = new object();
        private static object RdRecord01TLock = new object();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [APIExceptionFilter]
        [APIActionFilter]
        public string AddModels(string UserCode, string CipherPassword, int ModelType,
                                    string StrAccID, string Act, string Entity)
        {

            //Entity = System.Web.HttpUtility.UrlDecode(Entity);
            string plainPassword = DefineEncryptDecrypt.Decrypt(CipherPassword);
            int AccYear = U8BllBase.GetBeginYear(StrAccID);
            if (AccYear == 0)
            {
                return ControllerHelp.GetReturnStr(0, string.Format("没有找到账套号:{0}", StrAccID));
            }
            int success = 0;
            ModelsType mt = (ModelsType)ModelType;

            switch (mt)
            {
                case ModelsType.Sale: // 销售订单
                    lock (SaleLock)
                    {
                        if (Act == "add")
                        {

                            EntitySaleHead entity = JsonConvert.DeserializeObject<EntitySaleHead>(Entity);
                            SoMainBll bll = new SoMainBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddSale(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out  success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }

                case ModelsType.RdRecord09: // 配送出库单(其他出库单)
                    lock (RdRecord09Lock)
                    {
                        if (Act == "add")
                        {
                            EntityRdRecord09Head entity = JsonConvert.DeserializeObject<EntityRdRecord09Head>(Entity);
                            RdRecord09Bll bll = new RdRecord09Bll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddRdRecord09(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.DispatchList1://销售退货单
                case ModelsType.DispatchList4://委托代销退货单
                    lock (DispatchList1Lock)
                    {
                        if (Act == "add")
                        {
                            EntityDispatchListHead entity = JsonConvert.DeserializeObject<EntityDispatchListHead>(Entity);
                            DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddDispatchList(UserCode, plainPassword, StrAccID, AccYear, Act, ModelsType.DispatchList1 == mt ? 0 : 1, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.DispatchList2://销售发货单
                case ModelsType.DispatchList3://委托代销发货单
                    lock (DispatchList2Lock)
                    {
                        if (Act == "add")
                        {
                            EntityDispatchListHead entity = JsonConvert.DeserializeObject<EntityDispatchListHead>(Entity);
                            DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddDispatchList(UserCode, plainPassword, ModelType, StrAccID, AccYear, Act, ModelsType.DispatchList2 == mt ? 0 : 1, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.Ap_CloseBill://收款单
                    lock (Ap_CloseBillLock)
                    {
                        if (Act == "add")
                        {
                            EntityAp_CloseBillHead entity = JsonConvert.DeserializeObject<EntityAp_CloseBillHead>(Entity);
                            Ap_CloseBillBll bll = new Ap_CloseBillBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddAp_CloseBill(UserCode, plainPassword, StrAccID, AccYear, Act, ModelType, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.Ap_Vouch://其他应付款
                    lock (Ap_VouchLock)
                    {
                        if (Act == "add")
                        {
                            EntityAp_VouchHead entity = JsonConvert.DeserializeObject<EntityAp_VouchHead>(Entity);
                            Ap_VouchBll bll = new Ap_VouchBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddAp_Vouch(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.PO_Pomain://采购订单
                    lock (PO_PomainLock)
                    {
                        if (Act == "add")
                        {
                            EntityPO_Pomain entity = JsonConvert.DeserializeObject<EntityPO_Pomain>(Entity);
                            PO_PomainBll bll = new PO_PomainBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddPO_Pomain(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.RdRecord01://采购入库单
                    lock (RdRecord01Lock)
                    {
                        if (Act == "add")
                        {
                            EntityRdRecord01Head entity = JsonConvert.DeserializeObject<EntityRdRecord01Head>(Entity);
                            RdRecord01Bll bll = new RdRecord01Bll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddRdRecord01(UserCode, plainPassword, StrAccID, AccYear, Act, 0, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.PayAp_CloseBill: //付款单
                    lock (PayAp_CloseBillLock)
                    {
                        if (Act == "add")
                        {
                            EntityAp_CloseBillHead entity = JsonConvert.DeserializeObject<EntityAp_CloseBillHead>(Entity);
                            Ap_CloseBillBll bll = new Ap_CloseBillBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddAp_CloseBill(UserCode, plainPassword, StrAccID, AccYear, Act, ModelType, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.ST_AppTransVouch: //调拨申请单
                    lock (ST_AppTransVouchLock)
                    {
                        if (Act == "add")
                        {
                            EntityST_AppTransVouch entity = JsonConvert.DeserializeObject<EntityST_AppTransVouch>(Entity);
                            ST_AppTransVouchBll bll = new ST_AppTransVouchBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddST_AppTransVouch(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.TransVouch: //调拨单
                    lock (TransVouchLock)
                    {
                        if (Act == "add")
                        {
                            EntityTransVouch entity = JsonConvert.DeserializeObject<EntityTransVouch>(Entity);
                            TransVouchBll bll = new TransVouchBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddTransVouch(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.Vendor: //供应商  
                    lock (VendorLock)
                    {
                        if (Act == "add")
                        {
                            EntityVendor entity = JsonConvert.DeserializeObject<EntityVendor>(Entity);
                            VendorBll bll = new VendorBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddVendor(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.CheckVouch: //盘点单  
                    lock (VendorLock)
                    {
                        if (Act == "add")
                        {
                            EntityCheckVouchHead entity = JsonConvert.DeserializeObject<EntityCheckVouchHead>(Entity);
                            CheckVouchBll bll = new CheckVouchBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddCheckVouch(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.InventoryClass: //存货分类
                    lock (InventoryClassLock)
                    {
                        if (Act == "add")
                        {
                            EntityInventoryClass entity = JsonConvert.DeserializeObject<EntityInventoryClass>(Entity);
                            InventoryClassBll bll = new InventoryClassBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddInventoryClass(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.Inventory: //存货档案
                    lock (InventoryLock)
                    {
                        if (Act == "add")
                        {
                            EntityInventory entity = JsonConvert.DeserializeObject<EntityInventory>(Entity);
                            InventoryBll bll = new InventoryBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddInventory(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.WareHouse: //仓库档案
                    lock (WareHouseLock)
                    {
                        if (Act == "add")
                        {
                            EntityWareHouse entity = JsonConvert.DeserializeObject<EntityWareHouse>(Entity);
                            WareHouseBll bll = new WareHouseBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddWareHouse(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.Customer: //客户档案
                    lock (CustomerLock)
                    {
                        if (Act == "add")
                        {
                            EntityCustomer entity = JsonConvert.DeserializeObject<EntityCustomer>(Entity);
                            CustomerBll bll = new CustomerBll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddCustomer(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.RdRecord01T://红字采购入库单
                    lock (RdRecord01TLock)
                    {
                        if (Act == "add")
                        {
                            EntityRdRecord01Head entity = JsonConvert.DeserializeObject<EntityRdRecord01Head>(Entity);
                            RdRecord01Bll bll = new RdRecord01Bll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.AddRdRecord01(UserCode, plainPassword, StrAccID, AccYear, Act, 1, entity, out success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
            }

            return ControllerHelp.GetReturnStr(0, "AddModels中没有找到可对应的操作项");


        }

        [HttpPost]
        [APIExceptionFilter]
        [APIActionFilter]
        public string GetModels(string UserCode, string CipherPassword, int ModelType,
                                    string StrAccID, string Act, string Entity, string dModifyDate)
        {
            //Entity = System.Web.HttpUtility.UrlDecode(Entity);
            string plainPassword = DefineEncryptDecrypt.Decrypt(CipherPassword);
            int AccYear = U8BllBase.GetBeginYear(StrAccID);
            if (AccYear == 0)
            {
                return ControllerHelp.GetReturnStr(0, string.Format("没有找到账套号{0}", StrAccID));
            }
            ModelsType mt = (ModelsType)ModelType;
            string msg = "";
            switch (mt)
            {
                case ModelsType.CurrentStock: // 库存量查询
                    if (Act == "get")
                    {
                        EntityCurrentStock entity = JsonConvert.DeserializeObject<EntityCurrentStock>(Entity);
                        CurrentStockBll bll = new CurrentStockBll(StrAccID, AccYear, UserCode, plainPassword);
                        List<EntityCurrentStock> list = bll.GetCurrentStock(entity, out msg);
                        return ControllerHelp.GetReturnStr(string.IsNullOrWhiteSpace(msg) ? 1 : 0, "", list);
                    }
                    break;
                case ModelsType.Sale: // 查询销售订单是否存在 1表示不存在，2表示存在
                    if (Act == "query")
                    {
                        SoMainBll bll = new SoMainBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntitySaleHead entity = JsonConvert.DeserializeObject<EntitySaleHead>(Entity);
                        int num = bll.QuerySale(entity.cSOCode);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.DispatchList1: //退货单 1表示不存在，2表示存在
                    if (Act == "query")
                    {
                        DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityDispatchListHead entity = JsonConvert.DeserializeObject<EntityDispatchListHead>(Entity);
                        int num = bll.QueryDispatchList(entity.cDLCode);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.DispatchList2: //发货单 1表示不存在，2表示存在
                    if (Act == "query")
                    {
                        DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityDispatchListHead entity = JsonConvert.DeserializeObject<EntityDispatchListHead>(Entity);
                        int num = bll.QueryDispatchList(entity.cDLCode);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.RdRecord09: // 配送出库单(其他出库单) 1表示不存在，2表示存在
                    if (Act == "query")
                    {
                        RdRecord09Bll bll = new RdRecord09Bll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityRdRecord09Head entity = JsonConvert.DeserializeObject<EntityRdRecord09Head>(Entity);
                        int num = bll.QueryRdRecord09(entity.cCode);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.Ap_CloseBill: //收款单（汇款通知单）1表示不存在，2表示存在
                    if (Act == "query")
                    {
                        Ap_CloseBillBll bll = new Ap_CloseBillBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityAp_CloseBillHead entity = JsonConvert.DeserializeObject<EntityAp_CloseBillHead>(Entity);
                        int num = bll.QueryAp_CloseBill(entity.cVouchID);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.PayAp_CloseBill: //付款单（汇款通知单）1表示不存在，2表示存在
                    if (Act == "query")
                    {
                        Ap_CloseBillBll bll = new Ap_CloseBillBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityAp_CloseBillHead entity = JsonConvert.DeserializeObject<EntityAp_CloseBillHead>(Entity);
                        int num = bll.QueryAp_CloseBill(entity.cVouchID);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.ReturnAnalysis: //退货分析表查询
                    if (Act == "get")
                    {
                        ReturnAnalysisBll bll = new ReturnAnalysisBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityReturnAnalysisRQC entity = JsonConvert.DeserializeObject<EntityReturnAnalysisRQC>(Entity);
                        List<EntityReturnAnalysisReport> list = bll.GetReturnAnalysis(entity);
                        return ControllerHelp.GetReturnStr(1, "", list);

                    }
                    break;
                case ModelsType.Customer://客户档案
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        EntityCustomer entity = JsonConvert.DeserializeObject<EntityCustomer>(Entity);
                        List<EntityCustomer> customer = bll.GetCustomer(entity.cCusCode);
                        return ControllerHelp.GetReturnStr(1, "", customer);

                    }
                    break;
                case ModelsType.Vendor://供应商档案
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        List<EntityVendor> vendor = bll.GetVendor();
                        return ControllerHelp.GetReturnStr(1, "", vendor);

                    }
                    break;
                case ModelsType.PurchaseType://采购类型
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        List<EntityPurchaseType> purchasetype = bll.GetPurchaseType();
                        return ControllerHelp.GetReturnStr(1, "", purchasetype);

                    }
                    break;
                case ModelsType.Inventory://存货档案
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        List<EntityInventory> inventory=new List<EntityInventory>();
                        if (string.IsNullOrWhiteSpace(dModifyDate))
                        {
                            inventory = bll.GetInventory();
                        }
                        else
                        {
                            inventory = bll.GetInventory(Convert.ToDateTime(dModifyDate));
                        }
                        return ControllerHelp.GetReturnStr(1, "", inventory);
                    }
                    break;
                case ModelsType.WareHouse://仓库档案（入库仓库）
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        List<EntityWareHouse> warehouse = bll.GetWareHouse();
                        return ControllerHelp.GetReturnStr(1, "", warehouse);
                    }
                    break;
                case ModelsType.rd_Style://入库类别档案
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        List<Entityrd_Style> rd_style = bll.Getrd_Style();
                        return ControllerHelp.GetReturnStr(1, "", rd_style);
                    }
                    break;
                case ModelsType.SettleStyle://结算方式
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        List<EntitySettleStyle> settlestyle = bll.GetSettleStyle();
                        return ControllerHelp.GetReturnStr(1, "", settlestyle);
                    }
                    break;
                case ModelsType.CustomerProperty://经营属性
                    if (Act == "get")
                    {
                        U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                        string cuscode = Entity;
                        var customer = bll.GetCustomerProperty(cuscode);
                        return ControllerHelp.GetReturnStr(1, "", customer);
                    }
                    break;
                case ModelsType.InventoryPrice://存货入库含税单价
                    if (Act == "get")
                    {
                        if (!string.IsNullOrWhiteSpace(Entity))
                        {
                            U8BllBase bll = new U8BllBase(StrAccID, AccYear, UserCode, plainPassword);
                            EntityInventoryPrice entity = JsonConvert.DeserializeObject<EntityInventoryPrice>(Entity);
                            var inventoryPrice = bll.GetInventoryPrice(entity.cVenCode, entity.cInvCode);
                            return ControllerHelp.GetReturnStr(1, "", inventoryPrice);
                        }
                    }
                    break;
                case ModelsType.PO_Pomain: //采购订单
                    if (Act == "query")
                    {
                        PO_PomainBll bll = new PO_PomainBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityPO_Pomain entity = JsonConvert.DeserializeObject<EntityPO_Pomain>(Entity);
                        int num = bll.GetPO_Pomain(entity.cPOID);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
                case ModelsType.RdRecord01: //采购入库单
                    if (Act == "query")
                    {
                        RdRecord01Bll bll = new RdRecord01Bll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityRdRecord01Head entity = JsonConvert.DeserializeObject<EntityRdRecord01Head>(Entity);
                        int num = bll.GetRdRecord01(entity.cCode);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;

                case ModelsType.Ap_Vouch: //其他应付单
                    if (Act == "query")
                    {
                        Ap_VouchBll bll = new Ap_VouchBll(StrAccID, AccYear, UserCode, plainPassword);
                        EntityAp_VouchHead entity = JsonConvert.DeserializeObject<EntityAp_VouchHead>(Entity);
                        int num = bll.GetAp_Vouch(entity.cVouchID);
                        if (num == 0)
                            return ControllerHelp.GetReturnStr(1, "new");
                        else
                            return ControllerHelp.GetReturnStr(2, "old");
                    }
                    break;
            }

            return ControllerHelp.GetReturnStr(0, "GetModels(查询)中没有找到可对应的操作项");

        }
        [HttpPost]
        [APIExceptionFilter]
        [APIActionFilter]
        public string UpdateModels(string UserCode, string CipherPassword, int ModelType,
                                    string StrAccID, string Act, string cSOCode, string OType)
        {

            string plainPassword = DefineEncryptDecrypt.Decrypt(CipherPassword);
            int AccYear = U8BllBase.GetBeginYear(StrAccID);
            if (AccYear == 0)
            {
                return ControllerHelp.GetReturnStr(0, string.Format("没有找到账套号:{0}", StrAccID));
            }
            ModelsType mt = (ModelsType)ModelType;
            switch (mt)
            {
                case ModelsType.Sale: // 销售订单
                    if (Act == "update")
                    {
                        SoMainBll bll = new SoMainBll(StrAccID, AccYear, UserCode, plainPassword);
                        //EntitySaleHead entity = JsonConvert.DeserializeObject<EntitySaleHead>(Entity);
                        string UserName = bll.GetUserName(UserCode);
                        int success = bll.OpenCloseSale(cSOCode, UserName, OType);

                        if (success > 0)
                        {
                            if (OType == "open")
                            {
                                return ControllerHelp.GetReturnStr(1, "订单已打开！");
                            }
                            return ControllerHelp.GetReturnStr(1, "订单已关闭！");
                        }
                        else
                        {
                            return ControllerHelp.GetReturnStr(0, "没有找到相关订单！");
                        }

                    }
                    break;

            }
            return ControllerHelp.GetReturnStr(0, "UpdateModels(修改)中没有找到可对应的操作项");
        }

        [HttpPost]
        [APIExceptionFilter]
        [APIActionFilter]
        public string DeleteModels(string UserCode, string CipherPassword, int ModelType,
                                    string StrAccID, string Act, string Entity)
        {
            string plainPassword = DefineEncryptDecrypt.Decrypt(CipherPassword);
            int AccYear = U8BllBase.GetBeginYear(StrAccID);
            if (AccYear == 0)
            {
                return ControllerHelp.GetReturnStr(0, string.Format("没有找到账套号:{0}", StrAccID));
            }
            int success = 0;
            ModelsType mt = (ModelsType)ModelType;
            switch (mt)
            {
                case ModelsType.RdRecord01://采购入库单        
                    {
                        if (Act == "delete")
                        {
                            EntityRdRecord01Head entity = JsonConvert.DeserializeObject<EntityRdRecord01Head>(Entity);
                            RdRecord01Bll bll = new RdRecord01Bll(StrAccID, AccYear, UserCode, plainPassword);
                            var result = bll.DelRdRecord01(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out  success);
                            return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                        }
                        break;
                    }
                case ModelsType.DispatchList2: //发货单
                    if (Act == "delete")
                    {
                        EntityDispatchListHead entity = JsonConvert.DeserializeObject<EntityDispatchListHead>(Entity);
                        DispatchListBll bll = new DispatchListBll(StrAccID, AccYear, UserCode, plainPassword);
                        var result = bll.DelDispatchList(UserCode, plainPassword, StrAccID, AccYear, Act, entity, out success);
                        return ControllerHelp.GetReturnStr(success, StrAccID + '_' + result);
                    }
                    break;
            }
            return ControllerHelp.GetReturnStr(0, "DeleteModels中没有找到可对应的操作项");
        }
    }

    public class ControllerHelp
    {

        /// <summary>
        /// 组装返回信息JSON
        /// </summary>
        /// <param name="success"></param>
        /// <param name="msg"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetReturnStr(int success, string msg, object entity = null)
        {
            ReturnInfo info = new ReturnInfo() { Success = success, Message = msg, Entity = entity };
            return JsonConvert.SerializeObject(info);
        }
    }
}
