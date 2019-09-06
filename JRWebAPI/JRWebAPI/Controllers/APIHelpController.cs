using JR.BLL;
using JR.HL.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JRWebAPI.Controllers
{
    public class APIHelpController : Controller
    {
        //
        // GET: /APIHelp/

        public ActionResult HelpCenter()
        {
            return View();
        }


        [APIActionFilter]
        public string Encrypt(string plaintext)
        {            
            string str = DefineEncryptDecrypt.Encrypt(plaintext);
            return str;
        }

    }
}
