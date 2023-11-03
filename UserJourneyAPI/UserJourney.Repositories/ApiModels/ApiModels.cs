namespace UserJourney.Repositories.ApiModels
{
    using System.Net;

    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public List<ApiMessage> Messages { get; set; } = new List<ApiMessage>();
        public bool Success { get; set; }
        public T Content { get; set; }
    }

    public class ApiMessage
    {
        public static class MessageTypes
        {
            public const string INFORMATION = "Information";
            public const string VALIDATION_ERROR = "Validation";
            public const string EXCEPTION = "Exception";
        }

        public string MessageType { get; set; }
        public string Message { get; set; }
    }

    public static class ApiMessageExtensions
    {
        public static ApiResponse<T> HandleBadRequest<T>(this ApiModels.ApiResponse<T> apiResponse, string message)
        {
            apiResponse.Success = false;
            apiResponse.StatusCode = HttpStatusCode.BadRequest;
            apiResponse.Messages.Add(new ApiModels.ApiMessage()
            {
                MessageType = ApiModels.ApiMessage.MessageTypes.INFORMATION,
                Message = message,
            });

            return apiResponse;
        }

        public static ApiResponse<T> HandleException<T>(this ApiModels.ApiResponse<T> apiResponse, string exceptionMessage)
        {
            apiResponse.Success = false;
            apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            apiResponse.Messages.Add(new ApiModels.ApiMessage()
            {
                MessageType = ApiModels.ApiMessage.MessageTypes.EXCEPTION,
                Message = exceptionMessage,
            });

            return apiResponse;
        }

        public static ApiResponse<T> HandleResponse<T>(this ApiModels.ApiResponse<T> apiResponse, T responseContent)
        {
            var statusCode = (int)apiResponse.StatusCode;
            apiResponse.Success = statusCode <= 400; // if we aren't a 400s or 500s status code consider successful 
            apiResponse.StatusCode = apiResponse.StatusCode;
            apiResponse.Content = responseContent;
            return apiResponse;
        }
    }
}
