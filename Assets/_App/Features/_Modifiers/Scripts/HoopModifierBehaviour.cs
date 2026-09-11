using DigitalLove.Game.Balls;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class HoopModifierBehaviour : ModifierBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Rigidbody body = other.attachedRigidbody;
            if (body == null)
                return;

            if (body.GetComponent<BallBehaviour>() == null)
                return;

            RegisterActivation();
        }
    }
}
