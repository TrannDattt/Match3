using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Controls;
using Match3.Datas;
using Match3.Datas.Runtime;
using Match3.Utils;
using Match3.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using Cell = System.ValueTuple<int, int>;

namespace Match3.GameSystem
{
    [DefaultExecutionOrder(-100)]
    public class GameManager : Singleton<GameManager>
    {
        public Dictionary<LevelConfigSO, LevelDataJSON> LevelMap {get; private set;} = new();
        private LevelRuntimeData _curLevelData;

        public UnityEvent OnGameInitialized {get; private set;} = new();

        public async Task CheckAndUpdateAddressables()
        {
            await Addressables.InitializeAsync().Task;

            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            await checkHandle.Task;

            if (checkHandle.Result.Count > 0)
            {
                var updateHandle =
                    Addressables.UpdateCatalogs(checkHandle.Result);

                await updateHandle.Task;
            }
        }

        public async Task FetchAllLevels()
        {
            LevelMap.Clear();
            var handle = Addressables.LoadAssetsAsync<LevelConfigSO>("Level");
            await handle.Task;

            var levelConfigs = handle.Result;
            var savedData = RuntimeDataSaveLoader.LoadAllLevelData();
            foreach (var config in levelConfigs)
            {
                savedData.TryGetValue(config.LevelID, out var lastRuntimeData);
                LevelMap.Add(config, lastRuntimeData);
            }

            LevelMap = LevelMap.OrderBy(kv => kv.Key.LevelNumber).ToDictionary(kv => kv.Key, kv => kv.Value);
            Debug.Log($"Fetched {LevelMap.Count} levels from Addressables.");
        }

        public LevelRuntimeData GetLevelByNumber(int levelNumber)
        {
            var kvp = LevelMap.FirstOrDefault(kv => kv.Key.LevelNumber == levelNumber);
            if (kvp.Key == null)
            {
                Debug.LogError($"Level number {levelNumber} not found!");
                return null;
            }

            return new LevelRuntimeData(kvp.Key);
        }

        public bool GetLevelLastClearStatus(int levelNumber)
        {
            var kvp = LevelMap.FirstOrDefault(kv => kv.Key.LevelNumber == levelNumber);
            if (kvp.Key == null)
            {
                Debug.LogError($"Level number {levelNumber} not found!");
                return false;
            }

            return kvp.Value != null && kvp.Value.IsCleared;
        }

        public void OnMove()
        {
            _curLevelData.ReduceTurn();
        }

        public void OnMatch(List<Cell> matchedCells)
        {
            var score = ScoreCalculator.CalculateScore(matchedCells);
            Debug.Log($"Matched {matchedCells.Count} cells, score: {score}");
            _curLevelData.AddScore(score);

            var boardModel = BoardControl.Instance.GetModel();
            _curLevelData.TrackingObjective(matchedCells, boardModel);
        }

        public void OnEnterLevel()
        {
            if (_curLevelData == null) return;

            BoardControl.Instance.OnMove.AddListener(OnMove);
            BoardControl.Instance.OnMatch.AddListener(OnMatch);
        }

        public void OnExitLevel()
        {
            if (_curLevelData == null) return;

            BoardControl.Instance.OnMove.RemoveListener(OnMove);
            BoardControl.Instance.OnMatch.RemoveListener(OnMatch);
        }

        public void InitGame(LevelRuntimeData levelData)
        {
            _curLevelData = levelData;
            BoardControl.Instance.InitBoard(_curLevelData);

            var ingameView = FindFirstObjectByType<IngameView>();
            ingameView.Init(_curLevelData);

            _curLevelData.OnFinishedLevel.AddListener(async () =>
            {
                if (_curLevelData.IsCleared)
                {
                    RuntimeDataSaveLoader.SaveLevelData(_curLevelData);
                    await FetchAllLevels();
                } 
            });
        }

        public async Task StartLevel(LevelRuntimeData levelData = null)
        {
            if (levelData == null)
            {
                var firstUncleared = LevelMap.FirstOrDefault(kv => kv.Value == null || !kv.Value.IsCleared);
                levelData = new LevelRuntimeData(firstUncleared.Key) ?? GetLevelByNumber(1);
            }

            await SceneNavigator.Instance.LoadSceneAsync(SceneNavigator.ESceneName.Ingame);

            InitGame(levelData);
            OnEnterLevel();
        }

        public async Task StartNextLevel()
        {
            var nextLevelKvp = LevelMap.FirstOrDefault(kv => kv.Key.LevelNumber == _curLevelData.LevelConfig.LevelNumber + 1);
            if (nextLevelKvp.Key == null)
            {
                Debug.Log("No more levels available. Returning to main menu.");
                await ReturnToMainMenu();
                return;
            }

            OnExitLevel();
            await StartLevel(new LevelRuntimeData(nextLevelKvp.Key));
        }

        public async Task RestartLevel()
        {
            OnExitLevel();
            await StartLevel(new LevelRuntimeData(_curLevelData));
        }

        public async Task ReturnToMainMenu()
        {
            if (SceneNavigator.Instance.GetCurrentScene() == SceneNavigator.ESceneName.Ingame)
            {
                OnExitLevel();
            }
            await SceneNavigator.Instance.LoadSceneAsync(SceneNavigator.ESceneName.MainMenu);
        }

        async void Start()
        {
            await CheckAndUpdateAddressables();
            AudioControl.Instance.PlayBgm();
            await FetchAllLevels();
            OnGameInitialized?.Invoke();
        }
    }

    public class TurnManager
    {
        public int TurnCount { get; private set; }

        public TurnManager()
        {
            TurnCount = 0;
        }
    }

    public static class ScoreCalculator
    {
        public static int CalculateScore(List<Cell> matchedCells)
        {
            return matchedCells.Count * 10;
        }
    }
}