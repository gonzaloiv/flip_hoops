using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public abstract class BallThrowBehaviour : MonoBehaviour
    {
        public virtual bool ControlsFlight => false;

        public abstract void ApplyThrow(Rigidbody rb, Vector3 releaseDelta, float forceMultiplier);

        public virtual void SetTarget(Transform target) { }
    }
}
