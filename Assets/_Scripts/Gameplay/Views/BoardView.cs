using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Controls;
using Match3.Factories;
using Match3.Models;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match3.Views
{
    public class BoardView : MonoBehaviour
    {
        [Header("Initialize")]
        [SerializeField] private Tilemap _tilemap;

        [Header("Attribute")]
        [field: SerializeField] public Transform Blocks {get; private set;}
        [field: SerializeField] public Transform FXContainer {get; private set;}
        // private Dictionary<Vector3Int, IngameBlockView> _blockDict = new();

        #region Utility
        public BoundsInt GetBounds() => _tilemap.cellBounds;

        public Vector3 GetTileSize() => _tilemap.layoutGrid.cellSize;

        // public void Set(Vector3Int pos, IngameBlockView blockView) => _blockDict[pos] = blockView;

        public List<Vector3Int> GetTiledPositions()
        {
            List<Vector3Int> allPos = new();
            foreach(var pos in _tilemap.cellBounds.allPositionsWithin)
            {
                if (!_tilemap.HasTile(pos)) continue;
                allPos.Add(pos);
            }
            return allPos.OrderBy(p => p.x).ThenBy(p => p.y).ToList();
        }

        public IngameBlockView GetBlock(Vector3Int position)
        {
            var worldPos = CellToWorld(position);
            var hit = Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Block"));

            if (hit != null && hit.gameObject.TryGetComponent<IngameBlockView>(out var blockView)) return blockView;
            else return null;
            // if (!_blockDict.ContainsKey(position)) return null;
            // else return _blockDict[position];
        }

        public List<IngameBlockView> GetAllBlocks()
        {
            List<IngameBlockView> blocks = new();
            foreach(Transform transform in Blocks)
            {
                if (transform.gameObject.TryGetComponent<IngameBlockView>(out var block))
                    blocks.Add(block);
            }

            return blocks;
            // return _blockDict.Values.ToList();
        }

        public void Set(Vector3Int pos, BlockModel toSet)
        {
            var blockView = GetBlock(pos);
            if (blockView != null) 
            {
                blockView.UpdateView(toSet);
                return;
            }

            BoardControl.Instance.SpawnBlockView(toSet, (pos.x, pos.y));
        }

        public void Remove(Vector3Int pos)
        {
            // Debug.Log($"Remove block view at {pos}");
            BlockFactory.Instance.ReturnIngameBlock(GetBlock(pos));
            // var pos = WorldToCell(blockView.transform.position);
            // _blockDict.Remove(pos);
        }

        public bool HasTile(int cellX, int cellY)
        {
            return _tilemap.HasTile(new(cellX, cellY));
        }

        public Vector3Int WorldToCell(Vector3 position)
        {
            return _tilemap.WorldToCell(position);
        }

        public Vector3 CellToWorld(Vector3Int position)
        {
            return _tilemap.GetCellCenterWorld(position);
        }

        public bool IsAdjacent(IngameBlockView block1, IngameBlockView block2)
        {
            var distance = WorldToCell(block2.transform.position) - WorldToCell(block1.transform.position);
            return Mathf.Abs(distance.x) + Mathf.Abs(distance.y) + Mathf.Abs(distance.z) == 1;
        }
        #endregion

        #region Board Manage
        public void Clear()
        {
            // _blockDict.Clear();
            var blockViews = GetAllBlocks();
            blockViews.ForEach(b => BlockFactory.Instance.ReturnIngameBlock(b));
        }
        #endregion

        #region TEST
        void Awake()
        {
            _tilemap.CompressBounds();
        }
        #endregion
    }
}
