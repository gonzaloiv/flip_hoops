using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Furniture
{
    [CreateAssetMenu(fileName = "FurnitureSeedData", menuName = "DigitalLove/Game/FurnitureSeedData")]
    public class FurnitureSeedData : ScriptableObject
    {
        [Range(0f, 1f)] public float activatePercent = 0.4f;
        [Range(0f, 1f)] public float boostAmongActivesPercent = 0.7f;
        [Range(0f, 1f)] public float bounceAmongActivesPercent = 0.5f;

        public ThrowScoreOpKind boostKind = ThrowScoreOpKind.Multiply;
        public float boostValue = 1.5f;
        public ThrowScoreOpKind reduceKind = ThrowScoreOpKind.Multiply;
        public float reduceValue = 0.75f;
    }
}
