namespace LMS.Common.Extensions
{
    using System;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class StringExtensions
    {
        public static bool IsValidJson(this string strInput)
        {
            strInput = strInput.Trim();
            if (strInput.StartsWith("{") && strInput.EndsWith("}") || //For object
                strInput.StartsWith("[") && strInput.EndsWith("]")) //For array
            {
                try
                {
                    JToken obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return false;
        }

        public static string SectionTitle(this string title)
        {
            int defaultWidth = 80;

            var sb = new StringBuilder();

            string dash = new string('-', defaultWidth);
            int titleWidth = defaultWidth - (title.Length + 6);

            string titleLine = new string(' ', titleWidth / 2);

            sb.Append(Environment.NewLine);
            sb.AppendLine(dash);
            sb.AppendLine($"{titleLine} {title} {titleLine}");
            sb.AppendLine(dash);

            return sb.ToString();
        }
    }
}