using VContainer;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Bootstrap
{
    public sealed class BootstrapScope : LifetimeScope
    {
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BootstrapFlow>();
        }
    }
}
