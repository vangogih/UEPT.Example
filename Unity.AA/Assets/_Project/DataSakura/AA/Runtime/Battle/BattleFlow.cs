using System;
using DataSakura.AA.Runtime.Battle.UI;
using DataSakura.AA.Runtime.Utilities;
using DataSakura.AA.Runtime.Utilities.Logging;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Battle
{
    public class BattleFlow : IStartable, IDisposable
    {
        private readonly LoadingService _loadingService;
        private readonly PlaneFactory _planeFactory;
        private readonly BattleHudController _battleHudController;
        private readonly ShootingService _shootingService;

        public BattleFlow(LoadingService loadingService,
            PlaneFactory planeFactory,
            BattleHudController battleHudController,
            ShootingService shootingService)
        {
            _loadingService = loadingService;
            _planeFactory = planeFactory;
            _battleHudController = battleHudController;
            _shootingService = shootingService;
        }

        public async void Start()
        {
            await _loadingService.BeginLoading(_planeFactory);
            await _loadingService.BeginLoading(_shootingService);

            var playerView = _planeFactory.SpawnOrGetPlayerPlane(RuntimeConstants.Planes.Corncob);
            _battleHudController.Initialize(playerView);
            Log.Battle.D("BattleFlow.Start()");
        }

        public void Dispose()
        {
            Log.Battle.D("BattleFlow.Dispose()");
        }
    }
}