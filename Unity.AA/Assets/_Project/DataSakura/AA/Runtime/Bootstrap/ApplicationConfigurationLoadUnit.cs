using Cysharp.Threading.Tasks;
using DataSakura.Runtime.Utilities;
using UnityEngine;

namespace DataSakura.Runtime.Bootstrap
{
    public class ApplicationConfigurationLoadUnit : ILoadUnit
    {
        public UniTask Load()
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
            return UniTask.CompletedTask;
        }
    }
}