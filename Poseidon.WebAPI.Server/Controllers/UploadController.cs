using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Poseidon.WebAPI.Server.Controllers
{
    using Poseidon.Base.Framework;
    using Poseidon.Common;
    using Poseidon.Archives.Core.BL;
    using Poseidon.Archives.Core.DL;
    using Poseidon.WebAPI.Server.Utility;

    /// <summary>
    /// 上传控制器
    /// </summary>
    public class UploadController : ApiController
    {
        #region Function
        /// <summary>
        /// 生成保存文件夹
        /// </summary>
        /// <returns></returns>
        private string GeneratePath()
        {
            string folder = string.Format("{0}-{1:D2}", DateTime.Now.Year, DateTime.Now.Month);

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

            if (file.Headers.Contains("md5hash"))
            {
                string md5 = file.Headers.GetValues("md5hash").ToList().First();

                if (attachment.MD5Hash != md5)
                {
                    throw new Exception("文件哈希计算不匹配");
                }
            }

            attachment.Name = provider.FormData["name"];
            attachment.Remark = provider.FormData["remark"];

            BusinessFactory<AttachmentBusiness>.Instance.Create(attachment);

            return attachment;
        }
        #endregion //Function

        #region Action
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> PostFileDataAsync()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = AppConfig.GetAppSetting("UploadPath");
            var folder = GeneratePath();

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
        [HttpGet]
        public HttpResponseMessage DownloadFile(string id)
        {
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
        #endregion //Action
    }
}
