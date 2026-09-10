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
        [SerializeField, Tooltip("Scales sampled release velocity (m/s) into throw velocity.")]
        private float forceMultiplier = 1.25f;
        [SerializeField] private BallTrail trail;
        [SerializeField] private BallIdleMotion idleMotion;

        public UnityEvent hover;
        public UnityEvent unhover;
        public UnityEvent select;
        public UnityEvent unselect;

        public UnityEvent collisionEnter;
        public UnityEvent becameInactive;

        private Vector3 gravityDirection;
        private BallReleaseSampler releaseSampler;
        private bool isSelected;
        private bool hasBeenUnselected;
        private bool hasScored;
        private bool isInStreak;

        public BallData Data => data;
        public Vector3 GravityDirection { set { gravityDirection = value; } }
        public bool HasBeenUnselected => hasBeenUnselected;
        public bool IsActive => gameObject.activeInHierarchy;
        public bool HasScored { get { return hasScored; } set { hasScored = value; } }
        public float Volume => cachedVolume < 0f ? cachedVolume = ComputeVolume() : cachedVolume;

        private BallThrowBehaviour ThrowBehaviour => throwBehaviour ??= GetComponent<BallThrowBehaviour>();
        private BallReleaseSampler ReleaseSampler => releaseSampler ??= new BallReleaseSampler();
        private float cachedVolume = -1f;

        private void OnEnable()
        {
            grabbable.WhenPointerEventRaised += ListenPointer;

            isSelected = false;
            hasBeenUnselected = false;
            hasScored = false;
            rb.isKinematic = true;
            ReleaseSampler.Clear();
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
            ReleaseSampler.Clear();
            if (idleMotion != null)
                idleMotion.StopIdle();
            select.Invoke();
        }

        public void Invoke_OnUnselect() => OnUnselect();

        [Button]
        private void OnUnselect()
        {
            isSelected = false;
            hasBeenUnselected = true;
            rb.isKinematic = false;
            ThrowBehaviour.ApplyThrow(rb, ReleaseSampler.Resolve(), forceMultiplier);
            unselect.Invoke();
            trail.ShowStreak(isInStreak);
        }

        public void SetThrowTarget(Transform target) => ThrowBehaviour.SetTarget(target);

        private void FixedUpdate()
        {
            if (isSelected)
            {
                ReleaseSampler.Sample(transform, Time.fixedDeltaTime, maxQueueValues);
                return;
            }

            if (ShouldApplyGravity())
                rb.AddForce(gravityDirection * GravityData.Force, ForceMode.Force);
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
