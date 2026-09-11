using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class SlingshotBallThrow : BallThrowBehaviour
    {
        [SerializeField] private float minPullDistance = 0.06f;
        [SerializeField] private float maxPullDistance = 0.65f;
        [SerializeField] private float minSpeed = 3f;
        [SerializeField] private float maxSpeed = 13f;
        [SerializeField] private float maxSpinSpeed = 18f;
        [SerializeField] private LineRenderer band;
        [SerializeField] private SlingshotAimArrow aim;
        [SerializeField] private SlingshotOriginBillboard originBillboard;

        private Vector3 origin;
        private bool isPulling;

        public override void OnGrab(Vector3 origin)
        {
            this.origin = origin;
            isPulling = true;
            ShowVisuals();
        }

        public override void ApplyThrow(Rigidbody rb, BallThrowRelease release, float forceMultiplier)
        {
            isPulling = false;
            HideVisuals();
            rb.linearVelocity = LaunchVelocity(rb.position) * forceMultiplier;
            rb.angularVelocity = ClampSpin(release.AngularVelocity);
        }

        private void OnEnable()
        {
            isPulling = false;
            HideVisuals();
        }

        private void LateUpdate()
        {
            if (!isPulling)
                return;
            if (band != null)
            {
                band.SetPosition(0, origin);
                band.SetPosition(1, transform.position);
            }
            if (aim != null)
                aim.Draw(origin, transform.position);
        }

        private Vector3 LaunchVelocity(Vector3 position)
        {
            Vector3 toOrigin = origin - position;
            float pull = toOrigin.magnitude;
            if (pull < minPullDistance)
                return Vector3.zero;
            float t = Mathf.InverseLerp(minPullDistance, maxPullDistance, pull);
            t = Mathf.SmoothStep(0f, 1f, t);
            return toOrigin / pull * Mathf.Lerp(minSpeed, maxSpeed, t);
        }

        private Vector3 ClampSpin(Vector3 angularVelocity)
        {
            float spin = angularVelocity.magnitude;
            if (spin <= maxSpinSpeed || spin < 0.0001f)
                return angularVelocity;
            return angularVelocity * (maxSpinSpeed / spin);
        }

        private void ShowVisuals()
        {
            if (band != null)
            {
                band.positionCount = 2;
                band.enabled = true;
            }
            if (aim != null)
                aim.Show();
            if (originBillboard != null)
                originBillboard.Show(origin);
        }

        private void HideVisuals()
        {
            if (band != null)
                band.enabled = false;
            if (aim != null)
                aim.Hide();
            if (originBillboard != null)
                originBillboard.Hide();
        }
    }
}
