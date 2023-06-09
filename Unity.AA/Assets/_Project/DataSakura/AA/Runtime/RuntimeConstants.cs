using UnityEngine.SceneManagement;

namespace DataSakura.AA.Runtime
{
    public static class RuntimeConstants
    {
        public static class Planes
        {
            public const string Corncob = "corncob";
            public const string Jet = "jet";
            public const string Bombarding = "bombarding";
            
            public static readonly string[] All = {Corncob, Jet, Bombarding};
        }

        public static class Configs
        {
            public const string ConfigFileName = "Config.json";
        }

        public static class Scenes
        {
            public static readonly int Bootstrap = SceneUtility.GetBuildIndexByScenePath("0.Bootstrap");
            public static readonly int Battle = SceneUtility.GetBuildIndexByScenePath("1.Battle");
        }
    }
}