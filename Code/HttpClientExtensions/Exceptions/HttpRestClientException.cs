using System;
using System.Net;
using System.Net.Http;

namespace ClienteHttp.Exceptions
{
    public class HttpRestClientException : Exception
    {
        public string ReasonPhrase => HttpResponseMessage.ReasonPhrase;
        public HttpStatusCode StatusCode => HttpResponseMessage.StatusCode;
        public HttpResponseMessage HttpResponseMessage { get; }

        public HttpRestClientException(HttpResponseMessage httpResponseMessage) :
            base($"Error invoking URL: {httpResponseMessage.RequestMessage.RequestUri.AbsoluteUri}. {httpResponseMessage.ReasonPhrase}")
        {
            HttpResponseMessage = httpResponseMessage;
        }
    }
}
