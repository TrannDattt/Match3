using UnityEngine;
using static Match3.Factories.BlockFactory;

namespace Match3.Datas
{
    [CreateAssetMenu(menuName = "SO/Block SO")]
    public class BlockConfigSO : ScriptableObject
    {
        public EBlockType Type;
        public Sprite Sprite;
        public EAbilityType AbilityType;
    }
}