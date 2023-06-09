using System.IO;
using DataSakura.AA.Runtime;
using Unity.Plastic.Newtonsoft.Json;
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
                        Responsiveness = 0.1f,
                        WaterSpeedModificator = .5f,
                        Bullet = new BulletConfig
                        {
                            Speed = 5,
                            LifeTime = 5
                        }
                    },
                    BotPlaneConfig = new BotPlaneConfig
                    {
                        Speed = 4,
                        ShootInterval = 3,
                        DistanceToShoot = 10,
                        WaterSpeedModificator = .5f,
                        Bullet = new BulletConfig
                        {
                            Speed = 5,
                            LifeTime = 5
                        }
                    },
                    JoystickConfig = new JoystickConfig
                    {
                        JoystickJitter = .5f,
                        HandleOffsetActivation = .05f
                    }
                }
            };
            var json = JsonConvert.SerializeObject(configContainer, Formatting.Indented);
            File.WriteAllText(Path.Combine(Application.streamingAssetsPath, RuntimeConstants.Configs.ConfigFileName), json);
        }
    }
}