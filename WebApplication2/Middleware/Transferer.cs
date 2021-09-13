using Newtonsoft.Json;

using NLog;

using System.IO;
using System.Net;

namespace WebApplication2.Middleware
{
    public static class Transferer<T> where T : new()
    {
        public static string TransfererPost(T item, string url)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var client = new WebClient();
            string json = JsonConvert.SerializeObject(item);
            try
            {
                var responce = client.UploadString(url, json);
                return responce;
            }
            catch (WebException ex)
            {
                string responseFromServer = ex.Message.ToString() + " ";
                if (ex.Response != null)
                {
                    using (WebResponse response = ex.Response)
                    {
                        Stream dataRs = response.GetResponseStream();
                        using (StreamReader reader = new StreamReader(dataRs))
                        {
                            responseFromServer += reader.ReadToEnd();
                            logger.Error("Server Response: " + responseFromServer);
                        }
                    }
                }
                return "-1";
            }
        }
        public static T TransferGet(string url)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var client = new WebClient();
            try
            {
                //var client = new HttpClient
                //{
                //    BaseAddress = new Uri(BaseURL)
                //};
                client.Headers.Add("User-Agent", "Mobile App");
                client.Headers.Add("Content-type", "application/json");
                client.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                client.Encoding = System.Text.Encoding.UTF8;
                var responce = client.DownloadString(url);

                return JsonConvert.DeserializeObject<T>(responce);
            }
            catch (WebException ex)
            {
                string responseFromServer = ex.Message.ToString() + " ";
                if (ex.Response != null)
                {
                    using (WebResponse response = ex.Response)
                    {
                        Stream dataRs = response.GetResponseStream();
                        using (StreamReader reader = new StreamReader(dataRs))
                        {
                            responseFromServer += reader.ReadToEnd();
                            logger.Error("Server Response: " + responseFromServer);
                        }
                    }
                }
                return new T();
            }
        }
    }
}