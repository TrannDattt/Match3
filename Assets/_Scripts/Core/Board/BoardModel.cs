using System.Collections.Generic;
using System.Linq;
using Match3.Models;
using UnityEngine;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Board
{
    public class BoardModel
    {
        public Vector3Int Min {get; private set;}
        public Vector3Int Max {get; private set;}
        public Dictionary<Cell, BlockModel> BlockDict {get; private set;} = new();

        public BoardModel(Vector3Int min, Vector3Int max, List<Vector3Int> tiledPos)
        {
            Min = min;
            Max = max;

            for(int x = Min.x; x < Max.x; x++)
                for(int y = Min.y; y < Max.y; y++)
                    if(tiledPos.Contains(new(x, y))) 
                        BlockDict[(x, y)] = null;
        }

        public void Set(Cell pos, BlockModel block) => BlockDict[pos] = block;
        
        public void SetAll(Dictionary<Cell, BlockModel> newDict) => BlockDict = newDict;

        public List<BlockModel> GetAllBlocks()
        {
            return BlockDict.Values.ToList();
        }

        public BlockModel GetBlock(Cell pos)
        {
            return BlockDict[pos];
        }

        public List<Cell> GetAllPositions()
        {
            return BlockDict.Keys.ToList();
        }

        public Cell GetPosition(BlockModel block)
        {
            return BlockDict.FirstOrDefault(d => d.Value == block).Key;
        }

        public void Remove(Cell pos)
        {
            BlockDict[pos] = null;
        }
        
        public void Clear() 
        {
            List<Cell> positions = new(BlockDict.Keys);
            foreach (var pos in positions)
            {
                Remove(pos);
            };
        }

        public bool InBounds(Cell pos) => BlockDict.ContainsKey(pos);
    }
}
