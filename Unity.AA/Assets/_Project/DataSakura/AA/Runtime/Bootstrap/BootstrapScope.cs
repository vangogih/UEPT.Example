using DataSakura.AA.Runtime.Utilities;
using VContainer;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LoadingService>(Lifetime.Scoped);
            
            builder.RegisterEntryPoint<OrientationHelper>().AsSelf();
            builder.RegisterEntryPoint<BootstrapFlow>();
        }
    }
}
