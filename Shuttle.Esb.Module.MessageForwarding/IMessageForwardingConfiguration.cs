namespace Shuttle.Esb.Module.MessageForwarding
{
    public interface IMessageForwardingConfiguration
    {
        IMessageRouteCollection MessageRoutes { get; }
    }
}