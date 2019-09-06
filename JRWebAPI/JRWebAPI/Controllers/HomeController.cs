using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace JRWebAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetTest(string UserId,string Password)
        {
            
            StringBuilder sb = new StringBuilder();
            //List<Test> list = new List<Test>();
            //list.Add(new Test() { UserId = "abc", Password = "1233" });
            //return Json(list);
            return "[{\"UserId\":\"abc\",\"Password\":1233}]";
        }
    }

    public class Test
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
