using System.Collections.Generic;
using Match3.Models;
using UnityEngine;

namespace Match3.Board
{
    public static class BoardGravity
    {
        //TODO: Board is rectangle and no hole within
        public static void ApplyGravity(BoardModel model, out List<Queue<BlockModel>> newBlocks)
        {
            newBlocks = new();
            for (int x = model.Min.x; x < model.Max.x; x++)
                // for (int y = model.Min.y; y < model.Max.y;)
            {
                // Find the gap
                int y = model.Min.y;
                while (y < model.Max.y && model.BlockDict[(x, y)] != null) y++;

                // Calculate the gap and swap
                var gap = 0;
                while (y < model.Max.y)
                {
                    if (model.BlockDict[(x, y)] == null)
                    {
                        gap++;
                    }
                    else
                    {
                        BoardSwapper.Swap(model, (x, y), (x, y - gap));
                    }
                    y++;
                }

                // Fill gap
                Queue<BlockModel> newQueue = new();
                for (; gap > 0; gap--)
                {
                    var newBlock = BoardGenerator.GenerateBlock(model, (x, y - gap));
                    // newBlocks.Add(new(x, y - gap), newBlock);
                    newQueue.Enqueue(newBlock);
                }

                newBlocks.Add(newQueue);
            }
        }
    }
}
