using System.Configuration;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingSection : ConfigurationSection
    {
        [ConfigurationProperty("forwardingRoutes", IsRequired = true, DefaultValue = null)]
        public MessageRouteElementCollection ForwardingRoutes =>
            (MessageRouteElementCollection) this["forwardingRoutes"];
    }
}