using System;
using System.Configuration;
using Shuttle.Core.Configuration;

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
            var configuration = new MessageForwardingConfiguration();

	        if (section?.ForwardingRoutes == null)
	        {
	            return configuration;
	        }

	        var factory = new MessageRouteSpecificationFactory();

	        foreach (MessageRouteElement mapElement in section.ForwardingRoutes)
	        {
	            var map = configuration.MessageRoutes.Find(mapElement.Uri);

	            if (map == null)
	            {
	                map = new MessageRoute(new Uri(mapElement.Uri));

	                configuration.MessageRoutes.Add(map);
	            }

	            foreach (SpecificationElement specificationElement in mapElement)
	            {
	                map.AddSpecification(factory.Create(specificationElement.Name, specificationElement.Value));
	            }
	        }

	        return configuration;
	    }
    }
}
