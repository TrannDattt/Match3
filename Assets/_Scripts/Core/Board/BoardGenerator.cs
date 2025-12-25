using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Factories;
using Match3.Models;
using Match3.Views;
using UnityEditor;
using static Match3.Factories.BlockFactory;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Board
{
    public static class BoardGenerator
    {
        public static BlockModel GenerateBlock(BoardModel model, Cell pos)
        {
            BlockModel block = new(BlockFactory.Instance.GetBlockData(EBlockType.Random, EAbilityType.None)); 
            model.Set(pos, block);
            return block;
        }

        public static void GenerateBoard(BoardModel model)
        {
            List<Cell> positions = new(model.BlockDict.Keys);
            foreach (var pos in positions)
            {
                GenerateBlock(model, pos);
            }

            while (!CheckValidBoard(model)) 
            {
                BoardSwapper.Suffle(model);
            }
        }

        public static bool CheckValidBoard(BoardModel model)
        {
            bool checkNull = true;

            foreach(var pos in model.BlockDict.Keys)
            {
                if(checkNull && model.BlockDict[pos] != null) checkNull = false;
                if(BoardMatcher.HasMatchAt(model, pos, out var matchedHor, out var matchedVer)) return false;
            }

            if(checkNull) return false;
            if(!BoardMatcher.HasValidMove(model)) return false;

            return true;
        }
    }
}
