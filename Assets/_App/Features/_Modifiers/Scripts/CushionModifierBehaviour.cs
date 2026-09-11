using DigitalLove.Game.Balls;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class CushionModifierBehaviour : ModifierBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (!BallBehaviour.TryGetFromRigidbody(collision.rigidbody, out _))
                return;

            RegisterActivation();
        }
    }
}
