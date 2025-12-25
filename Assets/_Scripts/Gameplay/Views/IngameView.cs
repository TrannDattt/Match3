// using Match3.Controls;
using Match3.Datas.Runtime;
using Match3.GameSystem;
using Match3.Views.Components;
using UnityEngine;

namespace Match3.Views
{
    public class IngameView : ViewBase
    {
        [SerializeField] private TurnCountView _turnCountView;
        [SerializeField] private ScoreView _scoreView;
        [SerializeField] private StarBarView _starBarView;
        [SerializeField] private ObjectiveTrackerView _objectiveTrackerView;

        [SerializeField] private ResultMenu _resultMenu;

        public void Init(LevelRuntimeData levelData)
        {
            _turnCountView.UpdateTurn(levelData.RemainingTurns);
            _scoreView.UpdateScore(levelData.CurrentScore);
            _starBarView.InitBar(levelData.LevelConfig.ScoreThreshold3,
                                  levelData.LevelConfig.ScoreThreshold1,
                                  levelData.LevelConfig.ScoreThreshold2,
                                  levelData.LevelConfig.ScoreThreshold3);
            _objectiveTrackerView.InitTracker(levelData.ObjectiveTrackers);

            levelData.OnTurnChanged.AddListener(() =>
            {
                _turnCountView.UpdateTurn(levelData.RemainingTurns);
            });

            levelData.OnScoreChanged.AddListener(() =>
            {
                _scoreView.UpdateScore(levelData.CurrentScore);
            });

            levelData.OnScoreChanged.AddListener(() =>
            {
                _starBarView.UpdateStarBar(
                    levelData.CurrentScore,
                    levelData.LevelConfig.ScoreThreshold3,
                    levelData.LevelConfig.ScoreThreshold1,
                    levelData.LevelConfig.ScoreThreshold2,
                    levelData.LevelConfig.ScoreThreshold3
                );
            });

            levelData.OnObjectiveUpdated.AddListener(() =>
            {
                _objectiveTrackerView.UpdateTracker(levelData.ObjectiveTrackers);
            });

            levelData.OnFinishedLevel.AddListener(() =>
            {
                StartCoroutine(_resultMenu.OpenResultMenu(levelData));
            });
        }

        void Start()
        {
        }
    }
}
