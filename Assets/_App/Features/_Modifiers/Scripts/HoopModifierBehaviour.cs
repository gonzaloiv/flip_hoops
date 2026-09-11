using DigitalLove.Game.Balls;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class HoopModifierBehaviour : ModifierBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!BallBehaviour.TryGetFromRigidbody(other.attachedRigidbody, out _))
                return;

            RegisterActivation();
        }
    }
}
