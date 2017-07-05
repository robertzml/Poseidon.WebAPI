using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Poseidon.WebAPI.Server.Controllers
{
    using Poseidon.Base.Framework;
    using Poseidon.Common;
    using Poseidon.Attachment.Core.BL;
    using Poseidon.Attachment.Core.DL;
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
        private List<Attachment> SaveAttchment(PoseidonMultipartFormDataStreamProvider provider, string folder)
        {
            List<Attachment> data = new List<Attachment>();

            foreach (MultipartFileData file in provider.FileData)
            {
                Attachment attachment = new Attachment();

                attachment.Name = file.Headers.GetValues("name").ToList().First();
                attachment.FileName = Path.GetFileName(file.LocalFileName);
                attachment.Extension = Path.GetExtension(file.LocalFileName);
                attachment.ContentType = file.Headers.ContentType.MediaType;
                attachment.UploadTime = DateTime.Now;
                attachment.Size = new FileInfo(file.LocalFileName).Length;
                attachment.Folder = folder;
                attachment.MD5Hash = Hasher.GetFileMD5Hash(file.LocalFileName);

                if (file.Headers.Contains("remark"))
                    attachment.Remark = file.Headers.GetValues("remark").ToList().First();
                else
                    attachment.Remark = "";

                if (file.Headers.Contains("md5hash"))
                {
                    string md5 = file.Headers.GetValues("md5hash").ToList().First();

                    if (attachment.MD5Hash != md5)
                    {
                        throw new Exception("文件哈希计算不匹配");
                        continue;
                    }
                }

                var attchment = BusinessFactory<AttachmentBusiness>.Instance.Create(attachment);
                data.Add(attachment);
            }

            return data;
        }
        #endregion //Function

        #region Action
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> PostFileData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = AppConfig.GetAppSetting("UploadPath");
            var folder = GeneratePath();

            var path = HttpContext.Current.Server.MapPath("~" + root + "//" + folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var provider = new PoseidonMultipartFormDataStreamProvider(path);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                var returns = SaveAttchment(provider, folder);

                return Request.CreateResponse(HttpStatusCode.OK, returns);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        #endregion //Action
    }
}
