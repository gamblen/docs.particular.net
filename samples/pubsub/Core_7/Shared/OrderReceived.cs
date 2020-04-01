using System;
using NServiceBus;

public class OrderReceived :
    IOrderReceived
{
    public Guid OrderId { get; set; }
}

public interface IMyEvent : IEvent
{
}

public interface IOrderReceived : IMyEvent
{
    Guid OrderId { get; set; }
}