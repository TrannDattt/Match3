// using Match3.Controls;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class GameToggle : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _toggleRect;
        [SerializeField] private float _padding = .2f;
        // [SerializeField] private bool _reverseOnOff;
        public UnityEvent<bool> OnValueChanged;

        public bool IsOn { get; private set; }

        private Button _interactableRect;

        public void SetToggleWithoutNotify(bool isOn)
        {
            IsOn = isOn;

            var anchorMin = new Vector2(!isOn ? _padding : .5f + _padding, _toggleRect.anchorMin.y);
            var anchorMax = new Vector2(!isOn ? .5f - _padding : 1 - _padding, _toggleRect.anchorMax.y);
            _toggleRect.DOAnchorMin(anchorMin, 0.2f);
            _toggleRect.DOAnchorMax(anchorMax, 0.2f);
        }

        private void SetToggle(bool isOn)
        {
            SetToggleWithoutNotify(isOn);

            OnValueChanged?.Invoke(IsOn);
        }

        public void Toggle()
        {
            SetToggle(!IsOn);
        }

        void Awake()
        {
            _interactableRect = _container.GetComponent<Button>();
            _interactableRect.onClick.AddListener(Toggle);
        }

        // void Start()
        // {
        //     SetToggle(false);
        // }
    }
}
