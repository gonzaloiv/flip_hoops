using System;
using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class ModifierBehaviour : MonoBehaviour, IObligatoryActivatable
    {
        [SerializeField] private Renderer pulseRenderer;
        [SerializeField] private Color pulseColor = new Color(0.35f, 0.85f, 1f, 1f);
        [SerializeField] private float pulseSpeed = 2f;

        private bool obligatory;
        private bool activatedThisThrow;
        private ModifierScoreEffectKind scoreEffectKind;
        private float scoreEffectValue = 1.5f;
        private ObligatoryPulseVisual pulseVisual;

        public event Action<ModifierBehaviour> Activated;

        public bool IsObligatory => obligatory;
        public bool ActivatedThisThrow => activatedThisThrow;
        public ModifierScoreEffectKind ScoreEffectKind => scoreEffectKind;
        public float ScoreEffectValue => scoreEffectValue;

        private ObligatoryPulseVisual PulseVisual =>
            pulseVisual ??= new ObligatoryPulseVisual(pulseRenderer, pulseColor, pulseSpeed);

        public void Configure(
            bool isObligatory,
            ModifierScoreEffectKind effectKind,
            float effectValue)
        {
            obligatory = isObligatory;
            scoreEffectKind = effectKind;
            scoreEffectValue = effectValue;
            PulseVisual.EnsureMaterial();
            ResetForThrow();
        }

        public void Configure(bool isObligatory)
        {
            Configure(isObligatory, scoreEffectKind, scoreEffectValue);
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
            Activated?.Invoke(this);
        }

        private void Update() => PulseVisual.Tick();

        private void OnDestroy() => PulseVisual.DestroyMaterial();
    }
}
