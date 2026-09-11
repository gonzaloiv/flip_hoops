using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public enum ModifierPlacementMode
    {
        PathCell = 0,
        RoomBoundary = 1
    }

    [CreateAssetMenu(fileName = "ModifierData", menuName = "DigitalLove/Game/ModifierData")]
    public class ModifierData : ScriptableObject
    {
        public string id;
        public ModifierBehaviour prefab;
        public ModifierPlacementMode placementMode = ModifierPlacementMode.PathCell;
        public ThrowScoreOpKind scoreEffectKind = ThrowScoreOpKind.Multiply;
        public float scoreEffectValue = 1.5f;
    }
}
