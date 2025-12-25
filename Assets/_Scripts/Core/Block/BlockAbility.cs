using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Board;
using Match3.Controls;
using Match3.Factories;
using static Match3.Factories.BlockFactory;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Models
{
    public abstract class BlockAbility
    {
        public BlockModel BaseBlock {get;}

        public BlockAbility(BlockModel baseBlock)
        {
            BaseBlock = baseBlock;
        }

        public virtual void Do(BoardModel board, Cell curPos, out List<Cell> scored)
        {
            scored = new();
        }

        public abstract BlockAbility Combine(BlockAbility toCombine);
    }

    // public class NoAbility : BlockAbility
    // {
    //     public NoAbility(BlockModel baseBlock) : base(baseBlock) {}

    //     public override BlockAbility Combine(BlockAbility toCombine)
    //     {
    //         return toCombine switch
    //         {
    //             ClearInstance => new ClearInstance(BaseBlock),
    //             _ => null
    //         };
    //     }
    // }
    
    public class AxisSweep : BlockAbility
    {
        public bool Horizontal {get;}
        public bool Vertical {get;}
        public int Thickness {get;}

        public AxisSweep(BlockModel baseBlock, bool horizontal, bool vertical, int thickness = 1) : base(baseBlock)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            Thickness = thickness;
        }

        public override void Do(BoardModel board, Cell curPos, out List<Cell> scored)
        {
            base.Do(board, curPos, out scored);

            // TODO: This will cause error if Thickness is even
            Cell min = (curPos.Item1 - Thickness / 2, curPos.Item2 - Thickness / 2);
            Cell max = (curPos.Item1 + Thickness / 2, curPos.Item2 + Thickness / 2);

            var positions = board.GetAllPositions();
            foreach (var pos in positions)
            {
                if (Horizontal && pos.Item2 >= min.Item2 && pos.Item2 <= max.Item2) scored.Add(pos);
                if (Vertical && pos.Item1 >= min.Item1 && pos.Item1 <= max.Item1) scored.Add(pos);
            }
        }

        public override BlockAbility Combine(BlockAbility toCombine)
        {
            return toCombine switch
            {
                AxisSweep => new AxisSweep(BaseBlock, true, true),
                Boom boom => new AxisSweep(BaseBlock, Horizontal, Vertical, boom.Range),
                ClearInstance clearInstance => new ClearInstance(clearInstance.BaseBlock, BaseBlock),
                _ => null
            };
        }
    }

    public class Boom : BlockAbility
    {
        public int Range {get;}

        public Boom(BlockModel baseBlock, int range = 3) : base(baseBlock)
        {
            Range = range;
        }

        public override void Do(BoardModel board, Cell curPos, out List<Cell> scored)
        {
            base.Do(board, curPos, out scored);

            float halfRange = (float)Range / 2;
            for (int x = (int)-halfRange; x < halfRange; x++)
                for (int y = (int)-halfRange; y < halfRange; y++)
                {
                    var pos = (curPos.Item1 + x, curPos.Item2 + y);
                    if (board.InBounds(pos)) scored.Add(pos);
                }
        }

        public override BlockAbility Combine(BlockAbility toCombine)
        {
            return toCombine switch
            {
                AxisSweep axisSweep => new AxisSweep(BaseBlock, axisSweep.Horizontal, axisSweep.Vertical, Range),
                Boom boom => new Boom(BaseBlock, boom.Range + 2),
                ClearInstance clearInstance => new ClearInstance(clearInstance.BaseBlock, BaseBlock),
                _ => null
            };
        }
    }

    public class ClearInstance : BlockAbility
    {
        public BlockModel ToClear {get;}
        private bool _clearAll;

        public ClearInstance(BlockModel baseBlock, BlockModel toClear = null, bool clearAll = false) : base(baseBlock)
        {
            ToClear = toClear ?? new(BlockFactory.Instance.GetBlockData(EBlockType.Random, EAbilityType.None));
            _clearAll = clearAll;
        }

        public override void Do(BoardModel board, Cell curPos, out List<Cell> scored)
        {
            base.Do(board, curPos, out scored);

            var positions = board.GetAllPositions();
            if (_clearAll)
            {
                scored = positions;
                return;
            }

            scored = positions.Where(p => board.GetBlock(p)?.Type == ToClear.Type).ToList();
            if (!scored.Contains(curPos)) scored.Add(curPos);

            if (ToClear.AbilityType != EAbilityType.None)
            {
                foreach (var pos in scored)
                {
                    BoardControl.Instance.SetBlock(pos, new(ToClear));
                }
            }
        }

        public override BlockAbility Combine(BlockAbility toCombine)
        {
            return toCombine switch
            {
                AxisSweep axisSweep => new ClearInstance(BaseBlock, axisSweep.BaseBlock),
                Boom boom => new ClearInstance(BaseBlock, boom.BaseBlock),
                ClearInstance => new ClearInstance(BaseBlock, clearAll: true),
                _ => new ClearInstance(BaseBlock)
            };
        }
    }
}
