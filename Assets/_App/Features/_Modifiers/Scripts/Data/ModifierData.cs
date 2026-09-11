using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    [CreateAssetMenu(fileName = "ModifierData", menuName = "DigitalLove/Game/ModifierData")]
    public class ModifierData : ScriptableObject
    {
        public string id;
        public ModifierBehaviour prefab;
        public ModifierPlacementMode placementMode = ModifierPlacementMode.PathCell;
        public ModifierScoreEffectKind scoreEffectKind = ModifierScoreEffectKind.Multiply;
        public float scoreEffectValue = 1.5f;
    }
}
