using System.Collections.Generic;
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
        [SerializeField] private BallThrowBehaviour throwBehaviour;
        [SerializeField] private int maxQueueValues = 10;
        [SerializeField] private float forceMultiplier = 10;
        [SerializeField] private BallTrail trail;

        public UnityEvent hover;
        public UnityEvent unhover;
        public UnityEvent select;
        public UnityEvent unselect;

        public UnityEvent collisionEnter;
        public UnityEvent becameInactive;

        private Vector3 gravityDirection;
        private Queue<Vector3> queue = new();
        private bool isSelected;
        private bool hasBeenUnselected;
        private Vector3 previousPosition;
        private bool hasScored;
        private bool isInStreak;

        public BallData Data => data;
        public Vector3 GravityDirection { set { gravityDirection = value; } }
        public bool HasBeenUnselected => hasBeenUnselected;
        public bool IsActive => gameObject.activeInHierarchy;
        public bool HasScored { get { return hasScored; } set { hasScored = value; } }
        public float Volume => cachedVolume < 0f ? cachedVolume = ComputeVolume() : cachedVolume;

        private BallThrowBehaviour ThrowBehaviour => throwBehaviour ??= GetComponent<BallThrowBehaviour>();
        private float cachedVolume = -1f;

        private void OnEnable()
        {
            grabbable.WhenPointerEventRaised += ListenPointer;

            isSelected = false;
            hasBeenUnselected = false;
            hasScored = false;
            rb.isKinematic = true;
            trail.Reset();
        }

        private void OnDisable()
        {
            grabbable.WhenPointerEventRaised -= ListenPointer;

            gravityDirection = Vector3.zero;
            becameInactive?.Invoke();
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

        public void Invoke_OnSelect() => OnSelect();

        [Button]
        private void OnSelect()
        {
            isSelected = true;
            queue.Clear();
            select.Invoke();
        }

        public void Invoke_OnUnselect() => OnUnselect();

        [Button]
        private void OnUnselect()
        {
            isSelected = false;
            hasBeenUnselected = true;
            rb.isKinematic = false;
            ThrowBehaviour.ApplyThrow(rb, GetReleaseDelta(), forceMultiplier);
            unselect.Invoke();
            trail.ShowStreak(isInStreak);
        }

        public void SetThrowTarget(Transform target) => ThrowBehaviour.SetTarget(target);

        private Vector3 GetReleaseDelta()
        {
            if (queue.Count == 0)
                return Vector3.zero;

            Vector3 total = Vector3.zero;
            foreach (Vector3 value in queue)
                total += value;
            return total / queue.Count;
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
            else if (ShouldApplyGravity())
            {
                rb.AddForce(gravityDirection * GravityData.Force, ForceMode.Force);
            }
        }

        private bool ShouldApplyGravity()
        {
            return hasBeenUnselected
                && gravityDirection != Vector3.zero
                && !ThrowBehaviour.ControlsFlight;
        }

        private void OnCollisionEnter(Collision other)
        {
            collisionEnter.Invoke();
            trail.ShowStreak(false);
        }

        public void SetIsInStreak(bool isInStreak)
        {
            this.isInStreak = isInStreak;
        }

        private float ComputeVolume()
        {
            SphereCollider sphere = FindPhysicsSphere();
            if (sphere == null)
                return 0f;

            Vector3 scale = sphere.transform.lossyScale;
            float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            float radius = sphere.radius * maxScale;
            return (4f / 3f) * Mathf.PI * radius * radius * radius;
        }

        private SphereCollider FindPhysicsSphere()
        {
            SphereCollider[] spheres = GetComponentsInChildren<SphereCollider>();
            for (int i = 0; i < spheres.Length; i++)
            {
                if (!spheres[i].isTrigger)
                    return spheres[i];
            }
            return spheres.Length > 0 ? spheres[0] : null;
        }
    }
}
