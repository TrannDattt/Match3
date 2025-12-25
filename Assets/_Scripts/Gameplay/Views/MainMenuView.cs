// using Match3.Controls;
using UnityEngine;
using UnityEngine.UI;
using Match3.GameSystem;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Match3.Views
{
    public class MainMenuView : ViewBase
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _levelButton;
        [SerializeField] private Button _quitButton;

        void Start()
        {
            _startButton.GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.GetLevelLastClearStatus(1) ? "Continue" : "Start";
            _startButton.onClick.AddListener(async () =>
            {
                await GameManager.Instance.StartLevel();
            });

            _levelButton.onClick.AddListener(async () =>
            {
                await SceneNavigator.Instance.LoadSceneAsync(SceneNavigator.ESceneName.LevelSelect);
            });
            
            _quitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }
}
