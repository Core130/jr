using JR.HL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JRWebAPI.Controllers
{
    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class APIExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {            
            filterContext.ExceptionHandled = true;
            SystemLogs.WriteError(filterContext.Exception);
            string result = ControllerHelp.GetReturnStr(0, filterContext.Exception.Message);
            SystemLogs.WriteInfo(string.Format("返回数据：{0} \r\n --------------------------------------------------------------------------------------------------------------------------------------------",
                    result));
            filterContext.RequestContext.HttpContext.Response.Write(result);
        }

    }

    /// <summary>
    /// Action过滤器
    /// </summary>
    public class APIActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Action执行前
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                string requestUrl = filterContext.HttpContext.Request.Url.ToString();
                string paraData = "";
                if (filterContext.HttpContext.Request.Form != null && filterContext.HttpContext.Request.Form.Count > 0)
                {
                    for (int i = 0; i < filterContext.HttpContext.Request.Form.Count; i++)
                    {
                        paraData += string.Format("{0}={1};", filterContext.HttpContext.Request.Form.AllKeys[i], filterContext.HttpContext.Request.Form[i]);

                    }
                }

                SystemLogs.WriteInfo(string.Format("接收到请求地址为：{0}，请求数据为：{1} \r\n",
                     requestUrl, paraData));
            }
            catch (Exception ex)
            {
                SystemLogs.WriteError(ex);
            }
        }

        /// <summary>
        /// 返回结果事件
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            try
            {
                ContentResult cr = (ContentResult)filterContext.Result;
                string result = cr.Content;

                SystemLogs.WriteInfo(string.Format("返回数据：{0} \r\n--------------------------------------------------------------------------------------------------------------------------------------------",
                     result));
            }
            catch (Exception ex)
            {
                SystemLogs.WriteError(ex);
            }
        }
    }
}