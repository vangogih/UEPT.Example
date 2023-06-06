using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;

namespace DataSakura.AA.Runtime.Utilities
{
    public class OrientationHelper : IFixedTickable, IDisposable
    {
        private readonly ReactiveProperty<ScreenOrientation> _orientation = new(Screen.orientation);
        public IReactiveProperty<ScreenOrientation> OrientationChanged => _orientation;

        public void FixedTick()
        {
            ScreenOrientation newOrientation = Screen.orientation;

            if (newOrientation == _orientation.Value)
                return;

            switch (newOrientation) {
                case ScreenOrientation.Unknown:
                case ScreenOrientation.Portrait:
                case ScreenOrientation.PortraitUpsideDown:
                case ScreenOrientation.AutoRotation:
                    return;
            }

            _orientation.Value = newOrientation;
        }

        public void Dispose()
        {
            _orientation.Dispose();
        }
    }
}