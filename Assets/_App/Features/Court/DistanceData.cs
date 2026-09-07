using UnityEngine;

namespace DigitalLove.Game.Court
{
    [CreateAssetMenu(fileName = "DistanceData", menuName = "DigitalLove/Game/DistanceData")]
    public class DistanceData : ScriptableObject
    {
        [Range(0.5f, 2f)] public float[] minMax = new[] { 1.25f, 1.75f };
    }
}