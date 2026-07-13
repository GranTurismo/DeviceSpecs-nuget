using System;
using System.Net;

namespace DeviceSpecs.Exceptions
{
    /// <summary>
    /// Exception thrown when the DeviceSpecs API returns an error response.
    /// </summary>
    public class DeviceSpecsApiException : Exception
    {
        /// <summary>
        /// Gets the HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the raw string content returned by the API.
        /// </summary>
        public string ResponseContent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSpecsApiException"/> class.
        /// </summary>
        public DeviceSpecsApiException(HttpStatusCode statusCode, string responseContent, string message)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSpecsApiException"/> class with an inner exception.
        /// </summary>
        public DeviceSpecsApiException(HttpStatusCode statusCode, string responseContent, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}
