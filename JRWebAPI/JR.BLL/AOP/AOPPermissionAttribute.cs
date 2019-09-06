//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;

//namespace JR.BLL.AOP
//{
//    [Serializable]
//    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
//    public class AOPPermissionAttribute : AOPAspectHandlerAttribute
//    {
//        public string UserId { get; set; }
//        public string UserPassword { get; set; }
//        public override bool OnBeforeDoing(PostSharp.Aspects.MethodInterceptionArgs args)
//        {
//            return base.OnBeforeDoing(args);
//        }       
//    }
//}
