using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class OrderReceivedHandler :
    IHandleMessages<OrderReceived>
{
    static ILog log = LogManager.GetLogger<OrderReceivedHandler>();

    public Task Handle(OrderReceived message, IMessageHandlerContext context)
    {
        log.Info($"Subscriber has received OrderReceived event with OrderId {message.OrderId}.");
        return Task.CompletedTask;
    }
}

public class IOrderReceivedHandler :
    IHandleMessages<IOrderReceived>
{
    static ILog log = LogManager.GetLogger<IOrderReceivedHandler>();

    public Task Handle(IOrderReceived message, IMessageHandlerContext context)
    {
        log.Info($"Subscriber has received IOrderReceived event with OrderId {message.OrderId}.");
        return Task.CompletedTask;
    }
}