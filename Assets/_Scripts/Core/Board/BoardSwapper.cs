using System.Collections.Generic;
using System.Threading.Tasks;
using Match3.Models;
using UnityEngine;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Board
{
    public static class BoardSwapper
    {
        public static void Suffle(BoardModel model)
        {
            // Debug.Log("Suffle");
            List<Cell> availablePos = new(model.BlockDict.Keys);
            Dictionary<Cell, BlockModel> tempBoard = new();

            foreach(var pos in model.BlockDict.Keys)
            {
                var randomPos = availablePos[Random.Range(0, availablePos.Count)];

                tempBoard[pos] = model.BlockDict[randomPos];
                availablePos.Remove(randomPos);
            }

            model.SetAll(tempBoard);
            // return Task.CompletedTask;
        }

        public static void Swap(BoardModel model, Cell pos1, Cell pos2)
        {
            (model.BlockDict[pos2], model.BlockDict[pos1]) = (model.BlockDict[pos1], model.BlockDict[pos2]);
        }
    }
}
