using System.Collections.Generic;
using System.Linq;
using Match3.Board;
using Match3.GameSystem;
using UnityEngine;
using UnityEngine.Events;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Datas.Runtime
{
    public class LevelRuntimeData
    {
        public class ObjectiveTracker
        {
            public LevelObjective Objective;
            public int CurrentAmount;

            public ObjectiveTracker(LevelObjective objective)
            {
                Objective = objective;
                CurrentAmount = 0;
            }
        }

        public LevelConfigSO LevelConfig;
        public int RemainingTurns;
        public int CurrentScore;
        public List<ObjectiveTracker> ObjectiveTrackers = new();
        public bool IsCleared;
        
        public UnityEvent OnScoreChanged = new();
        public UnityEvent OnTurnChanged = new();
        public UnityEvent OnObjectiveUpdated = new();
        public UnityEvent OnFinishedLevel = new(); // Level finished when clear objectives or out of turns

        public LevelRuntimeData(LevelConfigSO config)
        {
            LevelConfig = config;
            CurrentScore = 0;
            RemainingTurns = config.MaxTurns;
            foreach (var o in config.Objectives)
            {
                ObjectiveTrackers.Add(new(o));
            }
            IsCleared = false;
        }

        public LevelRuntimeData(LevelRuntimeData other)
        {
            LevelConfig = other.LevelConfig;
            CurrentScore = 0;
            RemainingTurns = other.LevelConfig.MaxTurns;
        }

        public void TrackingObjective(List<Cell> matchPos, BoardModel board)
        {
            foreach (var pos in matchPos)
            {
                var block = board.GetBlock(pos);
                foreach (var ot in ObjectiveTrackers)
                {
                    if (block.Type == ot.Objective.Type) ot.CurrentAmount++;
                }
            }

            OnObjectiveUpdated?.Invoke();
            if (ObjectiveTrackers.All(ot => ot.CurrentAmount >= ot.Objective.Amount))
            {
                IsCleared = true;
                FinishLevel();
            }
        }

        public void AddScore(int points)
        {
            CurrentScore += points;
            OnScoreChanged?.Invoke();
        }

        public void ReduceTurn()
        {
            RemainingTurns--;
            OnTurnChanged?.Invoke();

            if (RemainingTurns <= 0)
            {
                FinishLevel();
            }
        }

        public void FinishLevel()
        {
            OnFinishedLevel?.Invoke();
        }
    }
}