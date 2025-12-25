// using Match3.Controls;
using System.Collections.Generic;
using UnityEngine;
using static Match3.Datas.Runtime.LevelRuntimeData;

namespace Match3.Views.Components
{
    public class ObjectiveTrackerView : MonoBehaviour
    {
        [SerializeField] private RectTransform _objectiveParent;
        [SerializeField] private ObjectiveView _objectiveViewPrefab;

        private List<ObjectiveView> _objectiveViews = new();

        public void InitTracker(List<ObjectiveTracker> runtimeTrackers)
        {
            _objectiveViews.Clear();
            foreach (Transform child in _objectiveParent)
            {
                if (child.TryGetComponent<ObjectiveView>(out var objectiveView))
                    Destroy(objectiveView.gameObject);
            }

            foreach (var tracker in runtimeTrackers)
            {
                var objectiveView = Instantiate(_objectiveViewPrefab, _objectiveParent);
                objectiveView.UpdateView(tracker);
                _objectiveViews.Add(objectiveView);
            }
        }

        public void UpdateTracker(List<ObjectiveTracker> runtimeTrackers)
        {
            for (int i = 0; i < runtimeTrackers.Count; i++)
            {
                _objectiveViews[i].UpdateView(runtimeTrackers[i]);
            }
        }
    }
}
