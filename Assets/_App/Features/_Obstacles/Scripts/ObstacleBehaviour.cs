using DigitalLove.Game.Balls;
using DigitalLove.Game.BankShot;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class ObstacleBehaviour : MonoBehaviour, IObligatoryActivatable
    {
        [SerializeField] private float clearanceRadius = 0.2f;
        [SerializeField] private Renderer pulseRenderer;
        [SerializeField] private Color pulseColor = new Color(1f, 0.85f, 0.2f, 1f);
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private VerticalSpanFitter verticalSpanFitter;

        private bool obligatory;
        private bool activatedThisThrow;
        private ObligatoryPulseVisual pulseVisual;

        public bool IsObligatory => obligatory;
        public bool ActivatedThisThrow => activatedThisThrow;
        public float ClearanceRadius => clearanceRadius;
        public bool FitsVerticalSpan => VerticalSpanFitterProp != null;

        private VerticalSpanFitter VerticalSpanFitterProp =>
            verticalSpanFitter ??= GetComponent<VerticalSpanFitter>();

        private ObligatoryPulseVisual PulseVisual =>
            pulseVisual ??= new ObligatoryPulseVisual(pulseRenderer, pulseColor, pulseSpeed);

        public void Configure(bool isObligatory)
        {
            obligatory = isObligatory;
            PulseVisual.EnsureMaterial();
            ResetForThrow();
        }

        public bool TryApplyVerticalFit(LayerMask surfaceMask)
        {
            VerticalSpanFitter fitter = VerticalSpanFitterProp;
            if (fitter == null)
                return true;

            return fitter.TryFit(surfaceMask);
        }

        public void ResetForThrow()
        {
            activatedThisThrow = false;
            PulseVisual.SetPulseActive(obligatory);
        }

        public void RegisterActivation()
        {
            if (activatedThisThrow)
                return;

            activatedThisThrow = true;
            PulseVisual.SetPulseActive(false);
        }

        private void Update() => PulseVisual.Tick();

        private void OnCollisionEnter(Collision collision)
        {
            if (!BallBehaviour.TryGetFromRigidbody(collision.rigidbody, out _))
                return;

            RegisterActivation();
        }

        private void OnDestroy() => PulseVisual.DestroyMaterial();
    }
}
