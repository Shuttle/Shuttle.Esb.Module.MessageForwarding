using System;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.Core.Logging;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Module.MessageForwarding
{
	public class MessageForwardingObserver : IPipelineObserver<OnAfterHandleMessage>
	{
	    private readonly IMessageForwardingConfiguration _configuration;

		private readonly ILog _log;

		public MessageForwardingObserver(IMessageForwardingConfiguration configuration)
		{
		    Guard.AgainstNull(configuration, nameof(configuration));

		    _configuration = configuration;

		    _log = Log.For(this);
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
					_configuration.MessageRoutes.FindAll(message.GetType().FullName)
						.Select(messageRoute => messageRoute.Uri.ToString())
						.ToList())
			{
				var recipientUri = uri;

				if (_log.IsTraceEnabled)
				{
					_log.Trace(string.Format(Resources.TraceForwarding, transportMessage.MessageType,
						transportMessage.MessageId, new Uri(recipientUri).Secured()));
				}

				handlerContext.Send(message, c => c.WithRecipient(recipientUri));
			}
		}
	}
}
