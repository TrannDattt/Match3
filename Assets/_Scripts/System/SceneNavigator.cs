using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Match3.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match3.GameSystem
{
    public class SceneNavigator : Singleton<SceneNavigator>
    {
        public enum ESceneName
        {
            Initial,
            MainMenu,
            LevelSelect,
            Ingame,
        }

        private Dictionary<ESceneName, string> _sceneMap = new()
        {
            { ESceneName.Initial, "InitScene" },
            { ESceneName.MainMenu, "MainMenu" },
            { ESceneName.LevelSelect, "LevelSelect" },
            { ESceneName.Ingame, "Ingame" },
        };

        public async Task LoadSceneAsync(ESceneName key, Action callback = null)
        {
            if (!_sceneMap.ContainsKey(key))
            {
                Debug.LogError($"Scene '{key}' is not registered in SceneNavigator.");
                return;
            }

            if (GetCurrentScene() == key)
            {
                return;
            }

            var loadTask = SceneManager.LoadSceneAsync(_sceneMap[key]);
            while (!loadTask.isDone)
            {
                await Task.Yield();
            }

            callback?.Invoke();
        }

        public ESceneName GetCurrentScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            foreach (var kvp in _sceneMap)
            {
                if (kvp.Value == activeScene.name)
                {
                    return kvp.Key;
                }
            }

            throw new Exception("Current scene is not registered in SceneNavigator.");
        }
    }
}