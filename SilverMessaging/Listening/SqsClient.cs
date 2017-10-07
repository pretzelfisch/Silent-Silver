using Amazon;
using Amazon.SQS;

namespace SilverPublish
{
    public class SqsClient : AmazonSQSClient, ISqsClient
    {
        public SqsClient(string url, string regionName) : base(Amazon.RegionEndpoint.GetBySystemName(regionName))
        {
            Region = Amazon.RegionEndpoint.GetBySystemName(regionName);
            Url = url;
        }
        public RegionEndpoint Region { get; private set; }
        public string Url { get; private set; }
        public string QueueName { get; private set; }

    }
}