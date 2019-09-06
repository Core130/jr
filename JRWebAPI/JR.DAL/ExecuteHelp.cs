using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace JR.DAL
{
    /// <summary>
    /// 执行语句辅助类
    /// </summary>
    public class ExecuteHelp
    {
        /// <summary>
        /// SQL语句
        /// </summary>
        public string SQL { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public SqlParameter[] Parameters { get; set; }
    }
}
