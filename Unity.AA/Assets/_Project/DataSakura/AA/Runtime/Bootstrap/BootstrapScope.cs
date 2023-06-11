using DataSakura.Runtime.Utilities;
using VContainer;
using VContainer.Unity;

namespace DataSakura.Runtime.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
        protected override void Awake()
        {
            IsRoot = true;
            DontDestroyOnLoad(this);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LoadingService>(Lifetime.Scoped);
            builder.Register<ConfigContainer>(Lifetime.Singleton);

            builder.RegisterEntryPoint<OrientationHelper>().AsSelf();
            builder.RegisterEntryPoint<BootstrapFlow>();
        }
    }
}