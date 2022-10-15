using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Infrastructure.Services
{
    public class BitcoinCoreClient : IBitcoinCoreClient
    {
        private readonly IConfiguration _config;
        private ApiRequestDto apiRequestDto { get; set; }
        public BitcoinCoreClient(IConfiguration config)
        {
            _config = config;
            apiRequestDto = new ApiRequestDto();
        }

        public async Task<string> ServerRequest(string methodName, List<JToken> parameters)
        {
            try
            {
                string bitcoinUrl = _config["Bitcoin:URl"];
                string username = _config["Bitcoin:username"];
                string password = _config["Bitcoin:password"];

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(bitcoinUrl);
                webRequest.Credentials = new NetworkCredential(username, password);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json-rpc";

                string response = string.Empty;
                JObject jObject = new JObject();
                jObject.Add(new JProperty("jsonrpc", "1.0"));
                jObject.Add(new JProperty("id", "1"));
                jObject.Add(new JProperty("method", methodName));

                JArray props = new JArray();
                foreach (var item in parameters)
                {
                    props.Add(item);
                }
                jObject.Add(new JProperty("params", props));

                // Serialize JSON for request
                string s = JsonConvert.SerializeObject(jObject);
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                webRequest.ContentLength = bytes.Length;
                Stream stream = webRequest.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();

                // Deserialize the response
                StreamReader reader = null;
                WebResponse webResponse = webRequest.GetResponse();
                reader = new StreamReader(webRequest.GetRequestStream(), true);
                response = reader.ReadToEnd();

                var data = JsonConvert.DeserializeObject(response).ToString();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> ServerRequest(string methodName, List<string> parameters)
        {
            try
            {
                var response = await ServerRequest(methodName, parameters.Select(c => new JValue(c)).ToList<JToken>());
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RestRequest CreateRestClientRequest()
        {
            var client = new RestClient(_config["Bitcoin:URL"]);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Basic {_config["Bitcoin:authKey"]}");
            request.AddHeader("Content-Type", "text/plain");
            return request;

        }
    }
}
