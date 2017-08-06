using System.Configuration;
using System.Linq;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class MessageForwardingSection : ConfigurationSection
    {
        [ConfigurationProperty("forwardingRoutes", IsRequired = true, DefaultValue = null)]
        public MessageRouteElementCollection ForwardingRoutes =>
            (MessageRouteElementCollection) this["forwardingRoutes"];

        public static IMessageForwardingConfiguration Configuration()
        {
            var section = ConfigurationSectionProvider.Open<MessageForwardingSection>("shuttle", "messageForwarding");

            if (section?.ForwardingRoutes == null)
            {
                return null;
            }

            var configuration = new MessageForwardingConfiguration();

            foreach (MessageRouteElement mapElement in section.ForwardingRoutes)
            {
                var messageRoute = new MessageRouteConfiguration(mapElement.Uri);

                foreach (SpecificationElement specificationElement in mapElement)
                {
                    messageRoute.AddSpecification(specificationElement.Name, specificationElement.Value);
                }

                if (messageRoute.Specifications.Any())
                {
                    configuration.AddMessageRoute(messageRoute);
                }
            }

            return configuration;
        }
    }
}