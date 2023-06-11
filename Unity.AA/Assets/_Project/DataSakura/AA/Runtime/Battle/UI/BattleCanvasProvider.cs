using System;
using DataSakura.Runtime.Utilities;
using UniRx;
using UnityEngine;
using VContainer;

namespace DataSakura.Runtime.Battle.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class BattleCanvasProvider : MonoBehaviour
    {
        public Canvas Canvas;
        public RectTransform CanvasRect => (RectTransform)Canvas.transform;
        public RectTransform SafeContainer;
        
        private IDisposable _disposable;

        [Inject]
        [UnityEngine.Scripting.Preserve]
        public void Inject(OrientationHelper orientationHelper)
        {
            _disposable = orientationHelper.OrientationChanged.RxSubscribe(o => FitSafeContainer());
        }

        public void FitSafeContainer()
        {
            SafeContainer.FitInSafeArea();
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}