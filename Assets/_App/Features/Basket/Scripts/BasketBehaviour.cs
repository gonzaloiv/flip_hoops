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
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private Collider trigger;

        private List<BallBehaviour> ballsInside = new();

        [Header("Physics")]
        [SerializeField] private float radius;
        public float Radius => radius;

        [SerializeField] private float height = 1f;
        public float Height => height;

        [Header("UI")]
        [SerializeField] private GameObject lookHerePanel;
        [SerializeField] private Transform panelRef;
        public Transform PanelRef => panelRef;

        public Vector3 WorldPosition => transform.position;

        public UnityEvent<ScoredEventArgs> scored;

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
            ps.Play();
            scored.Invoke(new(score, panelRef));
        }

        [Button]
        public void InvokeScored() => OnScored(2);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public void SetTriggerActive(bool isActive)
        {
            trigger.enabled = isActive;
        }

        public void Show(Vector3 position, Vector3 upDirection)
        {
            transform.position = position;
            transform.up = upDirection;
            SetTriggerActive(false);
            gameObject.SetActive(true);
            lookHerePanel?.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}