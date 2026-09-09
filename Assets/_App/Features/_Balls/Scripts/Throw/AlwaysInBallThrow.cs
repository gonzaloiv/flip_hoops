using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class AlwaysInBallThrow : BallThrowBehaviour
    {
        [SerializeField] private float entryOffset = 0.175f;
        [SerializeField] private float minSpeed = 4f;

        private Transform target;
        private Rigidbody rb;
        private BallBehaviour ball;
        private bool isFlying;

        public override bool ControlsFlight => true;

        private BallBehaviour Ball => ball ??= GetComponent<BallBehaviour>();

        private void OnEnable()
        {
            isFlying = false;
            rb = null;
        }

        public override void SetTarget(Transform target) => this.target = target;

        public override void ApplyThrow(Rigidbody rb, BallThrowRelease release, float forceMultiplier)
        {
            this.rb = rb;
            isFlying = target != null;
            if (!isFlying)
                return;

            float speed = Mathf.Max(release.LinearVelocity.magnitude * forceMultiplier, minSpeed);
            rb.linearVelocity = DirectionToAim() * speed;
            rb.angularVelocity = release.AngularVelocity;
        }

        private void FixedUpdate()
        {
            if (!ShouldSteer())
                return;

            float speed = Mathf.Max(rb.linearVelocity.magnitude, minSpeed);
            rb.linearVelocity = DirectionToAim() * speed;
        }

        private bool ShouldSteer()
        {
            if (!isFlying || rb == null || Ball.HasScored)
            {
                isFlying = false;
                return false;
            }

            return true;
        }

        private Vector3 DirectionToAim()
        {
            Vector3 aim = target.position + target.up * entryOffset;
            return (aim - rb.position).normalized;
        }
    }
}