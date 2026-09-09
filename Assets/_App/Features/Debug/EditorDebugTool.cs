using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Flow.RoundState
{
    public class EditorDebugTool : MonoBehaviour
    {
        [SerializeField] private BallsSpawner ballsSpawner;
        [SerializeField] private BasketSpawner basketSpawner;

        [Button]
        private void GrabBall()
        {
            ballsSpawner.Invoke_BallGrabbed();
        }

        [Button]
        private void ThrowBallForBasket()
        {
            ThrowBall(basketSpawner.Basket.transform.position + basketSpawner.Basket.transform.up);
        }

        [Button]
        private void ThrowBallForNonBasket()
        {
            ThrowBall(basketSpawner.Basket.transform.position + basketSpawner.Basket.transform.up + basketSpawner.Basket.transform.right);
        }

        [Button]
        private void ThrowBall()
        {
            BallBehaviour ball = ballsSpawner.ValidBall;
            if (ball == null)
                return;
            ball.Invoke_OnSelect();
            ball.Invoke_OnUnselect();
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