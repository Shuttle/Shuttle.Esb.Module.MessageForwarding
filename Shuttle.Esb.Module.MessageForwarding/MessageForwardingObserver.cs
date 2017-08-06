using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingObserver : IPipelineObserver<OnAfterHandleMessage>
    {
        private readonly IMessageRouteProvider _messageRouteProvider = new DefaultMessageRouteProvider();

        private readonly ILog _log;

        public MessageForwardingObserver(IMessageForwardingConfiguration configuration)
        {
            Guard.AgainstNull(configuration, nameof(configuration));

            var specificationFactory = new MessageRouteSpecificationFactory();

            foreach (var messageRouteConfiguration in configuration.MessageRoutes)
            {
                var messageRoute = _messageRouteProvider.Find(messageRouteConfiguration.Uri);

                if (messageRoute == null)
                {
                    messageRoute = new MessageRoute(new Uri(messageRouteConfiguration.Uri));

                    _messageRouteProvider.Add(messageRoute);
                }

                foreach (var specification in messageRouteConfiguration.Specifications)
                {
                    messageRoute.AddSpecification(specificationFactory.Create(specification.Name, specification.Value));
                }
            }

            _log = Log.For(this);
        }

        public void Execute(OnAfterHandleMessage pipelineEvent)
        {
            var state = pipelineEvent.Pipeline.State;
            var message = state.GetMessage();
            var transportMessage = state.GetTransportMessage();
            var messageSender = state.GetHandlerContext() as IMessageSender;

            Guard.AgainstNull(message, "message");
            Guard.AgainstNull(transportMessage, "transportMessage");
            Guard.AgainstNull(messageSender, "handlerContext as IMessageSender");

            foreach (var uri in _messageRouteProvider.GetRouteUris(message.GetType().FullName))
            {
                var recipientUri = uri;

                if (_log.IsTraceEnabled)
                {
                    _log.Trace(string.Format(MessageForwardingResources.TraceForwarding, transportMessage.MessageType,
                        transportMessage.MessageId, new Uri(recipientUri).Secured()));
                }

                // ReSharper disable once PossibleNullReferenceException
                messageSender.Send(message, c => c.WithRecipient(recipientUri));
            }
        }
    }
}