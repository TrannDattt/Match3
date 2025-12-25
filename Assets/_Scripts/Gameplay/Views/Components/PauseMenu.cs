// using Match3.Controls;
using Match3.GameSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class PauseMenu : PopupMenu
    {
        [SerializeField] private GameToggle _bgmToggle;
        [SerializeField] private GameToggle _sfxToggle;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _retryButton;

        public override void Open()
        {
            base.Open();

            _bgmToggle.SetToggleWithoutNotify(AudioControl.Instance.GetVolumeStatus(AudioControl.EAudioType.BGM));
            _sfxToggle.SetToggleWithoutNotify(AudioControl.Instance.GetVolumeStatus(AudioControl.EAudioType.SFX));

            AudioControl.Instance.SetVolume(AudioControl.EAudioType.BGM, _bgmToggle.IsOn ? .5f : 0f);
        }

        public override void Close()
        {
            base.Close();
            
            AudioControl.Instance.SetVolume(AudioControl.EAudioType.BGM, _bgmToggle.IsOn ? 1f : 0f);
        }

        protected override void Start()
        {
            base.Start();

            _bgmToggle.OnValueChanged.AddListener(isOn =>
            {
                AudioControl.Instance.SetVolume(AudioControl.EAudioType.BGM, isOn ? 1 : 0);
            });

            _sfxToggle.OnValueChanged.AddListener(isOn =>
            {
                AudioControl.Instance.SetVolume(AudioControl.EAudioType.SFX, isOn ? 1 : 0);
            });

            _mainMenuButton.onClick.AddListener(async () =>
            {
                Close();
                await GameManager.Instance.ReturnToMainMenu();
            });

            _retryButton.onClick.AddListener(async () =>
            {
                Close();
                await GameManager.Instance.RestartLevel();
            });
        }
    }
}
