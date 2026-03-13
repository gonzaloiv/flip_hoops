using System.Collections.Generic;
using DigitalLove.Game.Balls;
using DigitalLove.Global;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalLove.Game.Basket
{
    public class BasketBehaviour : MonoBehaviour
    {
        [SerializeField] private int maxBallsInside = 3;
        [SerializeField] private LayerMask ballLayerMask;
        [SerializeField] private float radius;
        [SerializeField] private ParticleSystem ps;

        private List<BallBehaviour> ballsInside = new();

        public float Radius => radius;

        public UnityEvent<int> scored;

        private void OnTriggerEnter(Collider other)
        {
            if (ballLayerMask.Contains(other.gameObject.layer))
            {
                BallBehaviour ball = other.attachedRigidbody.GetComponent<BallBehaviour>();
                if (!ballsInside.Contains(ball))
                {
                    OnScored(ball.Score);
                    ballsInside.Add(ball);
                    RemoveBallIfNeeded();
                }
            }
        }

        private void RemoveBallIfNeeded()
        {
            if (ballsInside.Count > maxBallsInside)
            {
                BallBehaviour ballToDisable = ballsInside[0];
                ballsInside.Remove(ballToDisable);
                ballToDisable.SetActive(false);
            }
        }
        private void OnScored(int score)
        {
            // ps.Play();
            scored.Invoke(score);
        }

        [Button]
        public void InvokeScored() => OnScored(2);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}