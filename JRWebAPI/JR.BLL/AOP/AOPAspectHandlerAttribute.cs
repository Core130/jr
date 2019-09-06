//using PostSharp.Aspects;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;


//namespace JR.BLL.AOP
//{

//    [Serializable]
//    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
//    public class AOPAspectHandlerAttribute : MethodInterceptionAspect
//    {
//        public override void OnInvoke(MethodInterceptionArgs args)
//        {
//            try
//            {
//                bool before = OnBeforeDoing(args);
//                if (before)
//                {
//                    base.OnInvoke(args);
//                }
//                OnAfterDoing(args);
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        /// <summary>
//        /// 切面前置方法
//        /// </summary>
//        /// <param name="args"></param>
//        /// <returns></returns>
//        public virtual bool OnBeforeDoing(MethodInterceptionArgs args) { return true; }

//        /// <summary>
//        /// 切面后置方法
//        /// </summary>
//        /// <param name="args"></param>
//        public virtual void OnAfterDoing(MethodInterceptionArgs args) { }

//        /// <summary>
//        /// 异常处理
//        /// </summary>
//        /// <param name="ex"></param>
//        public virtual void OnException(Exception ex) { }
//    }
//}
