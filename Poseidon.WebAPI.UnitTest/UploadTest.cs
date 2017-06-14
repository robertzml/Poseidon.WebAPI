using System;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Poseidon.WebAPI.UnitTest
{
    [TestClass]
    public class UploadTest
    {
        private string host = "http://localhost:57123/api/upload";

        public async Task<HttpResponseMessage> Post(string filePath)
        {
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 15);

            try
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                // 读取文件的 byte[]   
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
                // 把 byte[] 转换成 Stream   
                Stream stream = new MemoryStream(bytes);

                StreamContent scontent = new StreamContent(stream);

                var content = new MultipartFormDataContent();
                content.Add(scontent, "upfile", filePath);

                var result = await client.PostAsync(this.host, content);

                return result;
            }
            catch (HttpRequestException e)
            {
                //throw new PoseidonException($"Http Exception: {e.Message}");
                throw new Exception(e.Message);
            }
        }


        [TestMethod]
        public void TestMethod1()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\abc.txt";

            var task = Task.Run(() =>
            {
                var data = Post(filePath);

                return data;
            });

            var result = task.Result;

            Assert.AreEqual(200, result.StatusCode);
        }
    }
}