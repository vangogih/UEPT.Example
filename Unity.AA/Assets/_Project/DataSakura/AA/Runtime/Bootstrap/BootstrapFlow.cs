using DataSakura.AA.Runtime.Utilities.Logging;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Bootstrap
{
    public class BootstrapFlow : IStartable
    {
        public void Start()
        {
            Log.Default.D("BootstrapFlow.Start()");
        }
    }
}