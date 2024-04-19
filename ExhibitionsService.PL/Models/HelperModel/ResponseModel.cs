using System.Text.Json.Serialization;

namespace ExhibitionsService.PL.Models.HelperModel
{
    public class ResponseModel<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("successfully")]
        public bool Successfully { get; set; }

        [JsonPropertyName("code")]
        public int? Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        public static ResponseModel<T> CoverSuccessResponse(T data)
        {
            return new ResponseModel<T> {
                Successfully = true,
                Data = data
            };
        }

        public static ResponseModel<T> CoverUnsuccessResponse(int code, string message)
        {
            return new ResponseModel<T>
            {
                Successfully = false,
                Code = code,
                Message = message
            };
        }
    }
}
