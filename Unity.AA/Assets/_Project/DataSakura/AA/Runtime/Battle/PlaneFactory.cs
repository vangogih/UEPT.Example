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
        private Dictionary<string, PlaneView> _prefabs;
        private PlaneView _playerPlane;

        public PlaneFactory(ConfigContainer configs, JoystickInput joystick)
        {
            _configs = configs;
            _joystick = joystick;
        }

        UniTask ILoadUnit.Load()
        {
            string[] planesToLoad = RuntimeConstants.Planes.All;
            _prefabs = new Dictionary<string, PlaneView>(planesToLoad.Length);

            foreach (string planeToLoad in planesToLoad)
                _prefabs.Add(planeToLoad, Resources.Load<PlaneView>($"Planes/{planeToLoad}"));
            return UniTask.CompletedTask;
        }

        public PlaneView SpawnOrGetPlayerPlane(string planeName)
        {
            if (_playerPlane != null)
                return _playerPlane;

            _playerPlane = Object.Instantiate(_prefabs[planeName]);
            _playerPlane.Initialize(_configs.Battle.PlaneConfig, _joystick, true);
            return _playerPlane;
        }
    }
}