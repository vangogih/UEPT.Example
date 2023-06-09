using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace DataSakura.AA.Runtime.Battle
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
            _factory.Create(SpawnParams.Bot(planeName));
        }
    }
}