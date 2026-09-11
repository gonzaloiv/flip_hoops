using System;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    [Serializable]
    public class ModifierPlacement
    {
        public ModifierData modifier;
        public bool obligatory;

        [Tooltip("PathCell: lateral / height / depth each in {-1, 0, 1}. Ignored for RoomBoundary.")]
        public Vector3Int cell;
    }
}
