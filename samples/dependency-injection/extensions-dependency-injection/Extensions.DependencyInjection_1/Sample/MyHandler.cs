using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

#region InjectingDependency
public class MyHandler :
    IHandleMessages<MyMessage>
{
    MyService myService;

    public MyHandler(MyService myService)
    {
        this.myService = myService;
    }

    public EndpointHealthSettings HealthSettings { get; set; }

    public Task Handle(MyMessage message, IMessageHandlerContext context)
    {
        log.Info(HealthSettings.Settings);
        myService.WriteHello();
        return Task.CompletedTask;
    }

    static ILog log = LogManager.GetLogger<MyHandler>();
}
#endregion
