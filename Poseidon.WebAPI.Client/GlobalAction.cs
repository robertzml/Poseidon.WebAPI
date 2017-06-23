using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poseidon.WebAPI.Client
{
    using Poseidon.Base.Framework;
    using Poseidon.Base.System;
    using Poseidon.Core.Utility;
    using Poseidon.Common;

    /// <summary>
    /// 全局操作类
    /// </summary>
    public static class GlobalAction
    {
        #region Field
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static LoginUser CurrentUser = null;
        #endregion //Field

        #region Constructor

        #endregion //Constructor

        #region Method
        /// <summary>
        /// 全局初始化
        /// </summary>
        public static void Initialize()
        {
            // 设置连接字符串
            string source = AppConfig.GetAppSetting("ConnectionSource");
            if (string.IsNullOrEmpty(source))
                throw new PoseidonException(ErrorCode.DatabaseConnectionNotFound);

            string connection = "";
            if (source == "dbconfig")
            {
                string name = AppConfig.GetAppSetting("DbConnection");
                connection = ConfigUtility.GetConnectionString(name);
            }
            else if (source == "appconfig")
            {
                connection = AppConfig.GetConnectionString();
            }

            if (string.IsNullOrEmpty(connection))
                throw new PoseidonException(ErrorCode.DatabaseConnectionNotFound);

            Cache.Instance.Add("ConnectionString", connection);
        }
        #endregion //Method
    }
}
