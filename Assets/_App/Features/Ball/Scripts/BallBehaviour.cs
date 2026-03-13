using System.Collections.Generic;
using System.Linq;
using DigitalLove.Game.Court;
using DigitalLove.Global;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalLove.Game.Balls
{
    public class BallBehaviour : MonoBehaviour
    {
        [SerializeField] private BallData data;
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private int maxQueueValues = 10;
        [SerializeField] private float forceMultiplier = 10;

        public UnityEvent hover;
        public UnityEvent unhover;
        public UnityEvent select;
        public UnityEvent unselect;

        public UnityEvent collisionEnter;

        private GravityData gravity;
        private Queue<Vector3> queue = new();
        private bool isSelected;
        private bool hasBeenUnselected;
        private Vector3 previousPosition;

        public BallData Data => data;
        public GravityData Gravity { set { gravity = value; } }
        public bool HasBeenUnselected => hasBeenUnselected;
        public bool IsActive => gameObject.activeInHierarchy;
        public int Score => data.score;

        private void OnEnable()
        {
            isSelected = false;
            hasBeenUnselected = false;
            rb.isKinematic = true;
            grabbable.WhenPointerEventRaised += ListenPointer;
        }

        private void OnDisable()
        {
            gravity = null;
            grabbable.WhenPointerEventRaised -= ListenPointer;
        }

        private void ListenPointer(PointerEvent pointer)
        {
            if (pointer.Type == PointerEventType.Hover)
                OnHover();
            if (pointer.Type == PointerEventType.Unhover)
                OnUnhover();
            if (pointer.Type == PointerEventType.Select)
                OnSelect();
            if (pointer.Type == PointerEventType.Unselect)
                OnUnselect();
        }

        [Button]
        private void OnHover() => hover.Invoke();

        [Button]
        private void OnUnhover() => unhover.Invoke();

        [Button]
        private void OnSelect()
        {
            isSelected = true;
            queue.Clear();
            select.Invoke();
        }

        [Button]
        private void OnUnselect()
        {
            isSelected = false;
            hasBeenUnselected = true;
            Vector3 total = Vector3.zero;
            foreach (Vector3 value in queue)
            {
                total += value;
            }
            rb.isKinematic = false;
            if (queue.Count > 0)
            {
                Vector3 median = total / queue.Count();
                rb.AddForce(median * forceMultiplier, ForceMode.Impulse);
            }
            unselect.Invoke();
        }

        private void FixedUpdate()
        {
            if (isSelected)
            {
                if (queue.Count > maxQueueValues)
                    queue.Dequeue();
                Vector3 delta = transform.position - previousPosition;
                queue.Enqueue(delta);
                previousPosition = transform.position;
            }
            else if (hasBeenUnselected && gravity != null)
            {
                rb.AddForce(gravity.direction * gravity.force, ForceMode.Force);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            collisionEnter.Invoke();
        }
    }
}