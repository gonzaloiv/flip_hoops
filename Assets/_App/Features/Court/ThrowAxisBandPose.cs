using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public static class ThrowAxisBandPose
    {
        private const int MaxPlaceAttempts = 120;
        private const float MinDesired = 0.7f;
        private const float OppositeMargin = 0.25f;

        private struct PlaceArgs
        {
            public MRUKRoom Room;
            public DistanceData Band;
            public float LowPercent;
            public float HighPercent;
            public float MaxRay;
            public float ClearanceRadius;
            public bool Soft;
        }

        public static bool TryPlaceForBand(
            DistanceData band,
            float maxRayDistance,
            float clearanceRadius,
            LayerMask clearanceMask,
            out Vector3 throwPosition,
            out Vector3 forward,
            out float scaleFactor,
            out float desiredMeters)
        {
            ResetOut(out throwPosition, out forward, out scaleFactor);
            desiredMeters = 0f;
            if (!CanStart(band, maxRayDistance, out float low, out float high, out MRUKRoom room))
                return false;

            PlaceArgs args = new()
            {
                Room = room,
                Band = band,
                LowPercent = low,
                HighPercent = high,
                MaxRay = ThrowAxisClearDepth.EffectiveMaxRay(room, maxRayDistance),
                ClearanceRadius = clearanceRadius
            };
            return TryPlaceLoop(
                args, clearanceMask, out throwPosition, out forward, out scaleFactor, out desiredMeters);
        }

        private static bool CanStart(
            DistanceData band,
            float maxRayDistance,
            out float lowPercent,
            out float highPercent,
            out MRUKRoom room)
        {
            lowPercent = 0f;
            highPercent = 0f;
            room = null;
            if (band == null || maxRayDistance <= 0.01f)
                return false;
            if (!band.TryGetPercentRange(out lowPercent, out highPercent))
                return false;

            room = MRUK.Instance != null ? MRUK.Instance.GetCurrentRoom() : null;
            return room != null;
        }

        private static void ResetOut(out Vector3 throwPosition, out Vector3 forward, out float scaleFactor)
        {
            throwPosition = Vector3.zero;
            forward = Vector3.zero;
            scaleFactor = 1f;
        }

        private static bool TryPlaceLoop(
            PlaceArgs args,
            LayerMask clearanceMask,
            out Vector3 throwPosition,
            out Vector3 forward,
            out float scaleFactor,
            out float desiredMeters)
        {
            ResetOut(out throwPosition, out forward, out scaleFactor);
            desiredMeters = 0f;
            for (int i = 0; i < MaxPlaceAttempts; i++)
            {
                args.Soft = i >= MaxPlaceAttempts / 2;
                if (TryPlaceOnce(
                        args, clearanceMask, out throwPosition, out forward, out scaleFactor, out desiredMeters))
                    return true;
            }

            return false;
        }

        private static bool TryPlaceOnce(
            PlaceArgs args,
            LayerMask clearanceMask,
            out Vector3 throwPosition,
            out Vector3 forward,
            out float scaleFactor,
            out float desiredMeters)
        {
            ResetOut(out throwPosition, out forward, out scaleFactor);
            desiredMeters = 0f;
            if (!ThrowAxisClearDepth.TrySampleWall(args.Room, out Vector3 wallPoint, out Vector3 inward))
                return false;

            return TryPlaceWithInward(
                       args, wallPoint, inward, clearanceMask,
                       out throwPosition, out forward, out scaleFactor, out desiredMeters)
                || TryPlaceWithInward(
                       args, wallPoint, -inward, clearanceMask,
                       out throwPosition, out forward, out scaleFactor, out desiredMeters);
        }

        private static bool TryPlaceWithInward(
            PlaceArgs args,
            Vector3 wallPoint,
            Vector3 inward,
            LayerMask clearanceMask,
            out Vector3 throwPosition,
            out Vector3 forward,
            out float scaleFactor,
            out float desiredMeters)
        {
            ResetOut(out throwPosition, out forward, out scaleFactor);
            desiredMeters = 0f;
            if (!ThrowAxisClearDepth.TryMeasureClearDepth(
                    args.Room, wallPoint, inward, args.MaxRay, out float clearDepth))
                return false;

            float backMargin = Mathf.Max(OppositeMargin, args.ClearanceRadius);
            float desired = ClampDesired(clearDepth, args.LowPercent, args.HighPercent, backMargin);
            float radius = args.Soft ? args.ClearanceRadius * 0.65f : args.ClearanceRadius;
            if (!TryBuildThrowPose(
                    args.Room, wallPoint, inward, backMargin, radius, clearanceMask, args.Soft,
                    out throwPosition, out forward))
                return false;

            desiredMeters = desired;
            scaleFactor = args.Band.ScaleForMeters(desired);
            return true;
        }

        private static float ClampDesired(
            float clearDepth,
            float lowPercent,
            float highPercent,
            float backMargin)
        {
            float usable = Mathf.Min(clearDepth, ThrowAxisClearDepth.DesignClearCap);
            float desired = usable * ((lowPercent + highPercent) * 0.5f);
            float maxDesired = clearDepth - backMargin - OppositeMargin;
            if (maxDesired < MinDesired)
                maxDesired = clearDepth * 0.55f;

            return Mathf.Clamp(desired, MinDesired, Mathf.Max(MinDesired, maxDesired));
        }

        private static bool TryBuildThrowPose(
            MRUKRoom room,
            Vector3 wallPoint,
            Vector3 inward,
            float backMargin,
            float clearanceRadius,
            LayerMask clearanceMask,
            bool soft,
            out Vector3 throwPosition,
            out Vector3 forward)
        {
            throwPosition = Vector3.zero;
            forward = inward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                return false;

            forward.Normalize();
            Vector3 candidate = wallPoint + inward * backMargin;
            if (!ThrowAxisClearDepth.TrySnapToFloor(room, candidate, out throwPosition))
                return false;
            return ThrowAxisClearDepth.IsThrowClear(room, throwPosition, clearanceRadius, clearanceMask, soft);
        }
    }
}
