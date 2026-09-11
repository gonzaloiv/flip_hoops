using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class ModifierRoomBoundaryPose
    {
        private readonly float heightOffset;
        private readonly float maxRayDistance;
        private readonly float clearanceRadius;
        private readonly LayerMask occlusionMask;

        public ModifierRoomBoundaryPose(
            float heightOffset,
            float maxRayDistance,
            float clearanceRadius,
            LayerMask occlusionMask)
        {
            this.heightOffset = heightOffset;
            this.maxRayDistance = maxRayDistance;
            this.clearanceRadius = clearanceRadius;
            this.occlusionMask = occlusionMask;
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

            Vector3 throwPos = throwZone.position;
            Vector3 basketPos = basket.position;
            Vector3 forward = basketPos - throwPos;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = throwZone.forward;
            forward.Normalize();

            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            if (right.sqrMagnitude < 0.0001f)
                right = throwZone.right;

            float depthT = (cell.z + 1) / 2f;
            Vector3 origin = Vector3.Lerp(throwPos, basketPos, depthT);
            origin.y = Mathf.Lerp(throwPos.y, basketPos.y, depthT) + heightOffset;

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
