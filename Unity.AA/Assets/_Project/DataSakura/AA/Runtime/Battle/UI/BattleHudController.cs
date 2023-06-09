using DataSakura.AA.Runtime.Battle.Airplane;
using DataSakura.AA.Runtime.Utilities.Logging;
using Silverfox.Runtime.UI;
using Silverfox.Runtime.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace DataSakura.AA.Runtime.Battle.UI
{
    public sealed class BattleHudController : MonoBehaviour
    {
        [Header("Refs")] [SerializeField] private RectTransform crossHair;
        [SerializeField] private Button shootBtn;

        private PlaneView _planeView;
        private RectTransform _canvasRect;
        private ShootingService _shootingService;

        [Inject]
        private void Inject(BattleCanvasProvider battleCanvasProvider, ShootingService shootingService)
        {
            _shootingService = shootingService;
            _canvasRect = battleCanvasProvider.CanvasRect;
        }

        public void Initialize(PlaneView planeView)
        {
            _planeView = planeView;

            shootBtn.OnClickAsObservable().RxSubscribe(OnShootClicked).AddTo(this);
        }

        private void OnShootClicked(Unit _)
        {
            _shootingService.Shoot(_planeView);
        }

        private void Update()
        {
            if (_planeView == null)
                return;

            Vector3 crossPos = _planeView.transform.position + _planeView.transform.forward * 100f;
            crossHair.anchoredPosition = CoordinateTransformer.FromWorldToCanvasLocalPos(crossPos, _planeView.planeCamera, _canvasRect);
        }

        public void ShowGameOver()
        {
            Log.Battle.D("GAME OVER");
            SceneManager.LoadScene(RuntimeConstants.Scenes.Battle);
        }

        public void ShowWin()
        {
            Log.Battle.D("WIN");
            SceneManager.LoadScene(RuntimeConstants.Scenes.Battle);
        }
    }
}