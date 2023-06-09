using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Utilities;
using UnityEngine;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle.Joystick
{
    public class BotInput : ILoadUnit<IReadOnlyList<PlaneView>>, IInput, IFixedTickable
    {
        public Vector2 Direction { get; }
        public bool IsPressed { get; }

        private IReadOnlyList<PlaneView> _bots;

        public UniTask Load(IReadOnlyList<PlaneView> param)
        {
            _bots = param;
            return UniTask.CompletedTask;
        }

        public void FixedTick()
        {
            if (_bots.Count == 0)
                return;
        }
    }
}