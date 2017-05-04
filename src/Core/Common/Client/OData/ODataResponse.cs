namespace Core.Common.Client.OData
{
    public class ODataResponse
    {
        public string Code { get; set; }
        public InnerError InnerError { get; set; }
        public string Message { get; set; }
    }
}