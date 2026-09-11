using DigitalLove.Game.Balls;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class CushionModifierBehaviour : ModifierBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody == null)
                return;

            if (collision.rigidbody.GetComponent<BallBehaviour>() == null)
                return;

            RegisterActivation();
        }
    }
}
