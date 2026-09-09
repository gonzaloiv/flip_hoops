using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public readonly struct BallThrowRelease
    {
        public readonly Vector3 LinearVelocity;
        public readonly Vector3 AngularVelocity;

        public BallThrowRelease(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            LinearVelocity = linearVelocity;
            AngularVelocity = angularVelocity;
        }
    }
}
