using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataSakura.Runtime.Battle.Airplane;
using DataSakura.Runtime.Battle.UI;
using DataSakura.Runtime.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DataSakura.Runtime.Battle
{
    public class BattleController : ILoadUnit<LevelConfiguration>
    {
        public IReadOnlyList<PlaneView> Bots => _bots;
        public PlaneView Player { get; private set; }

        private readonly PlaneFactory _planeFactory;
        private readonly BattleHudController _battleHudController;
        private List<PlaneView> _bots;
        private readonly CompositeDisposable _disposables = new();

        public BattleController(PlaneFactory planeFactory, BattleHudController battleHudController)
        {
            _planeFactory = planeFactory;
            _battleHudController = battleHudController;
        }

        public UniTask Load(LevelConfiguration levelConfiguration)
        {
            // spawn player and bots
            {
                Player = _planeFactory.CreatePlayer(RuntimeConstants.Planes.Corncob);
                Player.IsDead.RxSubscribe(isDead => OnPlayerDead(isDead, Player)).AddTo(_disposables);
                Player.gameObject.SetActive(false);

                _bots = new List<PlaneView>(levelConfiguration.EnemiesCount);

                for (var i = 0; i < levelConfiguration.EnemiesCount; i++) {
                    PlaneView bot = _planeFactory.CreateBot(RuntimeConstants.Planes.Corncob, Player);
                    bot.IsDead.RxSubscribe(isDead => OnBotDie(isDead, bot)).AddTo(_disposables);
                    bot.transform.position = Random.insideUnitSphere * 15 + Player.transform.forward * 50 + Vector3.up * 15;
                    bot.gameObject.SetActive(false);
                    _bots.Add(bot);
                }
            }

            _battleHudController.Initialize(Player);

            return UniTask.CompletedTask;
        }

        public void StartBattle()
        {
            Player.gameObject.SetActive(true);

            for (var i = 0; i < _bots.Count; i++)
                _bots[i].gameObject.SetActive(true);
        }

        private void RestartBattle()
        {
            _disposables.Dispose();
            SceneManager.LoadScene(RuntimeConstants.Scenes.Battle);
        }

        private void OnPlayerDead(bool isDead, PlaneView view)
        {
            if (!isDead)
                return;

            _battleHudController.ShowGameOver();
            RestartBattle();
        }

        private void OnBotDie(bool isDead, PlaneView view)
        {
            if (!isDead)
                return;

            _bots.Remove(view);

            if (_bots.Count != 0)
                return;
            
            _battleHudController.ShowWin();
            RestartBattle();
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