// using Match3.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Match3.Models;
using UnityEngine;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Views.FXs
{
    public class AxisSweepFX : AbilityFX
    {
        [SerializeField] private SpriteRenderer _leftFX;
        [SerializeField] private SpriteRenderer _rightFX;
        [SerializeField] private SpriteRenderer _upFX;
        [SerializeField] private SpriteRenderer _downFX;

        private AxisSweep _ability;

        public override void Set(BlockAbility ability)
        {
            if (ability is not AxisSweep) return;

            _ability = ability as AxisSweep;
            _leftFX.gameObject.SetActive(_ability.Horizontal);
            _rightFX.gameObject.SetActive(_ability.Horizontal);
            _upFX.gameObject.SetActive(_ability.Vertical);
            _downFX.gameObject.SetActive(_ability.Vertical);
        }

        public override IEnumerator Play(BoardView boardView, Cell pos, List<Cell> scored, float duration = 0.3f)
        {
            yield return base.Play(boardView, pos,scored, duration);

            transform.position = boardView.CellToWorld(new(pos.Item1, pos.Item2));
            var tileSize = boardView.GetTileSize();
            var bound = boardView.GetBounds();
            var sequence = DOTween.Sequence();

            if (_ability.Horizontal)
            {
                _leftFX.size = new(tileSize.x, _ability.Thickness * tileSize.y);
                sequence.Join(Expand(_leftFX, boardView, pos, (bound.min.x, pos.Item2), duration));

                _rightFX.size = new(tileSize.x, _ability.Thickness * tileSize.y);
                sequence.Join(Expand(_rightFX, boardView, pos, (bound.max.x - 1, pos.Item2), duration));
            }

            if (_ability.Vertical)
            {
                _upFX.size = new(tileSize.x, _ability.Thickness * tileSize.y);
                sequence.Join(Expand(_upFX, boardView, pos, (pos.Item1, bound.max.y - 1), duration));

                _downFX.size = new(tileSize.x, _ability.Thickness * tileSize.y);
                sequence.Join(Expand(_downFX, boardView, pos, (pos.Item1, bound.min.y), duration));
            }

            yield return sequence.WaitForCompletion();
        }

        private Tween Expand(SpriteRenderer renderer, BoardView boardView, Cell from, Cell to, float duration)
        {
            var tileSize = boardView.GetTileSize();
            var distance = Mathf.Abs(from.Item1 - to.Item1 + from.Item2 - to.Item2) * tileSize.x + tileSize.x / 2;
            // Debug.Log($"Scaling FX {renderer.name} from {from} to {to}, distance: {distance}");
            return DOTween.To(() => renderer.size,
                              x => renderer.size = x,
                              new Vector2(distance, renderer.size.y),
                              duration).SetEase(Ease.OutQuad);
        }
    }
}
