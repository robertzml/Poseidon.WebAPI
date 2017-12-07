using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Poseidon.WebAPI.Server.Utility
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
        /// <summary>
        /// 获取本地文件名
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            string filePath = headers.ContentDisposition.FileName;

            filePath = filePath.Replace(@"""", string.Empty);

            var filename = Path.GetFileName(filePath);
            var name = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(filePath).ToLower();

            return name + extension;
        }
        #endregion //Method
    }
}