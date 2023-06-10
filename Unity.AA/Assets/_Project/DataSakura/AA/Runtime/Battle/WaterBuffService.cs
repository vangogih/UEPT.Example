using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Utilities;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public class WaterBuffService : ILoadUnit, IFixedTickable
    {
        private readonly BattleController _battleController;
        private readonly WaterConfig _waterConfig;
        private List<UnderwaterPlane> _follow;

        private bool _isInitialized;

        public WaterBuffService(BattleController battleController, ConfigContainer configContainer)
        {
            _battleController = battleController;
            _waterConfig = configContainer.Battle.WaterConfig;
        }

        UniTask ILoadUnit.Load()
        {
            _follow = new List<UnderwaterPlane>(_battleController.Bots.Count + 1) { new(_battleController.Player) };

            for (var i = 0; i < _battleController.Bots.Count; i++)
                _follow.Add(new UnderwaterPlane(_battleController.Bots[i]));

            _isInitialized = true;
            
            return UniTask.CompletedTask;
        }

        void IFixedTickable.FixedTick()
        {
            if (!_isInitialized)
                return;

            for (var i = 0; i < _follow.Count; i++)
            {
                UnderwaterPlane plane = _follow[i];

                if (plane.View.OnDead.Value)
                    continue;

                if (plane.View.transform.position.y > _waterConfig.WaterLevel && plane.IsUnderwater) {
                    // plane is above water
                    plane.View.ResetMaxSpeed();
                    plane.IsUnderwater = false;
                    continue;
                }
                
                if (plane.View.transform.position.y < _waterConfig.WaterLevel && !plane.IsUnderwater) {
                    // plane is under water
                    plane.View.BuffMaxSpeed(_waterConfig.WaterSpeedModificator);
                    plane.IsUnderwater = true;
                }
            }
        }
        
        private sealed class UnderwaterPlane
        {
            public bool IsUnderwater;
            public readonly PlaneView View;

            public UnderwaterPlane(PlaneView view)
            {
                View = view;
            }
        }
    }
}