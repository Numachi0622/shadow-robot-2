using InGame.Message;
using MessagePipe;
using NaughtyAttributes;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InGame.System
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private DebugCommand _debugCommand;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<StateChangeMessage>(options);

            builder.RegisterEntryPoint<InGameCore>().AsSelf();
            builder.Register<CharacterFactory>(Lifetime.Singleton).AsSelf();

            builder.RegisterComponent(_debugCommand);
        }
    }
}