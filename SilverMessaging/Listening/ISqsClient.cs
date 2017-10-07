using Amazon;
using Amazon.SQS;

namespace SilverPublish
{
    public interface ISqsClient : IAmazonSQS
    {
        RegionEndpoint Region { get; }
        string QueueName { get; }
        string Url { get; }
    }
}
