using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingModule
    {
        private readonly string _inboxMessagePipelineName = typeof(InboxMessagePipeline).FullName;
        private readonly MessageForwardingObserver _messageForwardingObserver;

        public MessageForwardingModule(IPipelineFactory pipelineFactory,
            MessageForwardingObserver messageForwardingObserver)
        {
            Guard.AgainstNull(pipelineFactory, "pipelineFactory");
            Guard.AgainstNull(messageForwardingObserver, "messageForwardingObserver");

            _messageForwardingObserver = messageForwardingObserver;

            pipelineFactory.PipelineCreated += PipelineCreated;
        }

        private void PipelineCreated(object sender, PipelineEventArgs e)
        {
            if (
                !e.Pipeline.GetType()
                    .FullName.Equals(_inboxMessagePipelineName, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            e.Pipeline.RegisterObserver(_messageForwardingObserver);
        }
    }
}