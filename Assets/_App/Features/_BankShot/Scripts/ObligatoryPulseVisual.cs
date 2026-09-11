using UnityEngine;

namespace DigitalLove.Game.BankShot
{
    public class ObligatoryPulseVisual
    {
        private readonly Renderer pulseRenderer;
        private readonly Color pulseColor;
        private readonly float pulseSpeed;
        private Material runtimeMaterial;
        private Color baseColor;
        private bool pulseActive;

        public ObligatoryPulseVisual(
            Renderer pulseRenderer,
            Color pulseColor,
            float pulseSpeed)
        {
            this.pulseRenderer = pulseRenderer;
            this.pulseColor = pulseColor;
            this.pulseSpeed = pulseSpeed;
        }

        public void EnsureMaterial()
        {
            if (pulseRenderer == null || runtimeMaterial != null)
                return;

            runtimeMaterial = pulseRenderer.material;
            baseColor = runtimeMaterial.color;
        }

        public void SetPulseActive(bool active)
        {
            pulseActive = active;
            if (!active)
                ApplySettledColor();
        }

        public void Tick()
        {
            if (!pulseActive || runtimeMaterial == null)
                return;

            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            runtimeMaterial.color = Color.Lerp(baseColor, pulseColor, t);
        }

        public void DestroyMaterial()
        {
            if (runtimeMaterial == null)
                return;

            Object.Destroy(runtimeMaterial);
            runtimeMaterial = null;
        }

        private void ApplySettledColor()
        {
            if (runtimeMaterial == null)
                return;

            runtimeMaterial.color = baseColor;
        }
    }
}
