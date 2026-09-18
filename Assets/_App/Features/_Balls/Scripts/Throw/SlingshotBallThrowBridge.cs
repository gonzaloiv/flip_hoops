using UnityEngine;
using DigitalLove.Interactables.OculusInteractionSDK;

namespace DigitalLove.Game.Balls
{
    public class SlingshotBallThrowBridge : BallThrowBehaviour
    {
        [SerializeField] private SlingshotBallThrow slingshot;

        private void Awake()
        {
            slingshot ??= GetComponent<SlingshotBallThrow>();
        }

        public override void OnGrab(Vector3 origin)
        {
            slingshot.OnGrab(origin);
        }

        public override void ApplyThrow(Rigidbody rb, BallThrowRelease release, float forceMultiplier)
        {
            slingshot.ApplyThrow(rb, release.AngularVelocity, forceMultiplier);
        }
    }
}
