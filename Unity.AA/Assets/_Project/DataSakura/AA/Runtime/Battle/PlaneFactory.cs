﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSakura.Runtime.Battle.Airplane;
using DataSakura.Runtime.Battle.Input;
using DataSakura.Runtime.Battle.Shooting;
using DataSakura.Runtime.Utilities;
using UnityEngine;
using Resources = UnityEngine.Resources;

namespace DataSakura.Runtime.Battle
{
    public class PlaneFactory : ILoadUnit
    {
        private readonly ConfigContainer _configs;
        private readonly JoystickInput _joystick;
        private readonly ShootingService _shootingService;
        private Dictionary<string, PlaneView> _prefabs;
        private PlaneView _playerPlane;

        public PlaneFactory(ConfigContainer configs, JoystickInput joystick, ShootingService shootingService)
        {
            _configs = configs;
            _joystick = joystick;
            _shootingService = shootingService;
        }

        UniTask ILoadUnit.Load()
        {
            string[] planesToLoad = RuntimeConstants.Planes.All;
            _prefabs = new Dictionary<string, PlaneView>(planesToLoad.Length);

            foreach (string planeToLoad in planesToLoad)
                _prefabs.Add(planeToLoad, Resources.Load<PlaneView>($"Planes/{planeToLoad}"));
            return UniTask.CompletedTask;
        }

        public PlaneView CreatePlayer(string planeName)
        {
            PlaneView player = Object.Instantiate(_prefabs[planeName]);
            player.Initialize(_configs.Battle.PlaneConfig, _joystick, true);
            return player;
        }

        public PlaneView CreateBot(string planeName, PlaneView playerPlane)
        {
            PlaneView bot = Object.Instantiate(_prefabs[planeName]);
            BotPlaneConfig botConfig = _configs.Battle.BotPlaneConfig;
            var botInput = new BotBrain(_shootingService, botConfig, bot, playerPlane);

            bot.Initialize(botConfig, botInput, false);
            bot.BuffDefaultSpeed(botConfig.DefaultSpeedModificator);
            return bot;
        }
    }
}