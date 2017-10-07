using SilverPublish;
using SilverPublish.Handling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;

namespace SilentSilver
{
    public class PreloadClient : System.Web.Hosting.IProcessHostPreloadClient
    {
        private static SimpleLogger logger = new SimpleLogger();
        public static ConcurrentQueue<SimpleLogger.LogDetails> Log { get => logger.LoggedItems; }

        public void Preload(string[] parameters)
        {
            
            if (SilentSilver.WebApiApplication.PreLoadCalledCount <= 0)
            {
                var baseUrl = "http://localhost/api";
                if (parameters != null && parameters.Length > 0)
                    baseUrl = parameters[0];



                var region = ConfigurationManager.AppSettings["AWS_SQS_Region"];
                var url = ConfigurationManager.AppSettings["AWS_SQS_URL"];
                System.Net.Http.HttpClient networkClient = CreateHttpClient();

                if (int.TryParse(ConfigurationManager.AppSettings["MaxMessageInFlightCount"], out int messageAllowedInFlight) == false)
                    messageAllowedInFlight = 25;
                                                


                if (ConfigurationManager.AppSettings["EnableQueue"] == "True")
                {
                    for (int i = int.Parse(ConfigurationManager.AppSettings["QueueListenerCount"]); i > 0; i--)
                    {
                        var queueListener = new SqsListener(region, url,
                            new HttpMessageHandler(baseUrl, PreloadClient.logger, networkClient),
                            PreloadClient.logger,
                            allowedInFlightMessages:  messageAllowedInFlight);

                        System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(ct => queueListener.Start(ct));
                    }
                }
            }
            SilentSilver.WebApiApplication.PreLoadCalledCount++;
        }

        private static System.Net.Http.HttpClient CreateHttpClient()
        {
            var networkClient = new System.Net.Http.HttpClient();
            if (int.TryParse(ConfigurationManager.AppSettings["HttpClientTimeoutInMinutes"], out int httpClientTimeoutInMinutes))
                networkClient.Timeout = TimeSpan.FromMinutes(httpClientTimeoutInMinutes);
            else
                networkClient.Timeout = TimeSpan.FromMinutes(2);


            networkClient.DefaultRequestHeaders.Accept.Clear();
            networkClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return networkClient;
        }
    };
    public class SimpleLogger : SilverPublish.Logging.ILogging
    {

        public class LogDetails
        {
            public LogDetails(int threadId, string Type, object message, Exception t)
            {
                this.ThreadId = threadId;
                this.Type = Type;
                Message = (SilverPublish.Logging.LogMessage)message;
                Exception = t;
                EventTime = DateTime.Now.ToLongTimeString();
            }

            public Exception Exception { get; set; }
            public SilverPublish.Logging.LogMessage Message { get; set; }
            public string EventTime { get; set; }
            public string Type { get; set; }
            public int ThreadId { get; set; }
        }

        private ConcurrentQueue<LogDetails> loggedItems = new ConcurrentQueue<LogDetails>();

        public ConcurrentQueue<LogDetails> LoggedItems { get => loggedItems; }

        public SimpleLogger()
        {
        }
        private void CoreLog(object message, Exception t, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            loggedItems.Enqueue(new LogDetails(System.Threading.Thread.CurrentThread.ManagedThreadId, callerName, message, t));
        }
        public void Debug(object message, Exception t)
        {
            CoreLog(message, t);
        }

        public void Error(object message, Exception t)
        {
            CoreLog(message, t);
        }

        public void Fatal(object message, Exception t)
        {
            CoreLog(message, t);
        }

        public void Info(object message, Exception t)
        {
            // CoreLog(message, t);
        }

        public void Warn(object message, Exception t)
        {
            CoreLog(message, t);
        }
    }
}