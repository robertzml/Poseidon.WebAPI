using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Poseidon.WebAPI.Server.Controllers
{
    using Poseidon.Base.Framework;
    using Poseidon.Common;
    using Poseidon.Attachment.Core.BL;
    using Poseidon.Attachment.Core.DL;

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
        public HttpResponseMessage Get()
        {
            var data = BusinessFactory<AttachmentBusiness>.Instance.FindAll();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }

        /// <summary>
        /// 获取指定附件
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public HttpResponseMessage GetById(string id)
        {
            var data = BusinessFactory<AttachmentBusiness>.Instance.FindById(id);
            if (data == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            //return Ok(data);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }
        #endregion //Action
    }
}
