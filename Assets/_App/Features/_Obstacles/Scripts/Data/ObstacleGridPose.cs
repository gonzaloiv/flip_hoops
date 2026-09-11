using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class ObstacleGridPose
    {
        private readonly ThrowPathCellPose pathPose;

        public ObstacleGridPose(float lateralHalfExtent, float heightHalfExtent)
        {
            pathPose = new ThrowPathCellPose(lateralHalfExtent, heightHalfExtent);
        }

        public Vector3 WorldPosition(Transform throwZone, Transform basket, Vector3Int cell) =>
            pathPose.WorldPosition(throwZone, basket, cell);

        public Vector3Int MirrorLateral(Vector3Int cell) => pathPose.MirrorLateral(cell);
    }
}
