using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Poseidon.WebAPI.Server.Controllers
{
    using Poseidon.Common;
    using System.Net.Http.Headers;

    /// <summary>
    /// 文件控制器
    /// 用于查看服务器保存文件
    /// </summary>
    public class FileController : ApiController
    {
        #region Action
        /// <summary>
        /// 获取文件夹列表
        /// </summary>
        /// <param name="parent">上级目录</param>
        /// <returns></returns>
        public HttpResponseMessage GetFolders(string parent)
        {
            string root = AppConfig.GetAppSetting("UploadPath");
            var path = HttpContext.Current.Server.MapPath("~" + root + "//" + parent);

            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var data = dir.GetDirectories();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="folder">目录</param>
        /// <returns></returns>
        public HttpResponseMessage GetFiles(string folder)
        {
            string root = AppConfig.GetAppSetting("UploadPath");
            var path = HttpContext.Current.Server.MapPath("~" + root + "//" + folder);

            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var data = dir.GetFiles();

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, data);
            return response;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DownloadFile(string folder, string filename)
        {
            string root = AppConfig.GetAppSetting("UploadPath");
            var filePath = HttpContext.Current.Server.MapPath("~" + root + "//" + folder + "//" + filename);

            if (!File.Exists(filePath)) //文件已删除
                return Request.CreateResponse(HttpStatusCode.NotFound);

            FileStream fs = new FileStream(filePath, FileMode.Open);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(fs);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(filename));
            response.Content.Headers.ContentType.CharSet = "utf-8";
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = filename;

            return response;
        }
        #endregion //Action
    }
}
