using UnityEngine;

namespace DigitalLove.Game.Court
{
    [CreateAssetMenu(fileName = "DistanceData", menuName = "DigitalLove/Game/DistanceData")]
    public class DistanceData : ScriptableObject
    {
        [Tooltip("Min/max fraction of clear throw-axis depth (0–1).")]
        [Range(0.05f, 1f)]
        public float[] minMax = new[] { 0.55f, 0.70f };

        [Tooltip("Resolved mid meters that map to hoop scale factor 1.")]
        public float referenceMidMeters = 1.5f;

        public bool TryGetPercentRange(out float lowPercent, out float highPercent)
        {
            lowPercent = 0f;
            highPercent = 0f;
            if (minMax == null || minMax.Length < 2)
                return false;

            lowPercent = Mathf.Clamp01(Mathf.Min(minMax[0], minMax[1]));
            highPercent = Mathf.Clamp01(Mathf.Max(minMax[0], minMax[1]));
            return highPercent > lowPercent;
        }

        public float ScaleForMeters(float meters)
        {
            float reference = referenceMidMeters > 0.01f ? referenceMidMeters : 1.5f;
            return Mathf.Clamp(meters / reference, 0.55f, 1.35f);
        }
    }
}
