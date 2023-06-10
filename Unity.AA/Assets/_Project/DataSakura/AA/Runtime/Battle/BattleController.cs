using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Battle.UI;
using DataSakura.AA.Runtime.Utilities;
using UniRx;

namespace DataSakura.AA.Runtime.Battle
{
    public class BattleController : ILoadUnit<LevelConfiguration>
    {
        public IReadOnlyList<PlaneView> Bots => _bots;
        public PlaneView Player => _player;

        private readonly PlaneFactory _planeFactory;
        private readonly BattleHudController _battleHudController;
        private readonly ShootingService _shootingService;
        private PlaneView _player;
        private List<PlaneView> _bots;
        private readonly CompositeDisposable _disposables = new();

        public BattleController(PlaneFactory planeFactory,
            BattleHudController battleHudController,
            ShootingService shootingService)
        {
            _planeFactory = planeFactory;
            _battleHudController = battleHudController;
            _shootingService = shootingService;
        }

        public UniTask Load(LevelConfiguration levelConfiguration)
        {
            // spawn player and bots
            {
                _player = _planeFactory.CreatePlayer(RuntimeConstants.Planes.Corncob);
                _player.OnDead.RxSubscribe(isDead => OnPlayerDead(isDead, _player)).AddTo(_disposables);
                _player.gameObject.SetActive(false);

                _bots = new List<PlaneView>(levelConfiguration.EnemiesCount);

                for (var i = 0; i < levelConfiguration.EnemiesCount; i++) {
                    PlaneView bot = _planeFactory.CreateBot(RuntimeConstants.Planes.Corncob, _player);
                    bot.OnDead.RxSubscribe(isDead => OnBotDie(isDead, bot)).AddTo(_disposables);
                    bot.gameObject.SetActive(false);
                    _bots.Add(bot);
                }
            }

            _battleHudController.Initialize(_player);

            return UniTask.CompletedTask;
        }

        public void StartBattle()
        {
            _player.gameObject.SetActive(true);

            for (var i = 0; i < _bots.Count; i++)
                _bots[i].gameObject.SetActive(true);
        }

        public void RestartBattle()
        {
            _disposables.Dispose();
        }

        private void OnPlayerDead(bool isDead, PlaneView view)
        {
            if (!isDead)
                return;

            RestartBattle();
            _battleHudController.ShowGameOver();
        }

        private void OnBotDie(bool isDead, PlaneView view)
        {
            if (!isDead)
                return;

            _bots.Remove(view);

            if (_bots.Count == 0)
                _battleHudController.ShowWin();
        }
    }

    // TODO: Move to config file
    public readonly struct LevelConfiguration
    {
        public readonly int EnemiesCount;

        public LevelConfiguration(int enemiesCount)
        {
            EnemiesCount = enemiesCount;
        }
    }
}