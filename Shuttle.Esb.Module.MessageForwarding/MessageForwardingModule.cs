using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Pipelines;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingModule
    {
        private readonly string _inboxMessagePipelineName = typeof(InboxMessagePipeline).FullName;
        private readonly MessageForwardingObserver _messageForwardingObserver;

        public MessageForwardingModule(IPipelineFactory pipelineFactory,
            MessageForwardingObserver messageForwardingObserver)
        {
            Guard.AgainstNull(pipelineFactory, nameof(pipelineFactory));
            Guard.AgainstNull(messageForwardingObserver, nameof(messageForwardingObserver));

            _messageForwardingObserver = messageForwardingObserver;

            pipelineFactory.PipelineCreated += PipelineCreated;
        }

        private void PipelineCreated(object sender, PipelineEventArgs e)
        {
            if (!(e.Pipeline.GetType().FullName ?? string.Empty)
                .Equals(_inboxMessagePipelineName, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            e.Pipeline.RegisterObserver(_messageForwardingObserver);
        }
    }
}