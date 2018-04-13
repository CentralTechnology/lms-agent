using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Json
{
    using System.IO;
    using Newtonsoft.Json;
    using RestSharp.Deserializers;
    using RestSharp.Serializers;
    using JsonSerializer = Newtonsoft.Json.JsonSerializer;

    public class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonSerializer(JsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public static NewtonsoftJsonSerializer Default => new NewtonsoftJsonSerializer(new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            string content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string ContentType
        {
            get { return "application/json"; } // Probably used for Serialization?
            set { }
        }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    _serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }
    }
}
