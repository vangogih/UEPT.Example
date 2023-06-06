using UnityEngine;
using UnityEngine.EventSystems;

namespace DataSakura.AA.Runtime.Battle.Joystick
{
    public class BaseJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] protected RectTransform background;
        [SerializeField] private RectTransform directionIndicator;
        [SerializeField] private RectTransform handle;

        protected Vector2 Direction => new Vector2(-_input.x, -_input.y);
        protected bool IsJoystickActiveNow { get; private set; }

        private JoystickSettings _joystickSettings;
        private RectTransform _baseRect;
        private Canvas _canvas;
        private Vector2 _input = Vector2.zero;

        protected virtual void Awake()
        {
            _baseRect = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();

            if (_canvas == null)
                Debug.LogError("The Joystick is not placed inside a canvas");

            var center = new Vector2(0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            var position = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, background.position);
            var radius = background.sizeDelta * _joystickSettings.HandleOffsetActivation;
            _input = (eventData.position - position) / (radius * _canvas.scaleFactor);

            //Ограничение грибка джойстика в UI, чтобы не вылезал за пределы своей области
            handle.anchoredPosition = Vector2.ClampMagnitude(radius * _input, handle.sizeDelta.x / 3);

            directionIndicator.rotation =
                Quaternion.Euler(0f, 0f, -Mathf.Atan2(_input.normalized.x, _input.normalized.y) * Mathf.Rad2Deg);

            HandleInput(_input.magnitude, _input.normalized);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint)) {
                IsJoystickActiveNow = false;
                return;
            }

            Vector2 sizeDelta;
            var pivotOffset = _baseRect.pivot * (sizeDelta = _baseRect.sizeDelta);
            background.anchoredPosition = localPoint - background.anchorMax * sizeDelta + pivotOffset;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            IsJoystickActiveNow = false;
            _input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            directionIndicator.rotation = Quaternion.identity;
        }

        private void HandleInput(float magnitude, Vector2 normalised)
        {
            if (magnitude > _joystickSettings.JoystickJitter) {
                _input = normalised;
                IsJoystickActiveNow = true;
            }
            else {
                _input = Vector2.zero;
                IsJoystickActiveNow = false;
            }
        }
    }
}