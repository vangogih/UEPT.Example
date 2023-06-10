using System.IO;
using DataSakura.AA.Runtime;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace _Project.DataSakura.AA.Editor
{
    public class ConfigGenerator
    {
        [MenuItem("DataSakura/Generate Configs")]
        public static void Generate()
        {
            var configContainer = new ConfigContainer
            {
                Battle = new BattleConfigContainer
                {
                    PlaneConfig = new PlaneConfig
                    {
                        Responsiveness = 0.5f,
                        Bullet = new BulletConfig
                        {
                            Speed = 500,
                            LifeTime = 5
                        }
                    },
                    BotPlaneConfig = new BotPlaneConfig
                    {
                        ShootInterval = 1,
                        DistanceToShoot = 100,
                        Responsiveness = 1f,
                        Bullet = new BulletConfig
                        {
                            Speed = 500,
                            LifeTime = 5
                        }
                    },
                    JoystickConfig = new JoystickConfig
                    {
                        JoystickJitter = .5f,
                        HandleOffsetActivation = .05f
                    },
                    WaterConfig = new WaterConfig
                    {
                        WaterLevel = 0,
                        WaterSpeedModificator = .5f
                    }
                }
            };
            var json = JsonConvert.SerializeObject(configContainer, Formatting.Indented);
            var path = Path.Combine(Application.dataPath, "_Project", "Resources", RuntimeConstants.Configs.ConfigFileName + ".json");
            File.WriteAllText(path, json);
        }
    }
}