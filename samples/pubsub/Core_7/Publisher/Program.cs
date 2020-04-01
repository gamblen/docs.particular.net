using System;
using System.Threading.Tasks;
using NServiceBus;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.PubSub.Publisher";
        var endpointConfiguration = new EndpointConfiguration("Samples.PubSub.Publisher");
        var transport = endpointConfiguration.UseTransport<SqsTransport>();

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        await Start(endpointInstance)
            .ConfigureAwait(false);
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    static async Task Start(IEndpointInstance endpointInstance)
    {
        Console.WriteLine("Press '1' to publish the OrderReceived event");
        Console.WriteLine("Press '2' to publish multiple the OrderReceived event");
        Console.WriteLine("Press '3' to publish the IOrderReceived event");
        Console.WriteLine("Press any other key to exit");

        #region PublishLoop

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            var orderReceivedId = Guid.NewGuid();
            if (key.Key == ConsoleKey.D1)
            {
                var orderReceived = new OrderReceived
                {
                    OrderId = orderReceivedId
                };
                await endpointInstance.Publish(orderReceived)
                    .ConfigureAwait(false);
                Console.WriteLine($"Published OrderReceived Event with Id {orderReceivedId}.");
            }
            else if (key.Key == ConsoleKey.D2)
            {
                for (var i = 0; i < 100; i++)
                {
                    var orderReceived = new OrderReceived
                    {
                        OrderId = orderReceivedId
                    };
                    _ = endpointInstance.Publish(orderReceived);
                }
            }
            else if (key.Key == ConsoleKey.D3)
            {
                var orderReceived = new OrderReceived
                {
                    OrderId = orderReceivedId
                };
                await endpointInstance.Publish<IOrderReceived>(m => { m.OrderId = orderReceivedId; })
                    .ConfigureAwait(false);
                Console.WriteLine($"Published OrderReceived Event with Id {orderReceivedId}.");
            }
            else
            {
                return;
            }
        }

        #endregion
    }
}