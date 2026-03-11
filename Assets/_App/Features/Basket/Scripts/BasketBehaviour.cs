using System.Collections.Generic;
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

        private List<GameObject> ballsInside = new();

        public float Radius => radius;

        public UnityEvent scored;

        private void OnTriggerEnter(Collider other)
        {
            if (ballLayerMask.Contains(other.gameObject.layer))
            {
                GameObject ball = other.attachedRigidbody.gameObject;
                if (!ballsInside.Contains(ball))
                {
                    scored.Invoke();
                    ballsInside.Add(ball);
                    if (ballsInside.Count > maxBallsInside)
                        Destroy(ballsInside[0]);
                }
            }
        }

        [Button]
        public void InvokeScored() => scored.Invoke();

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}