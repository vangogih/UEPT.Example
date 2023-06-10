using System;
using System.IO;
using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Utilities;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace DataSakura.AA.Runtime
{
    public sealed class ConfigContainer : ILoadUnit
    {
        public BattleConfigContainer Battle;
        
        public UniTask Load()
        {
            string path = Path.Combine(Application.streamingAssetsPath, RuntimeConstants.Configs.ConfigFileName);
            JsonConvert.PopulateObject(File.ReadAllText(path), this);
            return UniTask.CompletedTask;
        }
    }
    
    [Serializable]
    public class BattleConfigContainer
    {
        public PlaneConfig PlaneConfig;
        public BotPlaneConfig BotPlaneConfig;
        public JoystickConfig JoystickConfig;
        public WaterConfig WaterConfig;
    }

    [Serializable]
    public class PlaneConfig
    {
        public float Responsiveness;
        public BulletConfig Bullet;
    }

    [Serializable]
    public class BotPlaneConfig : PlaneConfig
    {
        public float ShootInterval;
        public float DistanceToShoot;
    }

    [Serializable]
    public class BulletConfig
    {
        public float Speed;
        public float LifeTime;
    }

    [Serializable]
    public sealed class JoystickConfig
    {
        public float HandleOffsetActivation;
        public float JoystickJitter;
    }

    [Serializable]
    public sealed class WaterConfig
    {
        public float WaterLevel;
        public float WaterSpeedModificator;
    }
}