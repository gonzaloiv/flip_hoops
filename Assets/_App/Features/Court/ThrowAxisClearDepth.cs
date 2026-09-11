using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public static class ThrowAxisClearDepth
    {
        public const float MinClearDepth = 0.85f;
        public const float DesignClearCap = 5.5f;

        private const float InwardEpsilon = 0.05f;
        private const float FloorProbeHeight = 2f;
        private const float MinMaxRay = 20f;

        private static readonly LabelFilter WallFilter = new(
            MRUKAnchor.SceneLabels.WALL_FACE
            | MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE
            | MRUKAnchor.SceneLabels.INNER_WALL_FACE);

        private static readonly LabelFilter FloorFilter = new(MRUKAnchor.SceneLabels.FLOOR);

        public static float EffectiveMaxRay(MRUKRoom room, float maxRayDistance)
        {
            Bounds bounds = room.GetRoomBounds();
            return Mathf.Max(MinMaxRay, maxRayDistance, bounds.size.magnitude + 1f);
        }

        public static bool TrySampleWall(MRUKRoom room, out Vector3 wallPoint, out Vector3 inward)
        {
            wallPoint = Vector3.zero;
            inward = Vector3.zero;
            if (!room.GenerateRandomPositionOnSurface(
                    MRUK.SurfaceType.VERTICAL,
                    0.1f,
                    WallFilter,
                    out wallPoint,
                    out Vector3 normal))
                return false;

            inward = normal;
            inward.y = 0f;
            if (inward.sqrMagnitude < 0.0001f)
                return false;

            inward.Normalize();
            return true;
        }

        public static bool TryMeasureClearDepth(
            MRUKRoom room,
            Vector3 wallPoint,
            Vector3 inward,
            float maxRayDistance,
            out float clearDepth)
        {
            clearDepth = 0f;
            Vector3 origin = wallPoint + inward * InwardEpsilon + Vector3.up * 0.2f;
            if (room.Raycast(new Ray(origin, inward), maxRayDistance, WallFilter, out RaycastHit hit))
            {
                if (hit.distance < MinClearDepth)
                    return false;
                clearDepth = hit.distance;
                return true;
            }

            return TryEstimateClearDepth(room, origin, inward, maxRayDistance, out clearDepth);
        }

        public static bool TrySnapToFloor(MRUKRoom room, Vector3 candidate, out Vector3 floorPoint)
        {
            floorPoint = Vector3.zero;
            Bounds bounds = room.GetRoomBounds();
            float top = Mathf.Max(candidate.y + FloorProbeHeight, bounds.max.y + 0.5f);
            Vector3 origin = new(candidate.x, top, candidate.z);
            float maxDist = top - bounds.min.y + 1.5f;
            if (room.Raycast(new Ray(origin, Vector3.down), maxDist, FloorFilter, out RaycastHit hit))
            {
                floorPoint = hit.point;
                return true;
            }

            if (TrySnapToFloorUnfiltered(room, origin, maxDist, out floorPoint))
                return true;

            floorPoint = new Vector3(candidate.x, bounds.min.y, candidate.z);
            return room.IsPositionInRoom(floorPoint, false);
        }

        private static bool TrySnapToFloorUnfiltered(
            MRUKRoom room,
            Vector3 origin,
            float maxDist,
            out Vector3 floorPoint)
        {
            floorPoint = Vector3.zero;
            if (!room.Raycast(new Ray(origin, Vector3.down), maxDist, out RaycastHit hit))
                return false;
            if (hit.normal.y < 0.5f)
                return false;

            floorPoint = hit.point;
            return true;
        }

        public static bool IsThrowClear(
            MRUKRoom room,
            Vector3 throwPosition,
            float clearanceRadius,
            LayerMask clearanceMask,
            bool softClearance)
        {
            if (!room.IsPositionInRoom(throwPosition, !softClearance))
                return false;
            if (!softClearance && room.IsPositionInSceneVolume(throwPosition, clearanceRadius * 0.5f))
                return false;

            Vector3 check = new(throwPosition.x, clearanceRadius * 2f, throwPosition.z);
            return !Physics.CheckSphere(check, clearanceRadius, clearanceMask);
        }

        private static bool TryEstimateClearDepth(
            MRUKRoom room,
            Vector3 origin,
            Vector3 inward,
            float maxRayDistance,
            out float clearDepth)
        {
            clearDepth = 0f;
            float estimated = DistanceToBoundsExit(origin, inward, room.GetRoomBounds());
            estimated = Mathf.Min(estimated, maxRayDistance);
            if (estimated < MinClearDepth)
                return false;

            clearDepth = estimated;
            return true;
        }

        private static float DistanceToBoundsExit(Vector3 origin, Vector3 direction, Bounds bounds)
        {
            float t = ExitT(origin.x, direction.x, bounds.min.x, bounds.max.x);
            t = Mathf.Min(t, ExitT(origin.y, direction.y, bounds.min.y, bounds.max.y));
            t = Mathf.Min(t, ExitT(origin.z, direction.z, bounds.min.z, bounds.max.z));
            return Mathf.Max(0f, t);
        }

        private static float ExitT(float origin, float dir, float min, float max)
        {
            if (Mathf.Abs(dir) < 0.0001f)
                return float.PositiveInfinity;
            float bound = dir > 0f ? max : min;
            float t = (bound - origin) / dir;
            return t > 0f ? t : 0f;
        }
    }
}
