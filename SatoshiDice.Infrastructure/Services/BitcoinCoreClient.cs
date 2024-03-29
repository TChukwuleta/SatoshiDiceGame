﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections;
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

        public async Task<string> BitcoinRequestServer(string methodName, List<JToken> parameters)
        {
            string ServerIp = _config["Bitcoin:URl"];
            string UserName = _config["Bitcoin:username"];
            string Password = _config["Bitcoin:password"];

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(ServerIp);
            webRequest.Credentials = new NetworkCredential(UserName, Password);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            string responseValue = string.Empty;
            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "curltest"));
            joe.Add(new JProperty("method", methodName));
            JArray props = new JArray();
            foreach (var parameter in parameters)
            {
                props.Add(parameter);
            }
            joe.Add(new JProperty("params", props));
            // serialize JSON for request
            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            // deserialze the response
            StreamReader sReader = null;
            WebResponse webResponse = webRequest.GetResponse();
            sReader = new StreamReader(webResponse.GetResponseStream(), true);
            responseValue = sReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject(responseValue).ToString();
            return data;
        }

        public async Task<string> BitcoinRequestServer(string methodName, List<string> Parameters)
        {
            /*string serverIp = _config["Bitcoin:URl"];
            string username = _config["Bitcoin:username"];
            string Password = _config["Bitcoin:password"];
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serverIp);
            webRequest.Credentials = new NetworkCredential(username, Password);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            string responseValue = string.Empty;
            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "curltext"));
            joe.Add(new JProperty("method", methodName));
            JArray props = new JArray();
            foreach (var parameter in Parameters)
            {
                props.Add(parameter);
            }
            joe.Add(new JProperty("params", props));
            // Serialize json for request
            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            // Deserialize the response
            StreamReader streamReader = null;
            WebResponse webResponse = webRequest.GetResponse();
            streamReader = new StreamReader(webResponse.GetResponseStream(), true);
            responseValue = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject(responseValue).ToString();
            return data;*/
            return await BitcoinRequestServer(methodName, Parameters.Select(c => new JValue(c)).ToList<JToken>());
        }

        public async Task<string> BitcoinRequestServer(string methodName)
        {
            string serverIp = _config["Bitcoin:URl"];
            string username = _config["Bitcoin:username"];
            string Password = _config["Bitcoin:password"];
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serverIp);
            webRequest.Credentials = new NetworkCredential(username, Password);
            JArray props = new JArray();
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            string responseValue = string.Empty;
            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "curltext"));
            joe.Add(new JProperty("method", methodName));
            joe.Add(new JProperty("params", props));
            string s = JsonConvert.SerializeObject(joe);
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = bytes.Length;
            Stream stream = webRequest.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            // Deserialize the response
            StreamReader streamReader = null;
            WebResponse webResponse = webRequest.GetResponse();
            streamReader = new StreamReader(webResponse.GetResponseStream(), true);
            responseValue = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject(responseValue).ToString();
            return data;
        }

        public async Task<string> BitcoinRequestServer(string methodName, string parameter)
        {
            string serverIp = _config["Bitcoin:URl"];
            string username = _config["Bitcoin:username"];
            string Password = _config["Bitcoin:password"];
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serverIp);
            webRequest.Credentials = new NetworkCredential(username, Password);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";
            string responseValue = string.Empty;
            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "curltext"));
            joe.Add(new JProperty("method", methodName));
            JArray props = new JArray();
            props.Add(parameter);
            joe.Add(new JProperty("params", props));
            // Serialize json for request
            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            // Deserialize the response
            StreamReader streamReader = null;
            WebResponse webResponse = webRequest.GetResponse();
            streamReader = new StreamReader(webResponse.GetResponseStream(), true);
            responseValue = streamReader.ReadToEnd();
            var data = JsonConvert.DeserializeObject(responseValue).ToString();
            return data;
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

        public string GetRawTransaction(string txid)
        {
            string bitcoinUrl = _config["Bitcoin:URl"];
            string username = _config["Bitcoin:username"];
            string password = _config["Bitcoin:password"];
            string jsonReq = default;
            try
            {
                var credentialCache = new CredentialCache();
                credentialCache.Add(new Uri(bitcoinUrl), "Basic", new NetworkCredential(username, password));
                var httpWebRequest =(HttpWebRequest)WebRequest.Create(bitcoinUrl);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Credentials = credentialCache;
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    jsonReq = "{ \"jsonrpc\": \"2.0\", \"id\":\"" + Guid.NewGuid().ToString() + "\", \"method\": \"getrawtransaction\",\"params\":[\"" + txid + "\",1]}";
                    streamWriter.Write(jsonReq);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    return responseText;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
