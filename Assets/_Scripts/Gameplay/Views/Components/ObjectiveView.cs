// using Match3.Controls;
using Match3.Factories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Match3.Datas.Runtime.LevelRuntimeData;

namespace Match3.Views.Components
{
    public class ObjectiveView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void UpdateView(ObjectiveTracker tracker)
        {
            _icon.sprite = BlockFactory.Instance.GetBlockData(tracker.Objective.Type, BlockFactory.EAbilityType.None).Sprite;
            _amountText.text = $"{tracker.CurrentAmount} / {tracker.Objective.Amount}";
        }
    }
}
