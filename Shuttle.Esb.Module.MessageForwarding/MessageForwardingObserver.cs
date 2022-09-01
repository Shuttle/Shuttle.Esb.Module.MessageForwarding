﻿using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingObserver : IPipelineObserver<OnAfterHandleMessage>
    {
        private readonly MessageRouteCollection _messageRoutes = new MessageRouteCollection();

        public MessageForwardingObserver(IOptions<MessageForwardingOptions> messageForwardingOptions)
        {
            Guard.AgainstNull(messageForwardingOptions, nameof(messageForwardingOptions));
            Guard.AgainstNull(messageForwardingOptions.Value, nameof(messageForwardingOptions.Value));

            var specificationFactory = new MessageRouteSpecificationFactory();

            foreach (var messageRouteOptions in messageForwardingOptions.Value.ForwardingRoutes)
            {
                var messageRoute = _messageRoutes.Find(messageRouteOptions.Uri);

                if (messageRoute == null)
                {
                    messageRoute = new MessageRoute(new Uri(messageRouteOptions.Uri));

                    _messageRoutes.Add(messageRoute);
                }

                foreach (var specification in messageRouteOptions.Specifications)
                {
                    messageRoute.AddSpecification(specificationFactory.Create(specification.Name, specification.Value));
                }
            }
        }

        public void Execute(OnAfterHandleMessage pipelineEvent)
        {
            var state = pipelineEvent.Pipeline.State;
            var message = state.GetMessage();
            var transportMessage = state.GetTransportMessage();
            var handlerContext = state.GetHandlerContext() as IHandlerContext;

            Guard.AgainstNull(message, nameof(message));
            Guard.AgainstNull(transportMessage, nameof(transportMessage));
            Guard.AgainstNull(handlerContext, nameof(handlerContext));

            foreach (
                var uri in
                _messageRoutes.FindAll(message.GetType().FullName)
                    .Select(messageRoute => messageRoute.Uri.ToString())
                    .ToList())
            {
                var recipientUri = uri;

                handlerContext?.Send(message, builder => builder.WithRecipient(recipientUri));
            }
        }
    }
}