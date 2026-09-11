using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public abstract class BallThrowBehaviour : MonoBehaviour
    {
        public virtual bool ControlsFlight => false;

        public virtual void OnGrab(Vector3 origin) { }

        public abstract void ApplyThrow(Rigidbody rb, BallThrowRelease release, float forceMultiplier);

        public virtual void SetTarget(Transform target) { }
    }
}
