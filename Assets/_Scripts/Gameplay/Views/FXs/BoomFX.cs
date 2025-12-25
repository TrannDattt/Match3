// using Match3.Controls;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Match3.Models;
using UnityEngine;

namespace Match3.Views.FXs
{
    public class BoomFX : AbilityFX
    {
        [SerializeField] private SpriteRenderer _boomFX;

        private Boom _ability;

        public override void Set(BlockAbility ability)
        {
            if (ability is not Boom) return;

            _ability = ability as Boom;
        }

        public override IEnumerator Play(BoardView boardView, (int, int) pos, List<(int, int)> scored, float duration = 0.3F)
        {
            yield return base.Play(boardView, pos,scored, duration);
            
            transform.position = boardView.CellToWorld(new(pos.Item1, pos.Item2));
            var tileSize = boardView.GetTileSize();

            _boomFX.size = new(tileSize.x, tileSize.y);
            var targetSize = new Vector2(tileSize.x * _ability.Range, tileSize.y * _ability.Range);

            yield return DOTween.To(() => _boomFX.size,
                                    size => _boomFX.size = size,
                                    targetSize,
                                    duration).SetEase(Ease.OutQuad).WaitForCompletion();
        }
    }
}
