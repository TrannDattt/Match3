// using Match3.Controls;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Views.Components
{
    public class StarBarView : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private Image _star3;

        [SerializeField] private float _offsetToMaxScore = .2f;

        private RectTransform _star1Rect;
        private RectTransform _star2Rect;
        private RectTransform _star3Rect;

        public void InitBar(int maxScore, int threshold1, int threshold2, int threshold3)
        {
            _progressBar.value = 0f;
            _star1Rect.localScale = Vector3.one;
            _star2Rect.localScale = Vector3.one;
            _star3Rect.localScale = Vector3.one;

            // var barWidth = (_progressBar.fillRect.parent as RectTransform).rect.width;

            void SetStarPosition(RectTransform starRect, int threshold)
            {
                var percentage = threshold / (maxScore * (1 + _offsetToMaxScore));
                starRect.anchorMin = new Vector2(percentage, .5f);
                starRect.anchorMax = new Vector2(percentage, .5f);
                starRect.anchoredPosition = Vector2.zero;
            }
            SetStarPosition(_star1Rect, threshold1);
            SetStarPosition(_star2Rect, threshold2);
            SetStarPosition(_star3Rect, threshold3);
        }

        public void UpdateStarBar(int currentScore, int maxScore, int threshold1, int threshold2, int threshold3)
        {
            if (maxScore <= 0) return;

            float progress = currentScore / (maxScore * (1 + _offsetToMaxScore));
            progress = Mathf.Clamp01(progress);
            DOTween.To(() => _progressBar.value,
                       x => 
                        {
                            _progressBar.value = x;
                            if (currentScore >= threshold1)
                                DOTween.To(() => _star1Rect.localScale,
                                           y => _star1Rect.localScale = y,
                                           1.2f * Vector3.one,
                                           0.3f).SetEase(Ease.OutBack);
                            if (currentScore >= threshold2)
                                DOTween.To(() => _star2Rect.localScale,
                                           y => _star2Rect.localScale = y,
                                           1.2f * Vector3.one,
                                           0.3f).SetEase(Ease.OutBack);
                            if (currentScore >= threshold3)
                                DOTween.To(() => _star3Rect.localScale,
                                           y => _star3Rect.localScale = y,
                                           1.2f * Vector3.one,
                                           0.3f).SetEase(Ease.OutBack);
                        },
                       progress,
                       0.5f).SetEase(Ease.Linear);
        }

        void Awake()
        {
            _star1Rect = _star1.GetComponent<RectTransform>();
            _star2Rect = _star2.GetComponent<RectTransform>();
            _star3Rect = _star3.GetComponent<RectTransform>();
        }
    }
}
