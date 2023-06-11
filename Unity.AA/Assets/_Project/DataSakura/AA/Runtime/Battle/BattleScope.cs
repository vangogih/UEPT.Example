using DataSakura.Runtime.Battle.Input;
using DataSakura.Runtime.Battle.Shooting;
using DataSakura.Runtime.Battle.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DataSakura.Runtime.Battle
{
    public sealed class BattleScope : LifetimeScope
    {
        [SerializeField] private BattleCanvasProvider _battleCanvasProvider;
        [SerializeField] private JoystickInput _joystick;
        [SerializeField] private BattleHudController _battleHudController;
        [SerializeField] private BattleDebug _debug;
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_battleCanvasProvider).AsSelf();
            builder.RegisterComponent(_joystick).AsSelf();
            builder.RegisterComponent(_battleHudController).AsSelf();
            builder.RegisterComponent(_debug).AsSelf();

            builder.Register<ShootingService>(Lifetime.Singleton);
            builder.Register<PlaneFactory>(Lifetime.Singleton);
            builder.Register<BattleController>(Lifetime.Singleton);

            builder.RegisterEntryPoint<WaterBuffService>().AsSelf();
            builder.RegisterEntryPoint<BattleFlow>();
        }
    }
}