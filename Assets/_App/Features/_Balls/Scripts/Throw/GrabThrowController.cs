using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalLove.Game.Balls
{
    public class GrabThrowController : MonoBehaviour
    {
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private Rigidbody body;
        [SerializeField] private BallThrowBehaviour throwBehaviour;
        [SerializeField] private int maxQueueValues = 10;
        [SerializeField] private int releaseSampleDeadZone = 2;
        [SerializeField] private float forceMultiplier = 1.25f;

        public UnityEvent selected;
        public UnityEvent unselected;
        public bool IsSelected { get; private set; }
        public bool ControlsFlight => throwBehaviour != null && throwBehaviour.ControlsFlight;

        private BallReleaseSampler releaseSampler = new();

        private void Awake()
        {
            grabbable ??= GetComponent<Grabbable>();
            body ??= GetComponent<Rigidbody>();
            throwBehaviour ??= GetComponent<BallThrowBehaviour>();
        }

        private void OnEnable()
        {
            if (grabbable != null)
                grabbable.WhenPointerEventRaised += Listen;
        }

        private void OnDisable()
        {
            if (grabbable != null)
                grabbable.WhenPointerEventRaised -= Listen;
            Clear();
        }

        private void FixedUpdate()
        {
            if (IsSelected)
                releaseSampler.Sample(transform, Time.fixedDeltaTime, maxQueueValues);
        }

        private void Listen(PointerEvent pointer)
        {
            if (pointer.Type == PointerEventType.Select)
                Select();
            if (pointer.Type == PointerEventType.Unselect)
                Unselect();
        }

        public void Clear()
        {
            releaseSampler.Clear();
            IsSelected = false;
        }

        private void Select()
        {
            IsSelected = true;
            releaseSampler.Clear();
            throwBehaviour.OnGrab(body.position);
            selected?.Invoke();
        }

        private void Unselect()
        {
            IsSelected = false;
            body.isKinematic = false;
            throwBehaviour.ApplyThrow(body, releaseSampler.Resolve(releaseSampleDeadZone), forceMultiplier);
            unselected?.Invoke();
        }
    }
}
