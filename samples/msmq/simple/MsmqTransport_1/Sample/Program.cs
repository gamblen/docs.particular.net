using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.Msmq.Simple";
        #region ConfigureMsmqEndpoint

        var endpointConfiguration = new EndpointConfiguration("Samples.Msmq.Simple");
        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString("");
        transport.Transactions(TransportTransactionMode.ReceiveOnly);
        transport.UseForwardingTopology();

        endpointConfiguration.LimitMessageProcessingConcurrencyTo(100);

        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();


        #endregion
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        //var metrics = endpointConfiguration.EnableMetrics();
        //metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(100));

        var recoverability = endpointConfiguration.Recoverability();
        recoverability.AddUnrecoverableException<InvalidOperationException>();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        for (var i = 0; i < 10000; i++)
        {
            _ = endpointInstance.SendLocal(new MyMessage())
                .ConfigureAwait(false);
        }
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}