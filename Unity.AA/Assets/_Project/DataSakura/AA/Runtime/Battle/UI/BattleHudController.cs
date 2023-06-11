using DataSakura.Runtime.Battle.Airplane;
using DataSakura.Runtime.Battle.Shooting;
using DataSakura.Runtime.Utilities;
using DataSakura.Runtime.Utilities.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace DataSakura.Runtime.Battle.UI
{
    public sealed class BattleHudController : MonoBehaviour
    {
        [Header("Refs")] [SerializeField] private RectTransform crossHair;
        [SerializeField] private Button shootBtn;

        private PlaneView _player;
        private RectTransform _canvasRect;
        private ShootingService _shootingService;

        [Inject]
        private void Inject(BattleCanvasProvider battleCanvasProvider, ShootingService shootingService)
        {
            _shootingService = shootingService;
            _canvasRect = battleCanvasProvider.CanvasRect;
        }

        public void Initialize(PlaneView player)
        {
            _player = player;
            shootBtn.OnClickAsObservable().RxSubscribe(OnShootClicked).AddTo(this);
        }

        private void OnShootClicked(Unit _)
        {
            _shootingService.Shoot(_player);
        }

        private void Update()
        {
            if (_player == null)
                return;

            Vector3 crossPos = _player.transform.position + _player.transform.forward * 100f;
            crossHair.anchoredPosition = CoordinateTransformer.FromWorldToCanvasLocalPos(crossPos, _player.planeCamera, _canvasRect);
        }

        public void ShowGameOver()
        {
            Log.Battle.D("GAME OVER");
        }

        public void ShowWin()
        {
            Log.Battle.D("WIN");
        }
    }
}