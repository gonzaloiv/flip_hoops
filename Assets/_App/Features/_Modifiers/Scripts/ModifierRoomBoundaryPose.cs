using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class ModifierRoomBoundaryPose
    {
        private readonly float heightOffset;
        private readonly float maxRayDistance;
        private readonly float clearanceRadius;
        private readonly LayerMask occlusionMask;
        private readonly ThrowPathCellPose pathPose;

        public ModifierRoomBoundaryPose(
            float heightOffset,
            float maxRayDistance,
            float clearanceRadius,
            LayerMask occlusionMask,
            float lateralHalfExtent,
            float heightHalfExtent)
        {
            this.heightOffset = heightOffset;
            this.maxRayDistance = maxRayDistance;
            this.clearanceRadius = clearanceRadius;
            this.occlusionMask = occlusionMask;
            pathPose = new ThrowPathCellPose(lateralHalfExtent, heightHalfExtent);
        }

        public bool TryResolve(
            Transform throwZone,
            Transform basket,
            Vector3Int cell,
            out Vector3 position,
            out Quaternion rotation)
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;

            pathPose.GetAxes(throwZone, basket, out _, out Vector3 right);
            float depthT = (cell.z + 1) / 2f;
            Vector3 origin = Vector3.Lerp(throwZone.position, basket.position, depthT);
            origin.y = Mathf.Lerp(throwZone.position.y, basket.position.y, depthT) + heightOffset;

            float sideSign = cell.x >= 0 ? 1f : -1f;
            Vector3 direction = right * sideSign;
            if (!Physics.Raycast(origin, direction, out RaycastHit hit, maxRayDistance, occlusionMask))
                return false;

            position = hit.point + hit.normal * clearanceRadius;
            if (Physics.CheckSphere(position, clearanceRadius, occlusionMask))
                return false;

            Vector3 face = -hit.normal;
            face.y = 0f;
            if (face.sqrMagnitude > 0.0001f)
                rotation = Quaternion.LookRotation(face.normalized, Vector3.up);

            return true;
        }
    }
}
