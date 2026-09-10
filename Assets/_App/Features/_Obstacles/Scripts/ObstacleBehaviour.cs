using DigitalLove.Game.Balls;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class ObstacleBehaviour : MonoBehaviour
    {
        [SerializeField] private float clearanceRadius = 0.2f;
        [SerializeField] private Renderer pulseRenderer;
        [SerializeField] private Color pulseColor = new Color(1f, 0.85f, 0.2f, 1f);
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private VerticalSpanFitter verticalSpanFitter;

        private bool obligatory;
        private bool hitThisThrow;
        private Color baseColor;
        private Material runtimeMaterial;
        private bool pulseActive;

        public bool IsObligatory => obligatory;
        public bool HitThisThrow => hitThisThrow;
        public float ClearanceRadius => clearanceRadius;
        public bool FitsVerticalSpan => VerticalSpanFitterProp != null;

        private VerticalSpanFitter VerticalSpanFitterProp =>
            verticalSpanFitter ??= GetComponent<VerticalSpanFitter>();

        public void Configure(bool isObligatory)
        {
            obligatory = isObligatory;
            EnsureMaterial();
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
            hitThisThrow = false;
            pulseActive = obligatory;
            ApplySettledColor();
        }

        public void RegisterHit()
        {
            if (hitThisThrow)
                return;

            hitThisThrow = true;
            pulseActive = false;
            ApplySettledColor();
        }

        private void Update()
        {
            if (!pulseActive || runtimeMaterial == null)
                return;

            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            runtimeMaterial.color = Color.Lerp(baseColor, pulseColor, t);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody == null)
                return;

            BallBehaviour ball = collision.rigidbody.GetComponent<BallBehaviour>();
            if (ball == null)
                return;

            RegisterHit();
        }

        private void EnsureMaterial()
        {
            if (pulseRenderer == null || runtimeMaterial != null)
                return;

            runtimeMaterial = pulseRenderer.material;
            baseColor = runtimeMaterial.color;
        }

        private void ApplySettledColor()
        {
            if (runtimeMaterial == null)
                return;

            runtimeMaterial.color = baseColor;
        }

        private void OnDestroy()
        {
            if (runtimeMaterial != null)
                Destroy(runtimeMaterial);
        }
    }
}
