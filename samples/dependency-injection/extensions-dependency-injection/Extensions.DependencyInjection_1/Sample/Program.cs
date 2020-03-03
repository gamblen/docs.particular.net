using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

static class Program
{
    static async Task Main()
    {
        Console.Title = "Samples.NServiceBus.Extensions.DependencyInjection";

        #region ContainerConfiguration

        var endpointConfiguration = new EndpointConfiguration("Samples.NServiceBus.Extensions.DependencyInjection");

        var endpointHealthSettings = new EndpointHealthSettings("My Settings");

        var serviceCollection = new ServiceCollection();

        var containerBuilder = new ContainerBuilder();
        var rootLifetimeScope = containerBuilder.Build();

        #endregion

        endpointConfiguration.UseTransport<LearningTransport>();

        var startableEndpoint =
            EndpointWithExternallyManagedServiceProvider.Create(endpointConfiguration, serviceCollection);

        var endpointInstancescope = rootLifetimeScope.BeginLifetimeScope(builder =>
        {
            builder.Populate(serviceCollection);
            builder.RegisterModule<AutofacPropertyInjectionModule>();
            builder.RegisterInstance(endpointHealthSettings);
            builder.RegisterInstance(new MyService());
        });

        var endpoint = await startableEndpoint.Start(new AutofacServiceProvider(endpointInstancescope))
            .ConfigureAwait(false);

        var myMessage = new MyMessage();
        await endpoint.SendLocal(myMessage)
            .ConfigureAwait(false);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpoint.Stop()
            .ConfigureAwait(false);
    }

    class AutofacPropertyInjectionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            registration.Activating += (sender, args) => args.Context.InjectProperties(args.Instance);
        }
    }
}