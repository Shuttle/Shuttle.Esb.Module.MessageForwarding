using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Module.MessageForwarding
{
	public static class ComponentRegistryExtensions
	{
		public static void RegisterMessageForwarding(this IComponentRegistry registry)
		{
			Guard.AgainstNull(registry, nameof(registry));

		    if (!registry.IsRegistered<IMessageForwardingConfiguration>())
		    {
		        registry.AttemptRegisterInstance(MessageForwardingSection.Configuration());
		    }

            registry.AttemptRegister<MessageForwardingModule>();
			registry.AttemptRegister<MessageForwardingObserver>();
		}
	}
}
