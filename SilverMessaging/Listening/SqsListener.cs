using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using SilverPublish.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SilverPublish
{
    public class SqsListener
    {
        private readonly string STR_MessageType;
        private readonly List<Task> currentInFlightMessages = new List<Task>();
        private readonly ISqsClient _queue;
        private readonly IMessageHandler handler;
        private readonly int maxNumberOfMessages = 10;
        private readonly ILogging _logger;
        private readonly int maxInFlightMessages;

        public SqsListener(string region, string url, IMessageHandler messageProcessor, ILogging logger, string subjectAttributeName = "MessageType", int allowedInFlightMessages = 25) :
            this(new SqsClient(url, region), messageProcessor, logger, subjectAttributeName, allowedInFlightMessages)
        {

        }
        public SqsListener(ISqsClient queue, IMessageHandler messageProcessor, ILogging logger, string subjectAttributeName, int allowedInFlightMessages)
        {
            if (allowedInFlightMessages < 10)
                throw new System.ArgumentOutOfRangeException("allowedInFlightMessages", "must be a number greater than 10");

            this.maxInFlightMessages = allowedInFlightMessages;
            STR_MessageType = subjectAttributeName;
            this._logger = logger;
            this._queue = queue;
            this.handler = messageProcessor;
        }


        public void RemoveCompletedTasks()
        {
            currentInFlightMessages.RemoveAll(x => x.IsCanceled || x.IsCompleted || x.IsFaulted);
        }
        public async Task<CancellationToken> Start(CancellationToken ct)
        {
            int recivedMessageCount = 0;
            _logger.Info(new LogMessage(nameof(Start), "Start message queue listening", null), null);
            do
            {
                if (currentInFlightMessages.Count > 0 && currentInFlightMessages.Count >= maxInFlightMessages)
                {
                    _logger.Debug(new LogMessage(nameof(Start), $"flight deck full wait for tasks to completed {currentInFlightMessages.Count} at {DateTime.Now.ToString()}", null), null);
                    await Task.WhenAny(currentInFlightMessages);
                }


                _logger.Debug(new LogMessage(nameof(Start), $"Removing completed messages from flight list {currentInFlightMessages.Count} at {DateTime.Now.ToString()}", null), null);
                RemoveCompletedTasks();


                _logger.Debug(new LogMessage(nameof(Start), $"Current in flight count {currentInFlightMessages.Count} at {DateTime.Now.ToString()}", null), null);
                if (currentInFlightMessages.Count < maxInFlightMessages)
                    recivedMessageCount = await PollForMessages(ct);

                
                if (!(recivedMessageCount == maxNumberOfMessages && currentInFlightMessages.Count < maxInFlightMessages))
                    await Task.Yield();
                /*
                if (recivedMessageCount == 0 )
                    await Task.Delay(new TimeSpan(0, 0, 30), ct);
                */

            } while (ct.IsCancellationRequested == false);
            _logger.Info(new LogMessage(nameof(Start), "Message queue listening was canceled.", null), null);
            return ct;
        }

        private async Task<int> PollForMessages(CancellationToken ct)
        {
            var request = new ReceiveMessageRequest()
            {
                QueueUrl = _queue.Url,
                MaxNumberOfMessages = maxNumberOfMessages,
                WaitTimeSeconds = 20,
                // AttributeNames = new System.Collections.Generic.List<string>() { "All" },
                MessageAttributeNames = new System.Collections.Generic.List<string>() { "All" }
            };
            try
            {
                _logger.Info(new LogMessage(nameof(PollForMessages), "Requesting messages from queue", request), null);
                var response = await _queue.ReceiveMessageAsync(request, ct);
                _logger.Info(new LogMessage(nameof(PollForMessages), $"Received {response.Messages.Count} messages from queue", response), null);

                foreach (var message in response.Messages)
                {
                    currentInFlightMessages.Add(ProcessMessage(message));
                }
                return response.Messages.Count;
            }
            catch (System.Exception ex)
            {
                _logger.Error(new LogMessage(nameof(PollForMessages), $"Error requesting messages from queue", request), ex);
            }
            return 0;
        }

        protected virtual async Task ProcessMessage(Message message)
        {
            if (message.MessageAttributes.ContainsKey(STR_MessageType))
            {
                string messageType = message.MessageAttributes[STR_MessageType].StringValue;
                _logger.Info(new LogMessage(nameof(ProcessMessage), $"Start processing message from queue", message), null);
                try
                {
                    if (await handler.Handle(messageType, message.Body))
                    {
                        _logger.Info(new LogMessage(nameof(ProcessMessage), $"Successfully processed message", message), null);
                        await _queue.DeleteMessageAsync(new DeleteMessageRequest { QueueUrl = _queue.Url, ReceiptHandle = message.ReceiptHandle });
                    }
                    else
                    {
                        _logger.Info(new LogMessage(nameof(ProcessMessage), $"Failed to process message", message), null);
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.Error(new LogMessage(nameof(ProcessMessage), $"Error processing messages", message), ex);
                }

            }
        }
    }
}