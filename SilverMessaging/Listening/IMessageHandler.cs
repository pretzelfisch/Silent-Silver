using System.Threading.Tasks;

namespace SilverPublish
{
    public interface IMessageHandler

    {
          Task<bool> Handle(string messageType, string messageBody);
    }
}