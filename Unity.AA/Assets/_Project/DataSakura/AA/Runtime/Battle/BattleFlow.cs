using DataSakura.AA.Runtime.Utilities.Logging;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public class BattleFlow : IStartable
    {
        public void Start()
        {
            Log.Default.D("BattleFlow.Start()");
        }
    }
}