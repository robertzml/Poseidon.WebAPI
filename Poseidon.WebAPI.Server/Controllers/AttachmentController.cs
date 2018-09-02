using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Http.Headers;

namespace Poseidon.WebAPI.Server.Controllers
{
    using Poseidon.Archives.Core.BL;
    using Poseidon.Archives.Core.DL;
    using Poseidon.Base.Framework;
    using Poseidon.Common;
    using Poseidon.WebAPI.Server.Utility;

    /// <summary>
    /// 附件控制器
    /// </summary>
    public class AttachmentController : ApiController
    {
        #region Function
        /// <summary>
        /// 生成保存文件夹
        /// </summary>
        /// <param name="module">模块名</param>
        /// <returns></returns>
        private string GeneratePath(string module)
        {
            string folder = string.Format("{0}/{1}-{2:D2}", module, DateTime.Now.Year, DateTime.Now.Month);

            return folder;
        }

        /// <summary>
        /// 保存附件信息到数据库
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="folder">保存文件夹</param>
        /// <returns></returns>
        private Attachment SaveAttchment(PoseidonMultipartFormDataStreamProvider provider, string folder)
        {
            MultipartFileData file = provider.FileData[0];

            Attachment attachment = new Attachment();
            attachment.FileName = Path.GetFileName(file.LocalFileName);
            attachment.OriginName = file.Headers.ContentDisposition.FileName;
            attachment.Extension = Path.GetExtension(file.LocalFileName);
            attachment.ContentType = file.Headers.ContentType.MediaType;
            attachment.UploadTime = DateTime.Now;
            attachment.Size = new FileInfo(file.LocalFileName).Length;
            attachment.Folder = folder;
            attachment.MD5Hash = Hasher.GetFileMD5Hash(file.LocalFileName);

            attachment.Name = provider.FormData["name"];
            attachment.Remark = provider.FormData["remark"];

            string md5 = provider.FormData["md5hash"];
            if (attachment.MD5Hash != md5)
            {
                throw new Exception("文件哈希计算不匹配");
            }

            BusinessFactory<AttachmentBusiness>.Instance.Create(attachment);

            return attachment;
        }
        #endregion //Function

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

        /// <summary>
        /// 异步上传单个附件
        /// </summary>
        /// <returns></returns>
        [Route("api/attachment/async-upload")]
        [HttpPost]
        public async Task<HttpResponseMessage> AsyncUpload()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var module = "Default";
            var headerModule = Request.Headers.GetValues("module").ToList();
            if (headerModule.Count > 0)
                module = headerModule[0];

            string root = AppConfig.GetAppSetting("UploadPath");
            var folder = GeneratePath(module);

            var path = HttpContext.Current.Server.MapPath("~" + root + "//" + folder);

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            var provider = new PoseidonMultipartFormDataStreamProvider(path);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.FileData.Count == 0)
                    return Request.CreateResponse(HttpStatusCode.Forbidden);

                var attachment = SaveAttchment(provider, folder);

                return Request.CreateResponse(HttpStatusCode.OK, attachment);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="id">附件ID</param>
        /// <returns></returns>
        [Route("api/attachment/download/{id}")]
        [HttpGet]
        public HttpResponseMessage DownloadFile(string id)
        {
            try
            {
                Logger.Instance.Info("下载文件:" + id);
                var attachment = BusinessFactory<AttachmentBusiness>.Instance.FindById(id);
                if (attachment == null) //文件不存在
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                string root = AppConfig.GetAppSetting("UploadPath");
                string folder = HttpContext.Current.Server.MapPath("~" + root + "//" + attachment.Folder);
                string path = folder + "//" + attachment.FileName;

                if (!File.Exists(path)) //文件已删除
                    return Request.CreateResponse(HttpStatusCode.Gone);

                FileStream fs = new FileStream(path, FileMode.Open);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(fs);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(attachment.ContentType);
                response.Content.Headers.ContentType.CharSet = "utf-8";
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = attachment.OriginName;
                response.Content.Headers.Add("md5hash", attachment.MD5Hash);

                return response;
            }
            catch(Exception e)
            {
                Logger.Instance.Exception("下载文件出错", e);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="id">附件ID</param>
        /// <returns></returns>
        [Route("api/attachment/delete/{id}")]
        [HttpGet]
        public HttpResponseMessage DeleteFile(string id)
        {
            var attachment = BusinessFactory<AttachmentBusiness>.Instance.FindById(id);
            if (attachment == null) //文件不存在
                return Request.CreateResponse(HttpStatusCode.NotFound);

            string root = AppConfig.GetAppSetting("UploadPath");
            string folder = HttpContext.Current.Server.MapPath("~" + root + "//" + attachment.Folder);
            string path = folder + "//" + attachment.FileName;

            File.Delete(path);

            BusinessFactory<AttachmentBusiness>.Instance.Delete(id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        #endregion //Action
    }
}
