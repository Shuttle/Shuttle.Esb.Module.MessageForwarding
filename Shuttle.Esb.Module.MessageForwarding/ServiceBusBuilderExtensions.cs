using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public static class ServiceBusBuilderExtensions
    {
        public static ServiceBusBuilder AddMessageForwardingModule(this ServiceBusBuilder serviceBusBuilder,
            Action<MessageForwardingBuilder> builder = null)
        {
            Guard.AgainstNull(serviceBusBuilder, nameof(serviceBusBuilder));

            var messageForwardingBuilder = new MessageForwardingBuilder(serviceBusBuilder.Services);

            builder?.Invoke(messageForwardingBuilder);

            serviceBusBuilder.Services.TryAddSingleton<MessageForwardingModule, MessageForwardingModule>();

            serviceBusBuilder.Services.AddOptions<MessageForwardingOptions>().Configure(options =>
            {
                options.ForwardingRoutes = messageForwardingBuilder.Options.ForwardingRoutes;
            });

            serviceBusBuilder.AddModule<MessageForwardingModule>();

            return serviceBusBuilder;
        }
    }
}