using System;
using System.Linq;
using Shuttle.Core.Configuration;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Module.MessageForwarding
{
	public class MessageForwardingObserver : IPipelineObserver<OnAfterHandleMessage>
	{
	    private readonly IMessageRouteCollection _messageRoutes = new MessageRouteCollection();

		private readonly ILog _log;

		public MessageForwardingObserver()
		{
		    _log = Log.For(this);
		}

	    internal void Initialize(IServiceBus bus)
		{
			var section = ConfigurationSectionProvider.Open<MessageForwardingSection>("shuttle", "messageForwarding");

			if (section?.ForwardingRoutes == null)
			{
				return;
			}

			var factory = new MessageRouteSpecificationFactory();

			foreach (MessageRouteElement mapElement in section.ForwardingRoutes)
			{
				var map = _messageRoutes.Find(mapElement.Uri);

				if (map == null)
				{
					map = new MessageRoute(new Uri(mapElement.Uri));

					_messageRoutes.Add(map);
				}

				foreach (SpecificationElement specificationElement in mapElement)
				{
					map.AddSpecification(factory.Create(specificationElement.Name, specificationElement.Value));
				}
			}
		}

		public void Execute(OnAfterHandleMessage pipelineEvent)
		{
			var state = pipelineEvent.Pipeline.State;
			var message = state.GetMessage();
			var transportMessage = state.GetTransportMessage();
			var handlerContext = state.GetHandlerContext() as IMessageSender;

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

				if (_log.IsTraceEnabled)
				{
					_log.Trace(string.Format(MessageForwardingResources.TraceForwarding, transportMessage.MessageType,
						transportMessage.MessageId, new Uri(recipientUri).Secured()));
				}

				handlerContext.Send(message, c => c.WithRecipient(recipientUri));
			}
		}
	}
}
