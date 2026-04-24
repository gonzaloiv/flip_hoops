using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Modifiers;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Flow.RoundState
{
    public class EditorDebugTool : MonoBehaviour
    {
        [SerializeField] private BallsSpawner ballsSpawner;
        [SerializeField] private BasketBehaviour basketBehaviour;

        [Button]
        private void GrabBall()
        {
            ballsSpawner.Invoke_BallGrabbed();
        }

        [Button]
        private void ThrowBallForBasket()
        {
            ThrowBall(basketBehaviour.transform.position + basketBehaviour.transform.up);
        }

        [Button]
        private void ThrowBallForNonBasket()
        {
            ThrowBall(basketBehaviour.transform.position + basketBehaviour.transform.up + basketBehaviour.transform.right);
        }

        [Button]
        private void ThrowBallForMultiplierBasket()
        {
            BasketModifierBehaviour modifier = FindAnyObjectByType<BasketModifierBehaviour>();
            if (modifier != null)
                ThrowBall(modifier.Basket.transform.position + modifier.Basket.transform.up);
        }

        private void ThrowBall(Vector3 position)
        {
            BallBehaviour ball = ballsSpawner.ValidBall;
            if (ball == null)
                return;
            ball.Invoke_OnSelect();
            ball.transform.position = position;
            ball.Invoke_OnUnselect();
        }
    }
}