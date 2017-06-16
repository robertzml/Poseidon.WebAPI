using System;
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

    public class UploadController : ApiController
    {
        public async Task<HttpResponseMessage> PostFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string path = AppConfig.GetAppSetting("UploadPath");
            string time = string.Format("{0}-{1:D2}", DateTime.Now.Year, DateTime.Now.Month);

            var root = HttpContext.Current.Server.MapPath("~" + path + "//" + time);

            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);


            //string root = HttpContext.Current.Server.MapPath("~/App_Data/Upload");
            var provider = new PoseidonMultipartFormDataStreamProvider(root);
         
            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                List<string> returns = new List<string>();
                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {                    
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);

                    returns.Add(file.LocalFileName);
                }
                
                return Request.CreateResponse(HttpStatusCode.OK, returns);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

    }
}
