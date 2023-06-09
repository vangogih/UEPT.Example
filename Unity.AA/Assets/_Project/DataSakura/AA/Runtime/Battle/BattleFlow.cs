using System;
using DataSakura.AA.Runtime.Battle.Joystick;
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
        private readonly BotInput _botInput;

        public BattleFlow(LoadingService loadingService,
            PlaneFactory planeFactory,
            ShootingService shootingService,
            BattleController battleController,
            BotInput botInput)
        {
            _loadingService = loadingService;
            _planeFactory = planeFactory;
            _shootingService = shootingService;
            _battleController = battleController;
            _botInput = botInput;
        }

        public async void Start()
        {
            await _loadingService.BeginLoading(_planeFactory);
            await _loadingService.BeginLoading(_shootingService);
            await _loadingService.BeginLoading(_battleController, new LevelConfiguration(0));
            await _loadingService.BeginLoading(_botInput, _battleController.Bots);
            
            _battleController.StartBattle();
            Log.Battle.D("BattleFlow.Start()");
        }

        public void Dispose()
        {
            Log.Battle.D("BattleFlow.Dispose()");
        }
    }
}