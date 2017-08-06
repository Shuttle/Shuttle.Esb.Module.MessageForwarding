using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Module.MessageForwarding
{
    public class Bootstrap :
        IComponentRegistryBootstrap,
        IComponentResolverBootstrap
    {
        private static bool _registered;
        private static bool _registryBootstrapCalled;
        private static bool _resolverBootstrapCalled;

        public void Register(IComponentRegistry registry)
        {
            Guard.AgainstNull(registry, "registry");

            if (_registryBootstrapCalled)
            {
                return;
            }

            _registryBootstrapCalled = true;

            if (!registry.IsRegistered<IMessageFailureConfiguration>())
            {
                var configuration = MessageForwardingSection.Configuration();

                if (configuration == null)
                {
                    return;
                }

                registry.AttemptRegister(configuration);
            }

            registry.AttemptRegister<MessageForwardingModule>();
            registry.AttemptRegister<MessageForwardingObserver>();

            _registered = true;
        }

        public void Resolve(IComponentResolver resolver)
        {
            Guard.AgainstNull(resolver, "resolver");

            if (_resolverBootstrapCalled || !_registered)
            {
                return;
            }

            resolver.Resolve<MessageForwardingModule>();

            _resolverBootstrapCalled = true;
        }
    }
}