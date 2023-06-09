using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Battle.Joystick;
using DataSakura.AA.Runtime.Utilities;
using UnityEngine;
using Resources = UnityEngine.Resources;

namespace DataSakura.AA.Runtime.Battle
{
    public class PlaneFactory : ILoadUnit
    {
        private readonly ConfigContainer _configs;
        private readonly JoystickInput _joystick;
        private readonly BotInput _botInput;
        private Dictionary<string, PlaneView> _prefabs;
        private PlaneView _playerPlane;

        public PlaneFactory(ConfigContainer configs, JoystickInput joystick, BotInput botInput)
        {
            _configs = configs;
            _joystick = joystick;
            _botInput = botInput;
        }

        UniTask ILoadUnit.Load()
        {
            string[] planesToLoad = RuntimeConstants.Planes.All;
            _prefabs = new Dictionary<string, PlaneView>(planesToLoad.Length);

            foreach (string planeToLoad in planesToLoad)
                _prefabs.Add(planeToLoad, Resources.Load<PlaneView>($"Planes/{planeToLoad}"));
            return UniTask.CompletedTask;
        }

        public PlaneView Create(SpawnParams @params)
        {
            PlaneView plane = Object.Instantiate(_prefabs[@params.PlaneName]);
            IInput input = @params.IsPlayer ? _joystick : _botInput;
            var cfg = @params.IsPlayer ? _configs.Battle.PlaneConfig : _configs.Battle.BotPlaneConfig;
            plane.Initialize(cfg, input, @params.IsPlayer);
            return plane;
        }
    }

    public readonly ref struct SpawnParams
    {
        public readonly string PlaneName;
        public readonly bool IsPlayer;

        private SpawnParams(string planeName, bool isPlayer)
        {
            PlaneName = planeName;
            IsPlayer = isPlayer;
        }

        public static SpawnParams Player(string planeName) => new(planeName, true);
        public static SpawnParams Bot(string planeName) => new(planeName, false);
    }
}