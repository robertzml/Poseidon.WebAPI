using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Poseidon.WebAPI.Client.Controllers
{
    using Poseidon.Base.Framework;
    using Poseidon.Common;
    using Poseidon.WebAPI.Client.Utility;
    using Poseidon.WebAPI.Core.BL;
    using Poseidon.WebAPI.Core.DL;

    /// <summary>
    /// 附件控制器
    /// </summary>
    public class AttachmentController : ApiController
    {
        #region Action
        /// <summary>
        /// 获取所有附件
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult Get()
        {
            var data = BusinessFactory<AttachmentBusiness>.Instance.FindAll();
            return Ok(data);
        }
        #endregion //Action
    }
}
