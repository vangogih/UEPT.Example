using Cysharp.Threading.Tasks;
using DataSakura.Runtime.Battle.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace DataSakura.Runtime.Battle.Input
{
    public class BaseJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] protected RectTransform background;
        [SerializeField] private RectTransform directionIndicator;
        [SerializeField] private RectTransform handle;
        [SerializeField] private BattleCanvasProvider canvasProvider;

        public Vector3 Direction => new Vector2(_input.x, _input.y);
        protected bool IsJoystickActiveNow { get; private set; }

        private JoystickConfig _joystickConfig;
        private RectTransform _baseRect;
        private Vector2 _input = Vector2.zero;

        [Inject]
        private void Inject(ConfigContainer configs)
        {
            _joystickConfig = configs.Battle.JoystickConfig;
        }

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (_joystickConfig == null) {
                var configContainer = new ConfigContainer();
                configContainer.Load().Forget();
                _joystickConfig = configContainer.Battle.JoystickConfig;
            }
#endif
            _baseRect = (RectTransform)transform;

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
            var radius = background.sizeDelta * _joystickConfig.HandleOffsetActivation;
            _input = (eventData.position - position) / (radius * canvasProvider.Canvas.scaleFactor);

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

            Vector2 sizeDelta = _baseRect.sizeDelta;
            var pivotOffset = _baseRect.pivot * sizeDelta;
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
            if (magnitude > _joystickConfig.JoystickJitter) {
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