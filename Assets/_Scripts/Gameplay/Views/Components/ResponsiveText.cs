// using Match3.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class ResponsiveText : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private Text _text;
        [SerializeField] private float _padding = 5f;

        #if UNITY_EDITOR
        void OnValidate()
        {
            var containerHeight = _container.rect.height;
            var containerWidth = _container.rect.width;
            var fontSize = Mathf.Min(containerHeight, containerWidth) - _padding * 2;

            if (fontSize > 0)
            {
                _text.fontSize = (int)fontSize;
            }
        }
        #endif
    }
}
