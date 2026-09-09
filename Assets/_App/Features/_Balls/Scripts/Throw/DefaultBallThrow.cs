using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class DefaultBallThrow : BallThrowBehaviour
    {
        public override void ApplyThrow(Rigidbody rb, Vector3 releaseDelta, float forceMultiplier)
        {
            if (releaseDelta == Vector3.zero)
                return;

            rb.AddForce(releaseDelta * forceMultiplier, ForceMode.Impulse);
        }
    }
}
