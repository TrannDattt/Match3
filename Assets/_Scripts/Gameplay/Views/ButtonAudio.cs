using Match3.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views
{
    public class ButtonAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip _buttonClickSfx;

        private Button _button;

        void Awake()
        {
            _button = GetComponentInChildren<Button>();
            if (_button && _buttonClickSfx)
            {
                _button.onClick.AddListener(() =>
                {
                    AudioControl.Instance.PlaySfx(_buttonClickSfx);
                });
            }
        }
    }
}
