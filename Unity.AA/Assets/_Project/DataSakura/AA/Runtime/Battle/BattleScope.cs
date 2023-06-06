using VContainer;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public sealed class BattleScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<BattleFlow>();
        }
    }
}