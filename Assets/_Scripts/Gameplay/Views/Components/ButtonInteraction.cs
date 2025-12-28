using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class ButtonInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private RectTransform _buttonRt;
        private Button _button;
        private Vector3 _baseScale;
        private Tween _curTween;

        [SerializeField] private float _shrinkRate = .9f;
        [SerializeField] private float _bulgeRate = 1.1f;

        private void ChangeScale(float scaleMult, float duration)
        {
            _curTween.Kill();

            var fromScale = _buttonRt.localScale;
            var toScale = scaleMult * _baseScale;
            _curTween = DOTween.To(() => fromScale,
                                   x => _buttonRt.localScale = x,
                                   toScale,
                                   duration);
        }

        void Awake()
        {
            _button = GetComponentInChildren<Button>();
            _buttonRt = GetComponent<RectTransform>();
            _baseScale = _buttonRt.localScale;
        }

        void OnDisable()
        {
            _curTween?.Kill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ChangeScale(_bulgeRate, .1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ChangeScale(1, .1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ChangeScale(1, .1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ChangeScale(_shrinkRate, .1f);
        }
    }
}

