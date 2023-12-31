﻿using System;
using UniRx;
using UnityEngine;

namespace DataSakura.Runtime.Battle.Input
{
    public interface IInput
    {
        Vector3 Direction { get; }
        bool IsPressed { get; }
    }

    public interface IJoystickInput : IInput
    {
        IObservable<Vector2> OnDragEvent { get; }
        IObservable<Unit> OnPointerDownEvent { get; }
        IObservable<Unit> OnPointerUpEvent { get; }
    }
}