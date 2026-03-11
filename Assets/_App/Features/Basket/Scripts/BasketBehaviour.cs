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

        private List<GameObject> ballsInside = new();

        public UnityEvent scored;

        private void OnTriggerEnter(Collider other)
        {
            if (ballLayerMask.Contains(other.gameObject.layer))
            {
                scored.Invoke();
                ballsInside.Add(other.attachedRigidbody.gameObject);
                if (ballsInside.Count > maxBallsInside)
                    Destroy(ballsInside[0]);
            }
        }

        [Button]
        public void InvokeScored() => scored.Invoke();
    }
}