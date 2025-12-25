using System.Collections.Generic;
using Match3.Models;
using UnityEngine;
using static Match3.Factories.BlockFactory;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Utils
{
    public static class SpecialAbilityMapping
    {
        public static EAbilityType GetAbilityType(int horCount, int verCount)
        {
            if (horCount + verCount > 6 || horCount > 4 || verCount > 4) return EAbilityType.ClearInstance;
            if (horCount + verCount == 6 && horCount == verCount) return EAbilityType.Boom;
            if (horCount == 4) return EAbilityType.Horizontal;
            if (verCount == 4) return EAbilityType.Vertical;
            return EAbilityType.None;
        }

        // public static BlockAbility GetCombinedAbility(BlockAbility ability1, BlockAbility ability2)
        // {
        //     return ability1 switch
        //     {
        //         AxisSweep => ability2 switch
        //         {
        //             AxisSweep => new AxisSweep(),
        //             _ => null
        //         },
        //         _ => null,
        //     };
        // }
    }
}
