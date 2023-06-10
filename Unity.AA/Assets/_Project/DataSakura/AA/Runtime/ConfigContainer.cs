﻿using System;
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
        /// <summary> How sensitively the AI applies the roll controls</summary>
        public float RollSensitivity = .5f;      
        /// <summary> How sensitively the AI applies the pitch controls</summary>
        public float PitchSensitivity = .5f;     
        /// <summary> The amount that the plane can wander by when heading for a target</summary>
        public float LateralWanderDistance = 2;  
        /// <summary> The speed at which the plane will wander laterally</summary>
        public float LateralWanderSpeed = 0.11f; 
        /// <summary> The maximum angle that the AI will attempt to make plane can climb at</summary>
        public float MaxClimbAngle = 45;         
        /// <summary> The maximum angle that the AI will attempt to u</summary>
        public float MaxRollAngle = 70;          
        /// <summary> This increases the effect of the controls based on the plane's speed.</summary>
        public float SpeedEffect = 0.01f;        
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