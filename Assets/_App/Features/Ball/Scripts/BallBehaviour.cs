using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class BallBehaviour : MonoBehaviour
    {
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private int maxQueueValues = 10;
        [SerializeField] private float forceMultiplier = 10;
        [SerializeField] private float gravityMultiplier = 9.8f;

        private Queue<Vector3> queue = new();
        private bool isSelected = false;
        private Vector3 previousPosition;

        private void OnEnable()
        {
            isSelected = false;
            grabbable.WhenPointerEventRaised += ListenPointer;
        }

        private void ListenPointer(PointerEvent pointer)
        {
            if (pointer.Type == PointerEventType.Select)
            {
                queue.Clear();
                isSelected = true;
            }
            if (pointer.Type == PointerEventType.Unselect)
            {
                isSelected = false;
                OnUnselect();
            }
        }

        private void OnUnselect()
        {
            Vector3 total = Vector3.zero;
            foreach (Vector3 value in queue)
            {
                total += value;
            }
            Vector3 median = total / queue.Count();
            rb.isKinematic = false;
            rb.AddForce(median * forceMultiplier, ForceMode.Impulse);
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
            else if (queue.Count > 0)
            {
                rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Force);
            }
        }
    }
}