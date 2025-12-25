using System.Collections;
using System.Collections.Generic;
using Match3.Models;
using Match3.Utils;
using Match3.Views;
using Match3.Views.FXs;
using UnityEngine;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Factories
{
    public class FXPlayer : Singleton<FXPlayer>
    {
        [Header("Ability FXs")]
        [SerializeField] private AxisSweepFX _axisSweepFXPrefab;
        [SerializeField] private BoomFX _boomFXPrefab;
        [SerializeField] private ClearInstanceFX _clearInstanceFXPrefab;

        // private 

        public IEnumerator PlayAbilityFX(BlockAbility ability, BoardView boardView, Cell pos, List<Cell> scored)
        {
            var fxPrefab = ability switch
            {
                AxisSweep => _axisSweepFXPrefab,
                Boom => _boomFXPrefab,
                ClearInstance => _clearInstanceFXPrefab,
                _ => (AbilityFX)null
            };

            if (fxPrefab == null) yield break;

            var fx = Instantiate(fxPrefab, boardView.FXContainer);
            fx.Set(ability);
            yield return fx.Play(boardView, pos, scored);
            Destroy(fx.gameObject, .5f);
        }
    }
}
