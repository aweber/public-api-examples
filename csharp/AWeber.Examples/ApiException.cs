using System;
using System.Net;
using AWeber.Examples.Models;

namespace AWeber.Examples
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public ApiError Error { get; }

        public ApiException(HttpStatusCode statusCode, ApiError error) : base(string.Format("API request failed. Error: {0}", error))
        {
            StatusCode = statusCode;
            Error = error;
        }
    }
}