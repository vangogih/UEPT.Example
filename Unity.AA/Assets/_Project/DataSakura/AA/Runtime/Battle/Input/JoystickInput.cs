using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DataSakura.AA.Runtime.Battle.Joystick
{
    public sealed class JoystickInput : BaseJoystick, IJoystickInput
    {
        public bool IsPressed { get; private set; }
        public IObservable<Vector2> OnDragEvent => _onDragSbj ??= new Subject<Vector2>();
        public IObservable<Unit> OnPointerDownEvent => _onPointerDownSbj ??= new Subject<Unit>();
        public IObservable<Unit> OnPointerUpEvent => _onPointerUpSbj ??= new Subject<Unit>();
        private Subject<Vector2> _onDragSbj;
        private Subject<Unit> _onPointerDownSbj;
        private Subject<Unit> _onPointerUpSbj;

        private Vector2 _fixedPosition = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
            _fixedPosition = background.anchoredPosition;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _onPointerUpSbj?.OnNext(Unit.Default);
            
            background.anchoredPosition = _fixedPosition;
            IsPressed = false;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!IsPressed)
                return;

            base.OnDrag(eventData);
            _onDragSbj?.OnNext(Direction);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _onPointerDownSbj?.OnNext(Unit.Default);
            IsPressed = true;
        }
    }
}