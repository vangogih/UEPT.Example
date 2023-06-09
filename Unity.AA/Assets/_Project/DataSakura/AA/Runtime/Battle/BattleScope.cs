using DataSakura.AA.Runtime.Battle.Joystick;
using Silverfox.Runtime.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public sealed class BattleScope : LifetimeScope
    {
        [SerializeField] private BattleCanvasProvider _battleCanvasProvider;
        [SerializeField] private JoystickInput _joystick;
        [SerializeField] private BattleDebug _debug;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_battleCanvasProvider).AsSelf();
            builder.RegisterComponent(_joystick).AsSelf();
            builder.RegisterComponent(_debug).AsSelf();

            builder.Register<PlaneFactory>(Lifetime.Singleton);
            
            builder.RegisterEntryPoint<BattleFlow>();
        }
    }
}