using JR.HL.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using JR.Models;

namespace JRWebAPI.WF.TEST
{
    public partial class Form1 : Form
    {
        string addurl = "http://210.75.12.178:3441/U8Api/AddModels?";
        string delurl = "http://210.75.12.178:3441/U8Api/DeleteModels?";
        string addpara = "UserCode=demo&CipherPassword=5246543K4Y3K&StrAccID=003&Act=add&Entity";
        string delpara = "UserCode=demo&CipherPassword=5246543K4Y3K&StrAccID=003&Act=delete&Entity";


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = DefineEncryptDecrypt.Encrypt(this.txtInput.Text.Trim());
            this.txtOutput.Text = str;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str = DefineEncryptDecrypt.Decrypt(this.txtInput.Text.Trim());
            this.txtOutput.Text = str;
        }


        /// <summary>
        /// 发货单测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_dispatchlist_Click(object sender, EventArgs e)
        {
            EntityDispatchListHead doc = new EntityDispatchListHead();
            doc.cSTCode = "1";
            doc.dDate = DateTime.Parse("2017-12-05");
            doc.cDepCode = "SZ300104";
            doc.cCusCode = "01007";
            doc.cDLCode = "2017120004";
            doc.cDefine1 = "合同";
            doc.cDefine3 = "是";
            List<EntityDispatchListBody> list = new List<EntityDispatchListBody>();
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "1010100001",
                cWhCode = "K001",//仓库编码
                iQuantity = 1,//数量
                iQuotedPrice = 200,//报价
                iSum = 200,//价税合计
                iTaxRate = 17,//税率 
                DHID = "0",
                cBatch = null
            });
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "1010100001",
                cWhCode = "K001",//仓库编码
                iQuantity = 1,//数量
                iQuotedPrice = 200,//报价
                iSum = 200,//价税合计
                iTaxRate = 17,//税率 
                DHID = "0",
                cBatch = null
            });
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "102111",
                cWhCode = "K001",//仓库编码
                iQuantity = 0,//数量
                iQuotedPrice = 0,//报价
                iSum = -90,//价税合计
                iTaxRate = 17,//税率 
                DHID = "0"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            //entity= System.Web.HttpUtility.UrlEncode(entity);

            string postUrl = string.Format(@"{0}={1}&ModelType=8005", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;

        }

        static string SendHttpRequest(string url, string requestString, string method)
        {
            string responseString = string.Empty;

            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            httpRequest.Method = method;
            httpRequest.KeepAlive = true;
            httpRequest.Accept = "text/html, application/xhtml+xml, */*";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            if (!string.IsNullOrEmpty(requestString))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(requestString);
                httpRequest.ContentLength = bytes.Length;
                //获得请 求流
                Stream writer = httpRequest.GetRequestStream();
                //将请求参数写入流
                writer.Write(bytes, 0, bytes.Length);
                // 关闭请求流
                writer.Close();
            }
            else
            {
                httpRequest.ContentLength = 0;
            }


            Stream requestStream = httpRequest.GetResponse().GetResponseStream();
            using (StreamReader sr = new StreamReader(requestStream, Encoding.UTF8))
            {
                responseString = sr.ReadToEnd();
            }

            return responseString;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string msg = "2016上海母婴护肤臻品展地(片区)礼包政策-珠润矜贵(孕产期&面膜含5&2)";
            byte[] bytes = Encoding.UTF8.GetBytes(msg);

            textBox1.Text = bytes.ToString();


        }

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
        /// <summary>
        /// 销售订单测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_somain_Click(object sender, EventArgs e)
        {
            EntitySaleHead doc = new EntitySaleHead();
            doc.cSOCode = "DHOR2016121200091";
            doc.dDate = DateTime.Parse("2016-12-16");
            doc.cBusType = "普通销售";
            doc.cSTCode = "1";
            doc.cCusCode = "0103094";
            doc.cPayCode = "";
            doc.cDepCode = "SZ300105";
            doc.iTaxRate = 17;
            doc.cDefine1 = "合同";
            doc.cDefine8 = "提付";
            doc.cDefine12 = "ZZ006";
            doc.cCusOAddress = "陶健15169062406山东省济南市天桥区蓝翔路卢庄工业园1区16号";
            doc.cMemo = "测试";
            List<EntitySaleBody> list = new List<EntitySaleBody>();
            list.Add(new EntitySaleBody
            {
                cInvCode = "1050200314",
                iQuantity = 261,
                iQuotedPrice = 0,
                iSum = 0,
                iTaxRate = 17,
                DHID = "0",
                cDefine22 = "123",
                cDefine23 = "10"
            });

            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            //entity= System.Web.HttpUtility.UrlEncode(entity);

            string postUrl = string.Format(@"{0}={1}&ModelType=8001", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_rdrecord09_Click(object sender, EventArgs e)
        {
            EntityRdRecord09Head doc = new EntityRdRecord09Head();
            doc.cCode = "20171252211";
            doc.dDate = DateTime.Parse("2017-12-04");
            doc.cWhCode = "005";
            doc.cRdCode = "201";
            doc.cDepCode = "01";
            doc.cDefine1 = "物料支持";
            doc.cCusCode = "0101";
            doc.cDefine8 = "提付";
            doc.cDefine10 = "";
            doc.cShipAddress = "";
            doc.cMemo = "";
            //doc.cMaker = "";            
            List<EntityRdRecord09Body> list = new List<EntityRdRecord09Body>();
            list.Add(new EntityRdRecord09Body
            {
                cInvCode = "1010100001",
                iQuantity = -1,
                cDefine26 = 220,
                cDefine27 = 2200,
                DHID = "0",
                cDefine22 = "6922401172510",
                cDefine23 = "",
                cBatch = "102"
            });
            //list.Add(new EntityRdRecord09Body
            //{
            //    cInvCode = "10201080",
            //    iQuantity = 10,
            //    cDefine26 = 220,
            //    cDefine27 = 2200,
            //    DHID = "0",
            //    cDefine22 = "6922401172510",
            //    cDefine23 = ""
            //});

            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            //entity= System.Web.HttpUtility.UrlEncode(entity);

            string postUrl = string.Format(@"{0}={1}&ModelType=8002", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;

        }

        private void bt_dispatchth_Click(object sender, EventArgs e)
        {
            EntityDispatchListHead doc = new EntityDispatchListHead();
            doc.cSTCode = "2";
            doc.dDate = DateTime.Parse("2016-12-19");
            doc.cDepCode = "SZ300115";
            doc.cCusCode = "0207006";
            doc.cDLCode = "TH20161100000042";
            doc.cDefine1 = "合同";
            doc.cDefine3 = "是";
            List<EntityDispatchListBody> list = new List<EntityDispatchListBody>();
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "1070200902",
                cWhCode = "CD049",//仓库编码
                iQuantity = -1,//数量
                iQuotedPrice = 369,//报价
                iSum = -369,//价税合计
                iTaxRate = 17,//税率 
                DHID = "0"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            //entity= System.Web.HttpUtility.UrlEncode(entity);

            string postUrl = string.Format(@"{0}={1}&ModelType=8003", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_apclosebill_Click(object sender, EventArgs e)
        {
            EntityAp_CloseBillHead doc = new EntityAp_CloseBillHead();
            doc.cVouchType = "48";
            doc.cVouchID = "HKTZ201612090017";
            doc.dVouchDate = DateTime.Parse("2018-03-01");
            doc.cDwCode = "0108135";
            doc.cDeptCode = "SZ300115";
            doc.cSSCode = "1";        
            doc.cDigest = "test";
            doc.iAmount = 888;
            doc.cDefine9 = "汇款通知单";
            doc.cDefine10 = "232323";
            List<EntityAp_CloseBillBody> list = new List<EntityAp_CloseBillBody>();
            list.Add(new EntityAp_CloseBillBody
            {
                iAmt = 888,
                cDefine22 = "合同",
                iType = 1,
                DHID = "0"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8004", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;



        }

        private void bt_apvouch_Click(object sender, EventArgs e)
        {
            EntityAp_VouchHead doc = new EntityAp_VouchHead();
            doc.cVouchID = "PL00820161100006";
            doc.dVouchDate = DateTime.Parse("2018-04-09");
            doc.cDwCode = "SZ0053";
            doc.iAmount_f = 888;
            doc.cDigest = "234";
            List<EntityAp_VouchBody> list = new List<EntityAp_VouchBody>();
            list.Add(new EntityAp_VouchBody
            {
                iAmount_f = 888,
                cCode = "1801",
                cDigest="123",
            }
            );
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8011", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;



        }

        private void bt_pomain_Click(object sender, EventArgs e)
        {
            EntityPO_Pomain doc = new EntityPO_Pomain();
            doc.cPOID = "11611314";
            doc.dPODate = DateTime.Parse("2018-4-09");
            doc.cVenCode = "SZ0046";
            List<EntityPO_PoDetails> list = new List<EntityPO_PoDetails>();
            list.Add(new EntityPO_PoDetails
                {
                    cInvCode = "403103",
                    iQuantity = 500,
                    iTaxPrice = decimal.Parse("4.69"),
                    iPerTaxRate = 17,
                    iSum = decimal.Parse("2345")
                }
                );

            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8015", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_rdrecord01_Click(object sender, EventArgs e)
        {
            EntityRdRecord01Head doc = new EntityRdRecord01Head();
            doc.cCode = "20171215625";
            doc.cPTCode = "01";
            doc.cWhCode = "005";
            doc.dDate = DateTime.Parse("2017-12-04");
            doc.cVenCode = "0101001";
            doc.cMaker = "";
            doc.cRdCode = "101";
            doc.cOrderCode = "";
            List<Entityrdrecords01Body> list = new List<Entityrdrecords01Body>();
            list.Add(new Entityrdrecords01Body
            {
                cInvCode = "1010100001",
                iQuantity = 1,
                iOriTaxCost = decimal.Parse("3.74"),
                iTaxRate = 17,
                ioriSum = decimal.Parse("3.74"),
                cBatch = "106"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8016", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;

        }

        private void bt_pay_Click(object sender, EventArgs e)
        {
            EntityAp_CloseBillHead doc = new EntityAp_CloseBillHead();
            doc.cVouchType = "49";
            doc.cVouchID = "HKTZ201612090017";
            doc.dVouchDate = DateTime.Parse("2016-12-19");
            doc.cDwCode = "SZ0066";
            doc.cDeptCode = "SZ0702";
            doc.cSSCode = "1";
            doc.cDigest = "";
            doc.iAmount = 888;
            doc.cDefine9 = "业务费核销单";
            doc.cDefine10 = "HXTZ201612090017";
            List<EntityAp_CloseBillBody> list = new List<EntityAp_CloseBillBody>();
            list.Add(new EntityAp_CloseBillBody
            {
                iAmt = 888,
                cDefine22 = "合同",
                DHID = "0"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8018", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        static string QueryCode(int mt, string entity)
        {
            string geturl = "http://localhost:45249/U8Api/GetModels?";
            string getpara = "UserCode=demo&CipherPassword=5246543K4Y3K&StrAccID=006&Act=query&Entity";
            string postUrl = string.Format(@"{0}={1}&ModelType={2}", getpara, entity, mt);
            return SendHttpRequest(geturl, postUrl, "POST");

        }

        static string GetCode(int mt, string entity)
        {
            string geturl = "http://localhost:45249/U8Api/GetModels?";
            string getpara = "UserCode=demo&CipherPassword=5246543K4Y3K&StrAccID=006&Act=get&Entity";
            string postUrl = string.Format(@"{0}={1}&ModelType={2}", getpara, entity, mt);
            return SendHttpRequest(geturl, postUrl, "POST");

        }

        private void bt_getso_Click(object sender, EventArgs e)
        {
            EntitySaleHead doc = new EntitySaleHead();
            doc.cSOCode = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8001, JsonConvert.SerializeObject(doc));
        }

        private void bt_getdisth_Click(object sender, EventArgs e)
        {
            EntityDispatchListHead doc = new EntityDispatchListHead();
            doc.cDLCode = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8003, JsonConvert.SerializeObject(doc));
        }

        private void bt_getrd09_Click(object sender, EventArgs e)
        {
            EntityRdRecord09Head doc = new EntityRdRecord09Head();
            doc.cCode = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8002, JsonConvert.SerializeObject(doc));
        }

        private void bt_getdis_Click(object sender, EventArgs e)
        {
            EntityDispatchListHead doc = new EntityDispatchListHead();
            doc.cDLCode = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8005, JsonConvert.SerializeObject(doc));
        }

        private void bt_getpo_Click(object sender, EventArgs e)
        {
            EntityPO_Pomain doc = new EntityPO_Pomain();
            doc.cPOID = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8015, JsonConvert.SerializeObject(doc));
        }

        private void bt_sk_Click(object sender, EventArgs e)
        {
            EntityAp_CloseBillHead doc = new EntityAp_CloseBillHead();
            doc.cVouchID = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8004, JsonConvert.SerializeObject(doc));
        }

        private void bt_payap_Click(object sender, EventArgs e)
        {
            EntityAp_CloseBillHead doc = new EntityAp_CloseBillHead();
            doc.cVouchID = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8017, JsonConvert.SerializeObject(doc));
        }

        private void bt_getapvouch_Click(object sender, EventArgs e)
        {
            EntityAp_VouchHead doc = new EntityAp_VouchHead();
            doc.cVouchID = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8011, JsonConvert.SerializeObject(doc));
        }

        private void bt_rd01_Click(object sender, EventArgs e)
        {
            EntityRdRecord01Head doc = new EntityRdRecord01Head();
            doc.cCode = tx_cCode.Text == "" ? "1" : tx_cCode.Text;
            textBox1.Text = QueryCode(8016, JsonConvert.SerializeObject(doc));
        }

        private void bt_AppTransVouch_Click(object sender, EventArgs e)
        {
            EntityST_AppTransVouch doc = new EntityST_AppTransVouch();
            doc.cTvCode = "TSZ170700000001";
            doc.dTvDate = DateTime.Parse("2017-7-1");
            doc.cOutWhCode = "K001";
            doc.cInWhCode = "HK005";
            doc.cOutDepCode = "SZ110101";
            doc.cInDepCode = "SZ110101";
            doc.cCusCode = "0204001";
            //doc.cOutRdCode = "20";
            //doc.cInRdCode = "03";
            doc.cMaker = "";
            doc.cTvMemo = "测试接口";
            List<EntityST_AppTransVouchBody> list = new List<EntityST_AppTransVouchBody>();
            list.Add(new EntityST_AppTransVouchBody
            {
                cInvCode = "1020208009",
                iTvQuantity = 10,
                iUnitCost = 12,
                iPrice = 120,
                cDefine26 = 118,
                cDefine27 = 1180
            });
            list.Add(new EntityST_AppTransVouchBody
            {
                cInvCode = "1020208010",
                iTvQuantity = 10,
                iUnitCost = 10,
                iPrice = 100,
                cDefine26 = 128,
                cDefine27 = 1280
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8020", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;

        }

        private void bt_TransVouch_Click(object sender, EventArgs e)
        {
            EntityTransVouch doc = new EntityTransVouch();
            doc.cTvCode = "TSZ170700000001";
            doc.dTvDate = DateTime.Parse("2017-7-01");
            doc.cOutWhCode = "K001";
            doc.cInWhCode = "HK005";
            doc.cOutDepCode = "SZ110101";
            doc.cInDepCode = "SZ110101";
            doc.cCusCode = "0204001";
            //doc.cOutRdCode = "20";
            //doc.cInRdCode = "03";
            doc.cMaker = "";
            doc.cTvMemo = "测试接口";
            List<EntityTransVouchBody> list = new List<EntityTransVouchBody>();
            list.Add(new EntityTransVouchBody
            {
                cInvCode = "1020208009",
                iTvQuantity = 10,
                iTvaCost = 12,
                iTvaPrice = 120,
                cDefine26 = 118,
                cDefine27 = 1180
            });
            list.Add(new EntityTransVouchBody
            {
                cInvCode = "1020208010",
                iTvQuantity = 10,
                iTvaCost = 10,
                iTvaPrice = 100,
                cDefine26 = 128,
                cDefine27 = 1280
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8021", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_Vendor_Click(object sender, EventArgs e)
        {
            EntityVendor doc = new EntityVendor();
            doc.cVenCode = "SZ0075";
            doc.cVenName = "测试";
            doc.cVenAbbName = "测试";
            doc.cVcCode = "01";
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8009", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_CheckVouch_Click(object sender, EventArgs e)
        {
            EntityCheckVouchHead doc = new EntityCheckVouchHead();
            doc.cCVCode = "TSZ000000000001";
            doc.dCVDate = DateTime.Parse("2017-7-1");
            doc.cWhCode = "K001";
            doc.dACDate = DateTime.Parse("2017-7-1");
            doc.cMaker = "";
            doc.cCVMemo = "测试接口";
            List<EntityCheckVouchBody> list = new List<EntityCheckVouchBody>();
            list.Add(new EntityCheckVouchBody
            {
                cInvCode = "1020208009",
                iCVQuantity = 0,
                iCVCQuantity = 2
            });
            list.Add(new EntityCheckVouchBody
            {
                cInvCode = "1020208010",
                iCVQuantity = 0,
                iCVCQuantity = 1
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8022", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_inventory_Click(object sender, EventArgs e)
        {
            //EntityInventory doc = new EntityInventory();
            //doc.cInvCode = "test123";
            //doc.cInvAddCode = "123";
            //doc.cInvName = "测试存货123";
            //doc.cInvCCode = "101";
            //doc.cBarCode = "201707111513";
            //doc.cComUnitCode = "个";
            //doc.cInvStd = "4*4";
            //doc.cPackingType = "4箱*4";
            //doc.cValueType = "全月平均法";
            //doc.fRetailPrice = 50;
            //doc.iInvSPrice = 8;
            //doc.cInvDefine11 = 10;
            //doc.cInvDefine13 = 25;
            //doc.cInvDefine14 = 4;
            Dictionary<string,string> dic = new Dictionary<string,string>();
            dic.Add("cInvCode","test123");
            dic.Add("cInvAddCode","123");
            dic.Add("cInvName", "测试存货123");
            dic.Add("cInvCCode", "101");
            dic.Add("cBarCode","201707111513");
            dic.Add("cComUnitCode","个");
            dic.Add("cInvStd","4*4");
            dic.Add("cPackingType","4箱*4");
            dic.Add("cValueType","全月平均法");
            dic.Add("fRetailPrice","50");
            dic.Add("iInvSPrice","8");
            dic.Add("cInvDefine11","10");
            dic.Add("cInvDefine13","25");
            dic.Add("cInvDefine14","4");
            string entity = JsonConvert.SerializeObject(dic);

            string postUrl = string.Format(@"{0}={1}&ModelType=8012", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;

        }

        private void bt_inventoryclass_Click(object sender, EventArgs e)
        {
            EntityInventoryClass doc = new EntityInventoryClass();
            doc.cInvCCode = "1888888";
            doc.cInvCName = "测试存货分类";
            doc.iInvCGrade = 2;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8023", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;

        }

        private void bt_warehouse_Click(object sender, EventArgs e)
        {
            EntityWareHouse doc = new EntityWareHouse();
            doc.cWhCode = "test321";
            doc.cWhName = "测试仓库321";
            doc.cWhPerson = "demo";
            doc.cWhPhone = "1882375487";
            doc.cWhAddress = "清华紫光信息港";
            doc.bShop = 1;
            doc.cWhValueStyle = "全月平均法";
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8013", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_customer_Click(object sender, EventArgs e)
        {
            EntityCustomer doc = new EntityCustomer();
            doc.cCusCode = "test110";
            doc.cCusName = "测试客户110";
            doc.cCusAbbName = "测试客户110";
            doc.cCCCode = "SZ01";
            doc.cCusDepart = "SZ300104";
            doc.cCusHand = "18836241241";
            doc.cCusAddress = "紫光信息港";
            doc.cCusDefine1 = "1+5渠道";
            doc.cCusPerson = "张三";
            string entity =System.Web.HttpUtility.UrlEncode(JsonConvert.SerializeObject(doc));

            string postUrl = string.Format(@"{0}={1}&ModelType=8008", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_getInvPrice_Click(object sender, EventArgs e)
        {
            EntityInventoryPrice doc = new EntityInventoryPrice();
            doc.cInvCode = "1010100001";
            doc.cVenCode = "09001";
            doc.noSettleTaxCost = 0;
            doc.SettleTaxCost = 0;
            doc.VenInvPrice = 0;
            string entity = System.Web.HttpUtility.UrlEncode(JsonConvert.SerializeObject(doc));
            textBox1.Text = GetCode(8024, entity);
        }

        private void bt_wtdispatchlist_Click(object sender, EventArgs e)
        {
            EntityDispatchListHead doc = new EntityDispatchListHead();
            doc.cSTCode = "2";
            doc.dDate = DateTime.Parse("2017-11-3");
            doc.cDepCode = "SZ300127";
            doc.cCusCode = "0220022";
            doc.cDLCode = "WT20171100000001";
            doc.cDefine1 = "合同";
            doc.cDefine3 = "是";
            List<EntityDispatchListBody> list = new List<EntityDispatchListBody>();
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "1070200307",
                cWhCode = "SZ094",//仓库编码
                iQuantity = 1,//数量
                iQuotedPrice = 200,//报价
                iSum = 200,//价税合计
                iTaxRate = 17,//税率 
                DHID = "0"
            });
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "102111",
                cWhCode = "SZ094",//仓库编码
                iQuantity = 0,//数量
                iQuotedPrice = 0,//报价
                iSum = -90,//价税合计
                iTaxRate = 0,//税率 
                DHID = "0"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            //entity= System.Web.HttpUtility.UrlEncode(entity);

            string postUrl = string.Format(@"{0}={1}&ModelType=8025", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_wtdispatchth_Click(object sender, EventArgs e)
        {
            EntityDispatchListHead doc = new EntityDispatchListHead();
            doc.cSTCode = "2";
            doc.dDate = DateTime.Parse("2017-11-03");
            doc.cDepCode = "SZ300115";
            doc.cCusCode = "0207006";
            doc.cDLCode = "TWT20171100000001";
            doc.cDefine1 = "合同";
            doc.cDefine3 = "是";
            List<EntityDispatchListBody> list = new List<EntityDispatchListBody>();
            list.Add(new EntityDispatchListBody
            {
                cInvCode = "1070200902",
                cWhCode = "CD049",//仓库编码
                iQuantity = -1,//数量
                iQuotedPrice = 369,//报价
                iSum = -369,//价税合计
                iTaxRate = 17,//税率 
                DHID = "0"
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            //entity= System.Web.HttpUtility.UrlEncode(entity);

            string postUrl = string.Format(@"{0}={1}&ModelType=8026", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_Rdrecord01T_Click(object sender, EventArgs e)
        {
            EntityRdRecord01Head doc = new EntityRdRecord01Head();
            doc.cCode = "SZPL012201701000023";
            doc.cPTCode = "1";
            doc.cWhCode = "K001";
            doc.dDate = DateTime.Parse("2017-11-06");
            doc.cVenCode = "09003";
            doc.cMaker = "";
            doc.cRdCode = "01";
            doc.cOrderCode = "";
            List<Entityrdrecords01Body> list = new List<Entityrdrecords01Body>();
            list.Add(new Entityrdrecords01Body
            {
                cInvCode = "443372",
                iQuantity = -1000,
                iOriTaxCost = decimal.Parse("3.43"),
                iTaxRate = 17,
                ioriSum = decimal.Parse("-3430")
            });
            doc.Details = list;
            string entity = JsonConvert.SerializeObject(doc);

            string postUrl = string.Format(@"{0}={1}&ModelType=8027", addpara, entity);
            string result = SendHttpRequest(addurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_DelRdRecord01_Click(object sender, EventArgs e)
        {
            //EntityRdRecord01Head doc = new EntityRdRecord01Head();
            //doc.cCode = "20171215625";
            string entity = JsonConvert.SerializeObject(new { cCode = "20171215625" });
            string postUrl = string.Format(@"{0}={1}&ModelType=8016", delpara, entity);
            string result = SendHttpRequest(delurl, postUrl, "POST");
            textBox1.Text = result;
        }

        private void bt_DelDispatchlist_Click(object sender, EventArgs e)
        {
            //EntityDispatchListHead doc = new EntityDispatchListHead();
            //doc.cDLCode = "2017120004";
            string entity = JsonConvert.SerializeObject(new { cDLCode = "2017120004" });
            string postUrl = string.Format(@"{0}={1}&ModelType=8005", delpara, entity);
            string result = SendHttpRequest(delurl, postUrl, "POST");
            textBox1.Text = result;
        }


    }



}
