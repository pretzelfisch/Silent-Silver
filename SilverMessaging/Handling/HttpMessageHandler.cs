using SilverPublish.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SilverPublish.Handling
{
    public class HttpMessageHandler : IMessageHandler
    {
        // TODO what about auth? maybe take a HttpClient?  Maybe force windows auth?
        string _baseAddress;
        private readonly ILogging _logger;
        private readonly HttpClient client = null;
        public HttpMessageHandler(string baseAddress, ILogging logger, HttpClient networkClient)
        {
            this._logger = logger;
            this.client = networkClient;
            _baseAddress = baseAddress;
        }
        public HttpMessageHandler(ILogging logger, HttpClient networkClient) : this("http://localhost/endpoint", logger, networkClient)
        {

        }

        protected virtual Uri BuildRoute(string baseAddress, string subject)
        {
            var address = _baseAddress.TrimEnd('/') + "/" + subject.Replace('.', '/');
            return new Uri( address);
        }
        protected virtual HttpContent BuildContent(string body)
        {
            var content = new StringContent(body, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
        public async Task<bool> Handle(string subject, string body)
        {
            
            try
            {
                // Post
                var route = BuildRoute(_baseAddress, subject);
                _logger.Info(new LogMessage(nameof(Handle), $"Sending message to {route}", body), null);
                var response = await client.PostAsync(route, BuildContent(body));
                if (response.IsSuccessStatusCode)
                {
                    _logger.Debug(new LogMessage(nameof(Handle), $"success handling message {route}", response), null);
                    return true;
                }
                else
                {
                    _logger.Warn(new LogMessage(nameof(Handle), $"failed handling message {route}", response), null);
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.Error(new LogMessage(nameof(Handle), $"Timed out handling message {subject}", body), ex);
            }
            catch (System.Exception ex)
            {
                _logger.Error(new LogMessage(nameof(Handle), $"Error handling message {subject}", body), ex);
            }

            return false;

        }
    }
}
