using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class ObstacleGridPose
    {
        private readonly float lateralHalfExtent;
        private readonly float heightHalfExtent;

        public ObstacleGridPose(float lateralHalfExtent, float heightHalfExtent)
        {
            this.lateralHalfExtent = lateralHalfExtent;
            this.heightHalfExtent = heightHalfExtent;
        }

        public Vector3 WorldPosition(Transform throwZone, Transform basket, Vector3Int cell)
        {
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
            Vector3 along = Vector3.Lerp(throwPos, basketPos, depthT);
            along.y = Mathf.Lerp(throwPos.y, basketPos.y, depthT);
            along += right * (cell.x * lateralHalfExtent);
            along += Vector3.up * (cell.y * heightHalfExtent);
            return along;
        }

        public Vector3Int MirrorLateral(Vector3Int cell)
        {
            return new Vector3Int(-cell.x, cell.y, cell.z);
        }
    }
}
