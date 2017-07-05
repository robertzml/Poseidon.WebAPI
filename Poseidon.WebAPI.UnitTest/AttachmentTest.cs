using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Poseidon.WebAPI.UnitTest
{
    using Poseidon.Base.Framework;
    using Poseidon.Common;
    using Poseidon.Attachment.Core.BL;

    /// <summary>
    /// 附件模块测试
    /// </summary>
    [TestClass]
    public class AttachmentTest
    {
        #region Constructor
        public AttachmentTest()
        {
            // 设置连接字符串
            Cache.Instance.Add("ConnectionString", "mongodb://localhost:27017");

            // 设置数据库类型
            Cache.Instance.Add("DALPrefix", "Mongo");
        }
        #endregion //Constructor

        #region Test
        /// <summary>
        /// 附件模块测试
        /// </summary>
        [TestMethod]
        public void Test1()
        {
            var bl = BusinessFactory<AttachmentBusiness>.Instance;

            Assert.IsNotNull(bl);

            var data = bl.FindAll();

            Assert.IsTrue(data.Count() > 0);
        }
        #endregion //Test
    }
}
