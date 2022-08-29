using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMessageForwardingModule(this IServiceCollection services,
            Action<MessageForwardingBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var messageForwardingBuilder = new MessageForwardingBuilder(services);

            builder?.Invoke(messageForwardingBuilder);

            services.TryAddSingleton<MessageForwardingModule, MessageForwardingModule>();
            services.TryAddSingleton<MessageForwardingObserver, MessageForwardingObserver>();

            services.AddOptions<MessageForwardingOptions>().Configure(options =>
            {
                options.ForwardingRoutes = messageForwardingBuilder.Options.ForwardingRoutes;
            });

            services.AddPipelineModule<MessageForwardingModule>();

            return services;
        }
    }
}