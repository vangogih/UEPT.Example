using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DataSakura.Runtime.Battle
{
    public sealed class BattleDebug : MonoBehaviour
    {
        private PlaneFactory _factory;

        [Inject]
        private void Inject(PlaneFactory factory)
        {
            _factory = factory;
        }

        [Button]
        public void Spawn(string planeName)
        {
            var battleCtl = ResolveFromScope<BattleController, BattleScope>();
            _factory.CreateBot(planeName, battleCtl.Player);
        }
        
        private static TResolve ResolveFromScope<TResolve, TScope>() where TScope : LifetimeScope
        {
            TScope scope = Object.FindObjectOfType<TScope>();
            return scope.Container.Resolve<TResolve>();
        }
    }
}