using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Datas;
using Match3.Models;
using Match3.Utils;
using Match3.Views;
using UnityEngine;

namespace Match3.Factories
{
    public class BlockFactory : Singleton<BlockFactory>
    {
        public enum EBlockType
        {
            None,
            Random,
            Apple,
            Banana,
            Blueberry,
            Grapes,
            Orange,
            Pear,
            Strawberry,
        }

        public enum EAbilityType
        {
            None,
            Vertical,
            Horizontal,
            Boom,
            ClearInstance,
        }

        [SerializeField] private IngameBlockView _ingameBlockPrefab;
        [SerializeField] private List<BlockConfigSO> _blockSettings;

        private Dictionary<EBlockType, BlockConfigSO> _blockDict = new();
        private Queue<IngameBlockView> _blockQueue = new();

        public EBlockType GetRandomType()
        {
            // Debug.Log($"Random type => {_blockSettings[UnityEngine.Random.Range(0, _blockSettings.Count)].Type}");
            return _blockSettings[UnityEngine.Random.Range(0, _blockSettings.Count)].Type;
        }

        public BlockConfigSO GetBlockData(EBlockType type, EAbilityType specialType)
        {
            type = type == EBlockType.Random ? GetRandomType() : type;
            // return _blockDict[type];
            var data = _blockSettings.FirstOrDefault(bs => bs.Type == type && bs.AbilityType == specialType);
            if (data == null) Debug.Log($"No data with type {type}, special {specialType} found");
            return data;
        }

        public IngameBlockView GetIngameBlock(BlockModel block, Transform parent = null)
        {
            if (block == null || block.Type == EBlockType.None) return null;
            
            if (_blockQueue.Count == 0)
            {
                var newBlock = Instantiate(_ingameBlockPrefab, parent);
                newBlock.gameObject.SetActive(false);
                _blockQueue.Enqueue(newBlock);
            }

            var blockView = _blockQueue.Dequeue();
            // var blockView = Instantiate(_ingameBlockPrefab, parent);
            blockView.UpdateView(block);
            blockView.gameObject.SetActive(true);
            return blockView;
        }

        public void ReturnIngameBlock(IngameBlockView block)
        {
            if (block == null) return;

            block.OnSelect.RemoveAllListeners();
            block.gameObject.SetActive(false);
            _blockQueue.Enqueue(block);
            // Destroy(block.gameObject);
        }

        protected override void Awake()
        {
            base.Awake();

            foreach (var bs in _blockSettings)
            {
                _blockDict[bs.Type] = bs;
            }
        }
    }
}
