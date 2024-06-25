using RestSharp;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 基于RestSharp帮助类
    /// </summary>
    public class RequestHelper
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        public static async Task<string?> GetAsync(string url)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(String.Empty, Method.Get);
                var response = await client.ExecuteAsync(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">头部键值对</param>
        public static async Task<(int statusCode, string? reHeaders, string? reBody)> GetAsync(string url, ICollection<KeyValuePair<string, string>> headers)
        {
            var client = new RestClient(url);
            var request = new RestRequest(String.Empty, Method.Get);
            request.AddHeaders(headers);
            var response = await client.ExecuteAsync(request);
            var statusCode = (int)response.StatusCode;
            var reHeaders = response.Headers?.ToJson();
            var reBody = response.Content;
            return (statusCode, reHeaders, reBody);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="content">POST内容</param>
        /// <param name="contentType">application/x-www-form-urlencoded</param>
        public static async Task<string?> PostAsync(string url, string content, string contentType= "application/json")
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(String.Empty, Method.Post);
                request.AddParameter(contentType, content, ParameterType.RequestBody);
                var response = await client.ExecuteAsync(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">头部键值对</param>
        /// <param name="content">POST内容</param>
        /// <param name="contentType">默认JSON</param>
        public static async Task<(int statusCode, string? reHeaders, string? reBody)> PostAsync(string url,
            ICollection<KeyValuePair<string, string>> headers, string content, string contentType = "application/json")
        {
            var client = new RestClient(url);
            var request = new RestRequest(String.Empty, Method.Post);
            request.AddHeaders(headers);
            request.AddParameter(contentType, content, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            var statusCode = (int)response.StatusCode;
            var reHeaders = response.Headers?.ToJson();
            var reBody = response.Content;
            return (statusCode, reHeaders, reBody);
        }

    }
}
