using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SilverMessageLibrary;
using System.Configuration;
using System.Collections.Generic;

namespace ChattySilver
{


    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new SilverPublish.Publisher(ConfigurationManager.AppSettings["AWS_SQS_Region"], ConfigurationManager.AppSettings["AWS_SQS_URL"]);
            BatchCreateItems(publisher);
        }
        static void ManualCreateItems(SilverPublish.Publisher publisher) {
            do
            {
                Console.WriteLine("please provide an order id: ");
                int orderNumber = int.Parse(Console.ReadLine());
                var order = new OrderAccepted(orderNumber);
                publisher.Publish(order);
            } while (true);
        }
        static void BatchCreateItems(SilverPublish.Publisher publisher)
        {
            do
            {
                Console.WriteLine("how many orders to create? ");
                int numberOfOrders = int.Parse(Console.ReadLine());

                var orders = new List<OrderAccepted>();
                for (int i = 1; i <= numberOfOrders; i++) {
                    orders.Add( new OrderAccepted(i));
                }
                publisher.PublishBatch(orders);
            } while (true);
        }
    }


}
