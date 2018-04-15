namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingConfiguration : IMessageForwardingConfiguration
    {
        public MessageForwardingConfiguration()
        {
            MessageRoutes = new MessageRouteCollection();
        }

        public IMessageRouteCollection MessageRoutes { get; }
    }
}