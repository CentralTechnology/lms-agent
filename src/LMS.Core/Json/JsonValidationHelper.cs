namespace LMS.Core.Json
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class JsonValidationHelper
    {
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            var val = json.Trim();

            if (val.StartsWith("{") && val.EndsWith("}") || // object
                val.StartsWith("[") && val.EndsWith("]")) // array
            {
                try
                {
                    var obj = JToken.Parse(val);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}