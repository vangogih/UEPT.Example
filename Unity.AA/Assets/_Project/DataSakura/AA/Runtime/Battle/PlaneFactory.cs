using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Battle.Joystick;
using DataSakura.AA.Runtime.Utilities;
using HeneGames.Airplane;
using UnityEngine;

namespace DataSakura.AA.Runtime.Battle
{
    public class PlaneFactory : ILoadUnit
    {
        private readonly ConfigContainer _configs;
        private readonly JoystickInput _joystick;
        private PlaneView _prefab;
        private PlaneView _playerPlane;

        public PlaneFactory(ConfigContainer configs, JoystickInput joystick)
        {
            _configs = configs;
            _joystick = joystick;
        }

        UniTask ILoadUnit.Load()
        {
            _prefab = AssetService.R.Load<PlaneView>("Plane");
            return UniTask.CompletedTask;
        }

        public PlaneView SpawnOrGetPlayerPlane()
        {
            if (_playerPlane != null)
                return _playerPlane;

            _playerPlane = Object.Instantiate(_prefab);
            _playerPlane.Initialize(_configs.Battle.PlaneConfig, _joystick);
            return _playerPlane;
        }
    }
}