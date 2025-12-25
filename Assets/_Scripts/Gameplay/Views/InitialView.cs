using Match3.GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views
{
    public class InitialView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private Button _tapToStartBtn;

        void Start()
        {
            _tapToStartBtn.gameObject.SetActive(false);
            _tapToStartBtn.onClick.AddListener(async () =>
            {
                await SceneNavigator.Instance.LoadSceneAsync(SceneNavigator.ESceneName.MainMenu);
            });

            GameManager.Instance.OnGameInitialized.AddListener(() =>
            {
                _loadingText.gameObject.SetActive(false);
                _tapToStartBtn.gameObject.SetActive(true);
            });
        }
    }
}
