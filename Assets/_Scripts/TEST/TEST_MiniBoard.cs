using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Match3.Board;
using Match3.Controls;
using Match3.Factories;
using Match3.Views;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Cell = System.ValueTuple<int, int>;

namespace Match3.TESTs
{
    public class TEST_MiniBoard : MonoBehaviour
    {
        [SerializeField] private RectTransform _rt;
        [SerializeField] private Image _miniBlockPrefab;

        private BoardModel Board => BoardControl.Instance.GetModel();
        
        Dictionary<Cell, Image> _blockMap = new();
        private Cell _highlightedPos;

        private void UpdateMiniBoard()
        {
            // Debug.Log("Update mini board");
            foreach(Transform rt in _rt)
            {
                Destroy(rt.gameObject);
            }

            var boardSize = _rt.rect.size;
            var blockSize = _miniBlockPrefab.GetComponent<RectTransform>().rect.size;
            Vector2 offset = new(Board.Min.x, Board.Min.y);
            Vector2 pos0 = Vector2.zero - boardSize / 2 + blockSize / 2;
            // Debug.Log($"2. First Block: {_boardView.Board.Blocks[0, 0]}");
            
            foreach (var pos in Board.BlockDict.Keys)
            {
                if (Board.BlockDict[pos] == null) continue;
                var type = Board.BlockDict[pos].Type;

                // Vector2 position;
                // if (pos.Item1 == Board.Min.x && pos.Item2 == Board.Min.y) position = pos0;
                // else position = new(pos0.x + (pos.Item1 - offset.x) * blockSize.x, pos0.y + (pos.Item2 - offset.y) * blockSize.y);
                Vector2 position = new(pos0.x + (pos.Item1 - offset.x) * blockSize.x, pos0.y + (pos.Item2 - offset.y) * blockSize.y);

                // Debug.Log($"Spawn block {type} with index ({x}, {y})");
                var miniBlock = Instantiate(_miniBlockPrefab, _rt);
                miniBlock.GetComponent<RectTransform>().anchoredPosition = position;
                miniBlock.sprite = BlockFactory.Instance.GetBlockData(type, BlockFactory.EAbilityType.None).Sprite;
                _blockMap[pos] = miniBlock;
            }
        }

        // private Image GetBlock(Cell pos)
        // {
        //     var boardSize = _rt.rect.size;
        //     var blockSize = _miniBlockPrefab.GetComponent<RectTransform>().rect.size;
        //     Vector2 offset = new(Board.Min.x, Board.Min.y);
        //     Vector2 pos0 = Vector2.zero - boardSize / 2 + blockSize / 2;

        //     Vector2 anchoredPosition = new(pos0.x + (pos.Item1 - offset.x) * blockSize.x, pos0.y + (pos.Item2 - offset.y) * blockSize.y);
        // }

        private IEnumerator SuggestMove(float delay)
        {
            yield return new WaitForSeconds(delay);

            var validMoves = BoardMatcher.GetValidMoves(Board);
            if (validMoves.Count == 0)
            {
                Debug.Log("No valid move");
                yield break;
            }
            
            Highlight(validMoves[0].Item1);
        }

        private void Highlight(Cell pos)
        {
            _blockMap[pos].color = Color.black;
        }

        private void UndoHighlight(Cell pos)
        {
            _blockMap[pos].color = Color.white;
        }

        void OnEnable()
        {
            BoardControl.Instance.OnMove.AddListener(() =>
            {
                UndoHighlight(_highlightedPos);
                UpdateMiniBoard();
                
                StopAllCoroutines();
                StartCoroutine(SuggestMove(5));
            });

            BoardControl.Instance.OnUpdateBoard.AddListener(() => UpdateMiniBoard());
        }

        // void Update()
        // {
        //     UpdateMiniBoard();
        // }
    }
}
