using System.Collections.Generic;
using DigitalLove.Game.Balls;
using DigitalLove.Global;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalLove.Game.Basket
{
    public class BasketBehaviour : MonoBehaviour
    {
        [SerializeField] private LayerMask ballLayerMask;
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private Collider trigger;

        [Header("Physics")]
        [SerializeField] private float radius;
        [SerializeField] private float height = 1f;

        [Header("Capacity")]
        [SerializeField] private float binRadius = 0.1f;
        [SerializeField] private float binHeight = 0.12f;
        [SerializeField, Range(0.05f, 1f)] private float packingFactor = 0.5f;

        [Header("UI")]
        [SerializeField] private GameObject lookHerePanel;
        [SerializeField] private Transform panelRef;

        private List<BallBehaviour> ballsInside = new();

        public float Height => height;
        public float Radius => radius;
        public Transform PanelRef => panelRef;
        public Vector3 WorldPosition => transform.position;

        public UnityEvent scored;

        private float Capacity => Mathf.PI * binRadius * binRadius * binHeight * packingFactor;

        private void OnTriggerEnter(Collider other)
        {
            if (ballLayerMask.Contains(other.gameObject.layer))
            {
                BallBehaviour ball = other.attachedRigidbody.GetComponent<BallBehaviour>();
                if (!ballsInside.Contains(ball))
                {
                    OnScored();
                    ball.HasScored = true;
                    ballsInside.Add(ball);
                    RemoveBallIfNeeded();
                }
            }
        }

        private void RemoveBallIfNeeded()
        {
            while (ballsInside.Count > 1 && ballsInside.TotalVolume() > Capacity)
            {
                BallBehaviour ballToDisable = ballsInside[0];
                ballsInside.RemoveAt(0);
                ballToDisable.SetActive(false);
            }
        }

        private void OnScored()
        {
            ps.Play();
            scored.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.color = Color.cyan;
            DrawBinGizmo();
        }

        private void DrawBinGizmo()
        {
            Vector3 center = transform.position + transform.up * (binHeight * 0.5f);
            Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(binRadius * 2f, binHeight, binRadius * 2f));
            Gizmos.matrix = Matrix4x4.identity;
        }

        public void SetTriggerActive(bool isActive)
        {
            trigger.enabled = isActive;
        }

        public void Show(Vector3 position, Vector3 upDirection, Vector3 lookTarget)
        {
            transform.position = position;
            transform.up = upDirection;
            FaceOnY(lookTarget);
            SetTriggerActive(false);
            gameObject.SetActive(true);
            lookHerePanel?.SetActive(true);
        }

        private void FaceOnY(Vector3 lookTarget)
        {
            Vector3 flatTarget = new(lookTarget.x, transform.position.y, lookTarget.z);
            Vector3 toTarget = flatTarget - transform.position;
            if (toTarget.sqrMagnitude < 0.0001f)
                return;
            transform.rotation = Quaternion.LookRotation(toTarget, transform.up);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
