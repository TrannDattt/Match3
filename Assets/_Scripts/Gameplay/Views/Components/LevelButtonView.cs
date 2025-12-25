// using Match3.Controls;
using Match3.Datas;
using Match3.Datas.Runtime;
using Match3.GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class LevelButtonView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _numberText;
        [SerializeField] private Button _levelButton;
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private Image _star3;

        public void Setup(LevelConfigSO levelConfig, LevelDataJSON lastRuntimeData)
        {
            if (levelConfig.LevelNumber == 1 || GameManager.Instance.GetLevelLastClearStatus(levelConfig.LevelNumber - 1))
            {
                _levelButton.interactable = true;
                _canvasGroup.alpha = 1;
            }
            else
            {
                _levelButton.interactable = false;
                _canvasGroup.alpha = .35f;
            }

            _numberText.text = levelConfig.LevelNumber.ToString();
            _levelButton.onClick.AddListener(async () =>
            {
                var levelData = new LevelRuntimeData(levelConfig);
                await GameManager.Instance.StartLevel(levelData);
            });

            var bestScore = lastRuntimeData != null ? lastRuntimeData.BestScore : 0;
            _star1.gameObject.SetActive(bestScore >= levelConfig.ScoreThreshold1);
            _star2.gameObject.SetActive(bestScore >= levelConfig.ScoreThreshold2);
            _star3.gameObject.SetActive(bestScore >= levelConfig.ScoreThreshold3);
        }
    }
}
