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
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_battleCanvasProvider);
            builder.RegisterComponent(_joystick);
            
            builder.RegisterEntryPoint<BattleFlow>();
        }
    }
}