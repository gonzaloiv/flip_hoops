using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class DefaultBallThrow : BallThrowBehaviour
    {
        [SerializeField] private float minThrowSpeed = 0.35f;
        [SerializeField] private float maxThrowSpeed = 12f;
        [SerializeField] private float spinMultiplier = 1f;
        [SerializeField, Tooltip("0 = raw speed, 1 = fully remapped to a smooth curve (more consistent).")]
        private float consistency = 0.45f;
        [SerializeField] private float maxSpinSpeed = 18f;

        public override void ApplyThrow(Rigidbody rb, BallThrowRelease release, float forceMultiplier)
        {
            Vector3 velocity = release.LinearVelocity * forceMultiplier;
            rb.linearVelocity = ClampThrowVelocity(velocity);
            rb.angularVelocity = ClampSpin(release.AngularVelocity * spinMultiplier);
        }

        private Vector3 ClampThrowVelocity(Vector3 velocity)
        {
            float speed = velocity.magnitude;
            if (speed < minThrowSpeed)
                return Vector3.zero;

            float remapped = RemapSpeed(speed);
            float finalSpeed = Mathf.Lerp(speed, remapped, consistency);
            if (finalSpeed > maxThrowSpeed)
                finalSpeed = maxThrowSpeed;
            return velocity * (finalSpeed / speed);
        }

        private float RemapSpeed(float speed)
        {
            float t = Mathf.InverseLerp(minThrowSpeed, maxThrowSpeed, speed);
            // Ease mid-range so similar swings land closer together.
            t = Mathf.SmoothStep(0f, 1f, t);
            return Mathf.Lerp(minThrowSpeed, maxThrowSpeed, t);
        }

        private Vector3 ClampSpin(Vector3 angularVelocity)
        {
            float spin = angularVelocity.magnitude;
            if (spin <= maxSpinSpeed || spin < 0.0001f)
                return angularVelocity;
            return angularVelocity * (maxSpinSpeed / spin);
        }
    }
}
