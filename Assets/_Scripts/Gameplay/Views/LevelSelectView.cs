using System.Collections.Generic;
using Match3.Datas;
using Match3.GameSystem;
using Match3.Views.Components;
using UnityEngine;

namespace Match3.Views
{
    public class LevelSelectView : ViewBase
    {
        [SerializeField] private RectTransform _levelContainer;
        [SerializeField] private LevelButtonView _levelButtonPrefab;

        // private List<LevelButtonView> _levelButtons = new();

        public void Initialize()
        {
            foreach(Transform transform in _levelContainer)
            {
                if (transform.gameObject.TryGetComponent<LevelButtonView>(out var buttonView))
                    Destroy(buttonView.gameObject);
            }

            foreach(var kv in GameManager.Instance.LevelMap)
            {
                var buttonView = Instantiate(_levelButtonPrefab, _levelContainer);
                buttonView.Setup(kv.Key, kv.Value);
            }
        }

        void Start()
        {
            Initialize();
        }
    }
}
