using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace JR.HL
{
    public class WebHttpRequest
    {
        /// <summary>
        /// 发送Http请求,并读取返回信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestString"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string SendHttpRequest(string url, string requestString, string method)
        {
            string responseString = string.Empty;

            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            httpRequest.Method = method;
            httpRequest.KeepAlive = true;
            httpRequest.Accept = "text/html, application/xhtml+xml, */*";
            httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";// "application/x-www-form-urlencoded";

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
    }
}
