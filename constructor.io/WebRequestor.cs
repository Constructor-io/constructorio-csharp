﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorIO
{
    internal class WebRequestor
    {
        private WebClient _downloader;

        private string _apiKey;
        private string _autocomlpeteKey;
        private string _lastBody;

        internal WebRequestor(string APIKey, string AutocompleteKey)
        {
            _downloader = new WebClient();
            _apiKey = APIKey;
            _autocomlpeteKey = AutocompleteKey;
        }

        internal async Task<Tuple<bool, string>> MakeRequest(ConstructorIORequest APIRequest)
        {
            return await MakeRequest(APIRequest, (response) =>
            {
                return ((int)response.StatusCode) == 204;
            });
        }

        internal async Task<Tuple<bool, string>> MakeRequest(ConstructorIORequest APIRequest, Func<HttpWebResponse, bool> ResponseCheck)
        {
            Uri requestURI = APIRequest.GetURI(_autocomlpeteKey);

            bool validResponse = false;
            bool errorRaised = false;
            string responseText = "";

            string jsonBody = null;
            HttpWebResponse serverResponse = null;

            try
            {
                if (APIRequest.RequestBody != null)
                {
                    JObject jobj = JObject.FromObject(APIRequest.RequestBody);
                    jsonBody = jobj.ToString()
                                    .Replace("\\\"", "\"")
                                    .Replace("\"[", "[")
                                    .Replace("]\"", "]");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error generating JSON body. See inner exception for details.", ex);
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestURI);

                string creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiKey + ":"));
                request.Headers[HttpRequestHeader.Authorization] = String.Format("Basic {0}", creds);
                request.Method = APIRequest.Method;

                if (request.Method != "GET")
                {
                    request.ContentType = "application/json";

                    using (Stream requestStream = await request.GetRequestStreamAsync())
                    {
                        using (StreamWriter writer = new StreamWriter(requestStream))
                        {
                            writer.Write(jsonBody);
                        }
                        await requestStream.FlushAsync();
                    }
                }

                serverResponse = (HttpWebResponse)await request.GetResponseAsync();
                validResponse = ResponseCheck(serverResponse);
            }
            catch(WebException webException)
            {
                errorRaised = true;

                if(webException.Response as HttpWebResponse != null)
                {
                    serverResponse = webException.Response as HttpWebResponse;
                }
                else
                {
                    throw new Exception("Error Downlaoding. See inner exception for details", webException);
                }
            }
            catch(Exception e)
            {
                throw new Exception("Error Downlaoding. See inner exception for details", e);
            }

            try
            {
                if (serverResponse != null)
                    using (Stream dataStream = serverResponse.GetResponseStream())
                        using (StreamReader reader = new StreamReader(dataStream))
                            _lastBody = responseText = await reader.ReadToEndAsync();

                if(errorRaised)
                {
                    //TODO: Check body for well-formed error message
                    throw new Exception("Error occured during request. Response:" + _lastBody);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error reading server response. See inner exception for details", ex);
            }

            return Tuple.Create(validResponse, responseText);
        }

        internal string GetLastBody()
        {
            return _lastBody;
        }
    }
}