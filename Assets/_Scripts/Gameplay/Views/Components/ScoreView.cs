// using Match3.Controls;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Match3.Views.Components
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        public void UpdateScore(int score)
        {
            var curScore = int.Parse(_scoreText.text);
            DOTween.To(() => curScore,
                       x => 
                        {
                            curScore = x;
                            _scoreText.text = $"{curScore}";
                        },
                       score,
                       0.5f).SetEase(Ease.OutCubic);
        }

        // void Start()
        // {
        //     UpdateScore(0);
        // }
    }
}
