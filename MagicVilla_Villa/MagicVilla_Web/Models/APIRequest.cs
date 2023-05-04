using static MagicVilla_Utility.SD;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        //ApiType, we need to get the type of the request first
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
