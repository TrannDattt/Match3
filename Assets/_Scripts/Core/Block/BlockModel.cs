using System.Threading.Tasks;
using Match3.Board;
using Match3.Datas;
using UnityEngine;
using static Match3.Factories.BlockFactory;

namespace Match3.Models
{
    public class BlockModel
    {
        private BlockConfigSO _data;
        // public BlockAbility Ability {get; private set;}
        
        public EBlockType Type => _data != null ? _data.Type : EBlockType.None;
        public EAbilityType AbilityType = EAbilityType.None;
        public Sprite Sprite => _data?.Sprite;

        public BlockModel(BlockConfigSO data)
        {
            _data = data;
            AbilityType = data.AbilityType;
        }

        public BlockModel(BlockModel model)
        {
            _data = model._data;
            AbilityType = model.AbilityType;
        }

        public void RemoveAbility()
        {
            AbilityType = EAbilityType.None;
        }

        public void Remove()
        {
            
        }
    }
}
