using System;
using System.Collections.Generic;
using System.IO;
using Match3.Datas.Runtime;
using UnityEngine;

namespace Match3.GameSystem
{
    public static class RuntimeDataSaveLoader
    {
        private static readonly string _savePathPrefix = Application.persistentDataPath;

        public static void SaveLevelData(LevelRuntimeData levelRuntimeData)
        {
            var allData = LoadAllLevelData();
            var lastData = allData.TryGetValue(levelRuntimeData.LevelConfig.LevelID, out var data) ? data : null;
            allData[levelRuntimeData.LevelConfig.LevelID] = new(levelRuntimeData.LevelConfig.LevelID,
                                                                levelRuntimeData.IsCleared,
                                                                Mathf.Max(lastData?.BestScore ?? 0, levelRuntimeData.CurrentScore));

            var json = JsonUtility.ToJson(new LevelDataCollectionJSON(allData.Values));
            var path = Path.Join(_savePathPrefix, "/Levels.dat");
            File.WriteAllText(path, json);

            Debug.Log($"Save level {levelRuntimeData.LevelConfig.LevelNumber} clear status to path: {path}");
        }

        public static Dictionary<string, LevelDataJSON> LoadAllLevelData()
        {
            var result = new Dictionary<string, LevelDataJSON>();
            var path = Path.Join(_savePathPrefix, "/Levels.dat");

            if (!File.Exists(path))
            {
                return result;
            }

            var json = File.ReadAllText(path);
            var collection = JsonUtility.FromJson<LevelDataCollectionJSON>(json);
            foreach (var levelData in collection.Levels)
            {
                result[levelData.LevelID] = levelData;
            }

            return result;
        }
    }

    [Serializable]
    public class LevelDataCollectionJSON
    {
        public List<LevelDataJSON> Levels;

        public LevelDataCollectionJSON() { }

        public LevelDataCollectionJSON(IEnumerable<LevelDataJSON> levelJsonDatas)
        {
            Levels = new List<LevelDataJSON>();
            foreach (var jsonData in levelJsonDatas)
            {
                Levels.Add(jsonData);
            }
        }
    }

    [Serializable]
    public class LevelDataJSON
    {
        public string LevelID;
        public bool IsCleared;
        public int BestScore;

        public LevelDataJSON() { }

        public LevelDataJSON(string levelID, bool isCleared, int bestScore)
        {
            LevelID = levelID;
            IsCleared = isCleared;
            BestScore = isCleared ? bestScore : 0;
        }
    }
}