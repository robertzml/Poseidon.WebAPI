﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;
using System.IO;

namespace Poseidon.WebAPI.Client.Controllers
{
    using Poseidon.Common;
    using Poseidon.WebAPI.Client.Utility;
    using Poseidon.WebAPI.Core.BL;
    using Poseidon.WebAPI.Core.DL;

    /// <summary>
    /// 上传控制器
    /// </summary>
    public class UploadController : ApiController
    {
        #region Function
        /// <summary>
        /// 生成保存路径
        /// </summary>
        /// <returns></returns>
        private string GeneratePath()
        {
            string path = AppConfig.GetAppSetting("UploadPath");
            string date = string.Format("{0}-{1:D2}", DateTime.Now.Year, DateTime.Now.Month);

            var root = HttpContext.Current.Server.MapPath("~" + path + "//" + date);

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            return root;
        }

        private List<string> SaveAttchment(PoseidonMultipartFormDataStreamProvider provider)
        {
            List<string> returns = new List<string>();
            
            foreach (MultipartFileData file in provider.FileData)
            {
                Attachment attachment = new Attachment();

                attachment.Name = file.Headers.ContentDisposition.Name;
                attachment.FileName = file.LocalFileName;

                IEnumerable<string> remarks;
                file.Headers.TryGetValues("remark", out remarks);
                
                int a = remarks.Count();
            }

            return returns;
        }
        #endregion //Function

        #region Action
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var root = GeneratePath(); 
            var provider = new PoseidonMultipartFormDataStreamProvider(root);
         
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                SaveAttchment(provider);

                List<string> returns = new List<string>();
                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {                    
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    //file.Headers.ContentDisposition.n
                    long length = file.Headers.ContentLength ?? 0;


                    returns.Add(file.LocalFileName);
                }
                
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
