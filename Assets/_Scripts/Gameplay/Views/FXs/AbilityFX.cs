// using Match3.Controls;
using System.Collections;
using System.Collections.Generic;
using Match3.Board;
using Match3.GameSystem;
using Match3.Models;
using Match3.Views.Components;
using UnityEngine;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Views.FXs
{
    public abstract class AbilityFX : MonoBehaviour
    {
        [SerializeField] private AudioClip _fxClip;

        public abstract void Set(BlockAbility ability);
        public virtual IEnumerator Play(BoardView boardView, Cell pos, List<Cell> scored, float duration = 0.3f)
        {
            if (_fxClip)
            {
                AudioControl.Instance.PlaySfx(_fxClip);
            }

            yield return null;
        }
    }
}
