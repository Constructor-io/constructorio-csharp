using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstructorIO
{
    using HashArgs = Dictionary<string, object>;

    internal class ConstructorIORequest
    {
        private string _apiPath;
        private string _method;
        private HashArgs _extraArgs;
        private HashArgs _requestBody;
        private string _uriScheme;

        public ConstructorIORequest(APIRequestType APIRequest, string Method,
            string Protocol = "https")
            : this(StringEnum.GetStringValue(APIRequest), Method, Protocol)
        {
        }

        public ConstructorIORequest(string APIPath, string Method,
                string Protocol = "https")
        {
            _apiPath = APIPath;
            _method = Method;
            _extraArgs = new HashArgs();
            _requestBody = new HashArgs();
            _uriScheme = Protocol;
        }

        public void AddParams(HashArgs ExtraParams)
        {
            foreach(var param in ExtraParams)
            {
                _extraArgs.Add(param.Key, param.Value);
            }
        }

        public void AddParam(string Key, object Value)
        {
            _extraArgs.Add(Key, Value);
        }

        internal Uri GetURI(string APIKey, string Host)
        {
            var tempParams = new HashArgs(_extraArgs); //Create a new instance
            //Don't want to have the API key floating around in each object
            tempParams["autocomplete_key"] = APIKey;
            
            var uriBuilder = new UriBuilder(_uriScheme, Host)
            {
                Path = _apiPath,
                Query = Util.SerializeParams(tempParams)
            };

            return uriBuilder.Uri;
        }

        public object this[string key]
        {
            get
            {
                return _extraArgs[key];
            }
            set
            {
                _extraArgs[key] = value;
            }
        }

        public HashArgs RequestBody
        {
            get { return _requestBody; }
        }

        public string Method { get { return _method; } }
    }

    internal enum APIRequestType
    {
        [StringValue("v1/verify")]
        V1_Verify,
        [StringValue("v1/item")]
        V1_Item,
        [StringValue("v1/click_through")]
        V1_ClickThrough,
        [StringValue("v1/conversion")]
        V1_Conversion,
        [StringValue("v1/search")]
        V1_Search,
        [StringValue("v1/batch_items")]
        V1_BatchItems
    };
}