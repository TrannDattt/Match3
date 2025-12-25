// using Match3.Controls;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Match3.Models;
using UnityEngine;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Views.FXs
{
    public class ClearInstanceFX : AbilityFX
    {
        [SerializeField] private SpriteRenderer _clearFX;

        private ClearInstance _ability;

        public override void Set(BlockAbility ability)
        {
            if (ability is not ClearInstance) return;

            _ability = ability as ClearInstance;
        }

        public override IEnumerator Play(BoardView boardView, (int, int) pos, List<(int, int)> scored, float duration = 0.3f)
        {
            yield return base.Play(boardView, pos,scored, duration);
            
            transform.position = boardView.CellToWorld(new(pos.Item1, pos.Item2));
            Debug.Log($"Matching {scored.Count} cells for ClearInstanceFX");

            var sequence = DOTween.Sequence();
            foreach (var cell in scored)
            {
                var fxInstance = Instantiate(_clearFX, transform);
                sequence.Join(Expand(fxInstance, boardView, pos, cell, duration));
            }

            _clearFX.gameObject.SetActive(false);
            yield return sequence.WaitForCompletion();
        }

        private Tween Expand(SpriteRenderer renderer, BoardView boardView, Cell from, Cell to, float duration)
        {
            var tileSize = boardView.GetTileSize();
            var distance = Vector3.Magnitude(new Vector2(from.Item1 - to.Item1, from.Item2 - to.Item2)) * tileSize.x;
            var angle = Mathf.Atan2(to.Item2 - from.Item2, to.Item1 - from.Item1) * Mathf.Rad2Deg;
            renderer.transform.rotation = Quaternion.Euler(0, 0, angle);
            Debug.Log($"Scaling FX {renderer.name} from {from} to {to}, distance: {distance}");
            return DOTween.To(() => new Vector2(distance, renderer.size.y),
                              x => renderer.size = x,
                              new Vector2(distance, renderer.size.y),
                              duration).SetEase(Ease.OutQuad);
        }
    }
}
