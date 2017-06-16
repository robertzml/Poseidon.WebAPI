using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Poseidon.WebAPI.Client.Utility
{
    /// <summary>
    /// 自定义上传数据流
    /// </summary>
    public class PoseidonMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        #region Constructor
        public PoseidonMultipartFormDataStreamProvider(string rootPath)
            : base(rootPath)
        {            
        }
        #endregion //Constructor

        #region Method
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string filePath = headers.ContentDisposition.FileName;

            // Multipart requests with the file name seem to always include quotes.
            if (filePath.StartsWith(@"""") && filePath.EndsWith(@""""))
                filePath = filePath.Substring(1, filePath.Length - 2);

            var filename = Path.GetFileName(filePath);
            var extension = Path.GetExtension(filePath);
            var contentType = headers.ContentType.MediaType;

            return filename;
        }
        #endregion //Method
    }
}