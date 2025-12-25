using System.Collections.Generic;
using System.Linq;
using Match3.Models;
using UnityEngine;
using static Match3.Factories.BlockFactory;
using Cell = System.ValueTuple<int, int>;

namespace Match3.Board
{
    public static class BoardMatcher
    {
        public static bool HasMatchAt(BoardModel model, Cell pos, out List<Cell> matchedHor, out List<Cell> matchedVer)
        {
            return HasMatchAt(model, pos.Item1, pos.Item2, out matchedHor, out matchedVer);
        }

        public static bool HasMatchAt(BoardModel model, BlockModel block, out List<Cell> matchedHor, out List<Cell> matchedVer)
        {
            var pos = model.GetPosition(block);
            return HasMatchAt(model, pos.Item1, pos.Item2, out matchedHor, out matchedVer);
        }

        public static bool HasMatchAt(BoardModel model, int x, int y, out List<Cell> matchedHor, out List<Cell> matchedVer)
        {
            matchedHor = new(){ (x, y) };
            matchedVer = new(){ (x, y) };
            EBlockType type = model.BlockDict[(x, y)].Type;

            if (type == EBlockType.None)
                return false;

            for (int i = x - 1; i >= model.Min.x && model.BlockDict[(i, y)].Type == type; i--)
                matchedHor.Add((i, y));
            for (int i = x + 1; i < model.Max.x && model.BlockDict[(i, y)].Type == type; i++)
                matchedHor.Add((i, y));

            if (matchedHor.Count < 3) matchedHor = new(){ (x, y) };

            for (int i = y - 1; i >= model.Min.y && model.BlockDict[(x, i)].Type == type; i--)
                matchedVer.Add((x, i));
            for (int i = y + 1; i < model.Max.y && model.BlockDict[(x, i)].Type == type; i++)
                matchedVer.Add((x, i));

            if (matchedVer.Count < 3) matchedVer = new(){ (x, y) };

            return matchedHor.Count >= 3 || matchedVer.Count >= 3;
        }

        public static bool HasValidMove(BoardModel model)
        {
            for (int x = model.Min.x; x < model.Max.x; x++)
                for (int y = model.Min.y; y < model.Max.y; y++)
                {
                    Cell pos = (x, y);
                    if (!model.InBounds(pos) || model.BlockDict[pos].Type == EBlockType.None) continue;

                    if (model.BlockDict[pos].AbilityType == EAbilityType.ClearInstance)
                        return true;

                    if (CheckSwapMatch(model, pos, (x + 1, y)))
                        return true;

                    if (CheckSwapMatch(model, pos, (x, y + 1)))
                        return true;
                }

                return false;
            }

        public static List<(Cell, Cell)> GetValidMoves(BoardModel model)
        {
            List<(Cell, Cell)> validMoves = new();

            for (int x = model.Min.x; x < model.Max.x; x++)
                for (int y = model.Min.y; y < model.Max.y; y++)
                {
                    Cell pos = (x, y);
                    if (!model.InBounds(pos) || model.BlockDict[pos].Type == EBlockType.None) continue;

                    //TODO: Get adjacent type that has most blocks on board
                    if (model.BlockDict[pos].AbilityType == EAbilityType.ClearInstance)
                        validMoves.Add((pos, (pos.Item1 + 1, pos.Item2)));

                    if (CheckSwapMatch(model, pos, (pos.Item1 + 1, pos.Item2)))
                        validMoves.Add((pos, (pos.Item1 + 1, pos.Item2)));

                    if (CheckSwapMatch(model, pos, (pos.Item1, pos.Item2 + 1)))
                        validMoves.Add((pos, (pos.Item1, pos.Item2 + 1)));
                }

            return validMoves;
        }

        public static bool CheckSwapMatch(BoardModel model, Cell pos1, Cell pos2) => CheckSwapMatch(model, pos1.Item1, pos1.Item2, pos2.Item1, pos2.Item2);

        public static bool CheckSwapMatch(BoardModel model, int x1, int y1, int x2, int y2)
        {
            if (!model.InBounds((x2, y2)) || model.BlockDict[(x2, y2)].Type == EBlockType.None) 
                return false;
                
            if (model.BlockDict[(x1, y1)].AbilityType != EAbilityType.None
                && model.BlockDict[(x2, y2)].AbilityType != EAbilityType.None)
                return true;

            if (model.BlockDict[(x1, y1)].Type == model.BlockDict[(x2, y2)].Type)
                return false;

            BoardSwapper.Swap(model, (x1, y1), (x2, y2));

            bool hasMatch = HasMatchAt(model, x1, y1, out _, out _)
                            || HasMatchAt(model, x2, y2, out _, out _);

            BoardSwapper.Swap(model, (x1, y1), (x2, y2));

            return hasMatch;
        }

        public static bool HasMatch(BoardModel model, out List<(List<Cell>, List<Cell>)> allMatched)
        {
            bool hasMatch = false;
            allMatched = new();

            static bool SameList(List<Cell> a, List<Cell> b)
            {
                if (a.Count != b.Count) return false;
                return !a.Except(b).Any();
            }
            
            for (int x = model.Min.x; x < model.Max.x; x++)
                for (int y = model.Min.y; y < model.Max.y; y++)
                {
                    Cell pos = (x, y);
                    // if (visited.Contains(pos)) continue;
                    var curMatch = HasMatchAt(model, pos, out var matchedHor, out var matchedVer);
                    if (!curMatch) continue;

                    hasMatch = true;
                    // matchedHor = matchedHor.OrderBy(c => c.Item1).ToList();
                    // matchedVer = matchedVer.OrderBy(c => c.Item2).ToList();

                    int matchedIndex = allMatched.IndexOf(allMatched.FirstOrDefault(match => (SameList(match.Item2, matchedVer) && match.Item1.Count < 3) 
                                                                                        || (SameList(match.Item1, matchedHor) && match.Item2.Count < 3)));
                    if (matchedIndex < 0) 
                    {
                        allMatched.Add((matchedHor, matchedVer));
                    }
                    else allMatched[matchedIndex] = (matchedHor, matchedVer);
                }

            return hasMatch;
        }
    }
}
