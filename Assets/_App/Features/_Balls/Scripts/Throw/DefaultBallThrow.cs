using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class DefaultBallThrow : BallThrowBehaviour
    {
        [SerializeField] private float minThrowSpeed = 0.35f;
        [SerializeField] private float maxThrowSpeed = 12f;
        [SerializeField] private float spinMultiplier = 1f;

        public override void ApplyThrow(Rigidbody rb, BallThrowRelease release, float forceMultiplier)
        {
            Vector3 velocity = release.LinearVelocity * forceMultiplier;
            rb.linearVelocity = ClampThrowVelocity(velocity);
            rb.angularVelocity = release.AngularVelocity * spinMultiplier;
        }

        private Vector3 ClampThrowVelocity(Vector3 velocity)
        {
            float speed = velocity.magnitude;
            if (speed < minThrowSpeed)
                return Vector3.zero;
            if (speed > maxThrowSpeed)
                return velocity * (maxThrowSpeed / speed);
            return velocity;
        }
    }
}
