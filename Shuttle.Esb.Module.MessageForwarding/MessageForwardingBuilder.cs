using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingBuilder
    {
        private MessageForwardingOptions _activeTimeRangeOptions = new MessageForwardingOptions();
        public IServiceCollection Services { get; }

        public MessageForwardingBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }

        public MessageForwardingOptions Options
        {
            get => _activeTimeRangeOptions;
            set => _activeTimeRangeOptions = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}