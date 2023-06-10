using _Project.DataSakura.AA.Editor.ManualTests;
using DataSakura.AA.Runtime;
using DataSakura.AA.Runtime.Battle;
using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Battle.Joystick;
using UnityEngine;

public class BotBrainDebug : MonoBehaviour
{
    public PlaneView Source;
    public TestFollowable Target;
    private BotBrain _bb;

    private async void Awake()
    {
        var cfg = new ConfigContainer();
        await cfg.Load();
        var sh = new ShootingService(cfg);
        await sh.Load();
        _bb = new BotBrain(sh, cfg.Battle.BotPlaneConfig, Source, Target);
        Source.Initialize(cfg.Battle.BotPlaneConfig, _bb, true);
    }

    private void FixedUpdate()
    {
        _bb.FixedTick();
    }
}
