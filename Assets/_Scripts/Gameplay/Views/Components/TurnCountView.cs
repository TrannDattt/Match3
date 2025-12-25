// using Match3.Controls;
using TMPro;
using UnityEngine;

namespace Match3.Views.Components
{
    public class TurnCountView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _turnCountText;

        public void UpdateTurn(int turnCount)
        {
            _turnCountText.text = $"{turnCount}";
        }

        // void Start()
        // {
        //     UpdateTurn(0);
        // }
    }
}
