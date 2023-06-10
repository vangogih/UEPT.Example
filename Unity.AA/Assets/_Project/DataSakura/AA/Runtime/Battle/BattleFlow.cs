using System;
using DataSakura.AA.Runtime.Utilities;
using DataSakura.AA.Runtime.Utilities.Logging;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public class BattleFlow : IStartable, IDisposable
    {
        private readonly LoadingService _loadingService;
        private readonly PlaneFactory _planeFactory;
        private readonly ShootingService _shootingService;
        private readonly BattleController _battleController;
        private readonly WaterBuffService _waterBuffService;

        public BattleFlow(LoadingService loadingService,
            PlaneFactory planeFactory,
            ShootingService shootingService,
            BattleController battleController,
            WaterBuffService waterBuffService)
        {
            _loadingService = loadingService;
            _planeFactory = planeFactory;
            _shootingService = shootingService;
            _battleController = battleController;
            _waterBuffService = waterBuffService;
        }

        public async void Start()
        {
            await _loadingService.BeginLoading(_planeFactory);
            await _loadingService.BeginLoading(_shootingService);
            await _loadingService.BeginLoading(_battleController, new LevelConfiguration(0));
            await _loadingService.BeginLoading(_waterBuffService);
            
            _battleController.StartBattle();
            Log.Battle.D("BattleFlow.Start()");
        }

        public void Dispose()
        {
            Log.Battle.D("BattleFlow.Dispose()");
        }
    }
}