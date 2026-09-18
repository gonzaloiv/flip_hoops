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
        [SerializeField] private BallTrail trail;
        [SerializeField] private BallIdleMotion idleMotion;

        public UnityEvent hover;
        public UnityEvent unhover;
        public UnityEvent select;
        public UnityEvent unselect;

        public UnityEvent collisionEnter;
        public UnityEvent becameInactive;

        private Vector3 gravityDirection;
        private GrabThrowController grabThrow;
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
        private GrabThrowController GrabThrow => grabThrow ??= GetComponent<GrabThrowController>();
        private float cachedVolume = -1f;

        private void OnEnable()
        {
            grabbable.WhenPointerEventRaised += ListenPointer;
            GrabThrow.selected.AddListener(OnSelect);
            GrabThrow.unselected.AddListener(OnUnselect);

            isSelected = false;
            hasBeenUnselected = false;
            hasScored = false;
            rb.isKinematic = true;
            GrabThrow.Clear();
            trail.Reset();
        }

        private void OnDisable()
        {
            grabbable.WhenPointerEventRaised -= ListenPointer;
            GrabThrow.selected.RemoveListener(OnSelect);
            GrabThrow.unselected.RemoveListener(OnUnselect);
            GrabThrow.Clear();

            gravityDirection = Vector3.zero;
            becameInactive?.Invoke();
        }

        private void ListenPointer(PointerEvent pointer)
        {
            if (pointer.Type == PointerEventType.Hover)
                OnHover();
            if (pointer.Type == PointerEventType.Unhover)
                OnUnhover();

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
            if (idleMotion != null)
                idleMotion.StopIdle();
            select.Invoke();
        }

        public static bool TryGetFromRigidbody(Rigidbody body, out BallBehaviour ball)
        {
            ball = null;
            if (body == null)
                return false;

            ball = body.GetComponent<BallBehaviour>();
            return ball != null;
        }

        public void Invoke_OnUnselect() => OnUnselect();

        [Button]
        private void OnUnselect()
        {
            isSelected = false;
            hasBeenUnselected = true;
            unselect.Invoke();
            trail.ShowStreak(isInStreak);
        }

        public void SetThrowTarget(Transform target) => ThrowBehaviour.SetTarget(target);

        private void FixedUpdate()
        {
            if (isSelected)
                return;

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
            float radius = PhysicsRadius();
            return (4f / 3f) * Mathf.PI * radius * radius * radius;
        }

        private float PhysicsRadius()
        {
            SphereCollider sphere = FindPhysicsSphere();
            if (sphere == null)
                return 0f;
            Vector3 scale = sphere.transform.lossyScale;
            float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            return sphere.radius * maxScale;
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
