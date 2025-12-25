using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Match3.Factories.BlockFactory;

namespace Match3.Datas
{
    [CreateAssetMenu(menuName = "SO/Level SO")]
    public class LevelConfigSO : ScriptableObject
    {
        public string LevelID;
        public int LevelNumber;
        public int MaxTurns;
        public int ScoreThreshold1;
        public int ScoreThreshold2;
        public int ScoreThreshold3;
        public List<LevelObjective> Objectives;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(LevelID))
            {
                LevelID = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
#endif
    }

    [Serializable]
    public class LevelObjective
    {
        public EBlockType Type;
        public int Amount;
    }
}