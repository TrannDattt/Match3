// using Match3.Controls;
using System.Collections;
using DG.Tweening;
using Match3.Datas.Runtime;
using Match3.GameSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class ResultMenu : PopupMenu
    {
        [SerializeField] private TextMeshProUGUI _tiltleText;
        [SerializeField] private RectTransform _star1;
        [SerializeField] private RectTransform _star2;
        [SerializeField] private RectTransform _star3;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private RectTransform _glowingBG;

        [Header("Audio")]
        [SerializeField] private AudioClip _levelClearedClip;
        [SerializeField] private AudioClip _levelFailedClip;
        [SerializeField] private AudioClip _starClip1;
        [SerializeField] private AudioClip _starClip3;
        [SerializeField] private AudioClip _starClip2;

        public IEnumerator OpenResultMenu(LevelRuntimeData runtimeData)
        {
            Open();
                
            _tiltleText.text = runtimeData.IsCleared ? "Cleared!" : "Failed";
            _nextButton.gameObject.SetActive(runtimeData.IsCleared);
            _glowingBG.gameObject.SetActive(runtimeData.IsCleared);

            _star1.localScale = Vector3.zero;
            _star2.localScale = Vector3.zero;
            _star3.localScale = Vector3.zero;

            _scoreText.text = "0";

            var audioControl = AudioControl.Instance;
            audioControl.PlaySfx(runtimeData.IsCleared ? _levelClearedClip : _levelFailedClip);

            yield return DOTween.To(() => 0, x => _scoreText.text = x.ToString(), runtimeData.CurrentScore, 0.5f).WaitForCompletion();

            // var sequence = DOTween.Sequence();
            if (runtimeData.CurrentScore >= runtimeData.LevelConfig.ScoreThreshold1)
            {
                // sequence.Append(_star1.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
                audioControl.PlaySfx(_starClip1);
                yield return _star1.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            }
            if (runtimeData.CurrentScore >= runtimeData.LevelConfig.ScoreThreshold2)
            {
                // sequence.Append(_star2.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
                audioControl.PlaySfx(_starClip2);
                yield return _star2.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            }
            if (runtimeData.CurrentScore >= runtimeData.LevelConfig.ScoreThreshold3)
            {
                // sequence.Append(_star3.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
                audioControl.PlaySfx(_starClip3);
                yield return _star3.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();
            }
            // sequence.Append(DOTween.To(() => 0, x => _scoreText.text = x.ToString(), runtimeData.CurrentScore, 0.5f));
            // yield return sequence.WaitForCompletion();
        }

        protected override void Start()
        {
            base.Start();

            _nextButton.onClick.AddListener(async () =>
            {
                Close();
                await GameManager.Instance.StartNextLevel();
            });

            _retryButton.onClick.AddListener(async () =>
            {
                Close();
                await GameManager.Instance.RestartLevel();
            });
        }
    }
}
