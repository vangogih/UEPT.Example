namespace DataSakura.AA.Runtime.Utilities
{
    public class AssetService
    {
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            return UnityEngine.Resources.Load<T>(path);
        }
    }
}