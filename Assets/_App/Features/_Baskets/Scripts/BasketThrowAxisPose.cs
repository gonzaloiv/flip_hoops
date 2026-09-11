using DigitalLove.Game.Court;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public static class BasketThrowAxisPose
    {
        private const int Tries = 32;
        private const float LateralJitter = 0.7f;

        public static bool TryPlaceAtDistance(
            GravityData gravity,
            Vector3 throwPosition,
            Vector3 forward,
            float desiredMeters,
            System.Func<Vector3, Vector3, bool> acceptPose)
        {
            if (!CanStart(gravity, forward, desiredMeters, out MRUKRoom room, out Vector3 flatForward))
                return false;

            for (int i = 0; i < Tries; i++)
            {
                if (!TryCandidate(room, gravity, throwPosition, flatForward, desiredMeters, i, out Vector3 position, out Vector3 normal))
                    continue;
                if (acceptPose(position, normal))
                    return true;
            }

            return false;
        }

        private static bool CanStart(
            GravityData gravity,
            Vector3 forward,
            float desiredMeters,
            out MRUKRoom room,
            out Vector3 flatForward)
        {
            room = null;
            flatForward = Vector3.zero;
            if (gravity == null || desiredMeters < 0.2f)
                return false;

            room = MRUK.Instance != null ? MRUK.Instance.GetCurrentRoom() : null;
            if (room == null)
                return false;

            flatForward = forward;
            flatForward.y = 0f;
            if (flatForward.sqrMagnitude < 0.0001f)
                return false;

            flatForward.Normalize();
            return true;
        }

        private static bool TryCandidate(
            MRUKRoom room,
            GravityData gravity,
            Vector3 throwPosition,
            Vector3 forward,
            float desiredMeters,
            int attempt,
            out Vector3 position,
            out Vector3 normal)
        {
            position = Vector3.zero;
            normal = Vector3.zero;
            if ((gravity.surfaceTypes & MRUK.SurfaceType.VERTICAL) != 0)
                return TrySnapToWall(room, gravity, throwPosition, forward, desiredMeters, out position, out normal);

            Vector3 right = Vector3.Cross(Vector3.up, forward);
            float lateral = attempt == 0 ? 0f : Random.Range(-LateralJitter, LateralJitter);
            Vector3 candidate = throwPosition + forward * desiredMeters + right * lateral;
            return TrySnapHorizontal(room, gravity, candidate, out position, out normal);
        }

        private static bool TrySnapHorizontal(
            MRUKRoom room,
            GravityData gravity,
            Vector3 candidate,
            out Vector3 position,
            out Vector3 normal)
        {
            position = Vector3.zero;
            normal = Vector3.zero;
            if ((gravity.surfaceTypes & MRUK.SurfaceType.FACING_UP) != 0)
            {
                if (!ThrowAxisClearDepth.TrySnapToFloor(room, candidate, out position))
                    return false;
                normal = Vector3.up;
                return true;
            }

            if ((gravity.surfaceTypes & MRUK.SurfaceType.FACING_DOWN) != 0)
                return TrySnapToCeiling(room, candidate, out position, out normal);

            return false;
        }

        private static bool TrySnapToWall(
            MRUKRoom room,
            GravityData gravity,
            Vector3 throwPosition,
            Vector3 forward,
            float desiredMeters,
            out Vector3 position,
            out Vector3 normal)
        {
            position = Vector3.zero;
            normal = Vector3.zero;
            Vector3 origin = throwPosition + Vector3.up * 1.1f;
            float maxDist = Mathf.Max(desiredMeters + 2f, 6f);
            LabelFilter filter = new(gravity.sceneLabels);
            if (!room.Raycast(new Ray(origin, forward), maxDist, filter, out RaycastHit hit))
                return false;

            position = hit.point;
            normal = hit.normal;
            return true;
        }

        private static bool TrySnapToCeiling(
            MRUKRoom room,
            Vector3 candidate,
            out Vector3 position,
            out Vector3 normal)
        {
            position = Vector3.zero;
            normal = Vector3.zero;
            Bounds bounds = room.GetRoomBounds();
            Vector3 origin = new(candidate.x, bounds.min.y + 0.1f, candidate.z);
            float maxDist = bounds.size.y + 2f;
            LabelFilter filter = new(MRUKAnchor.SceneLabels.CEILING);
            if (!room.Raycast(new Ray(origin, Vector3.up), maxDist, filter, out RaycastHit hit))
                return false;

            position = hit.point;
            normal = hit.normal;
            return true;
        }
    }
}
