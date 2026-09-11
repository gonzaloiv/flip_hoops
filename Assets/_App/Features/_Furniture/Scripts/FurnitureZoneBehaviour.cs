using System;
using DigitalLove.Game.Balls;
using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Furniture
{
    public class FurnitureZoneBehaviour : MonoBehaviour
    {
        private bool activatedThisThrow;
        private ThrowScoreOpKind scoreEffectKind;
        private float scoreEffectValue;
        private Collider hostCollider;
        private PhysicsMaterial originalMaterial;
        private Renderer hostRenderer;
        private MaterialPropertyBlock propertyBlock;
        private bool hasTint;

        public event Action<FurnitureZoneBehaviour> Activated;

        public bool ActivatedThisThrow => activatedThisThrow;
        public ThrowScoreOpKind ScoreEffectKind => scoreEffectKind;
        public float ScoreEffectValue => scoreEffectValue;

        public void Configure(
            ThrowScoreOpKind effectKind,
            float effectValue,
            Color tint,
            PhysicsMaterial bounceMaterial)
        {
            scoreEffectKind = effectKind;
            scoreEffectValue = effectValue;
            hostCollider = GetComponent<Collider>();
            hostRenderer = GetComponent<Renderer>();
            ApplyTint(tint);
            ApplyBounce(bounceMaterial);
            ResetForThrow();
        }

        public void ResetForThrow() => activatedThisThrow = false;

        public void Restore()
        {
            ClearTint();
            RestoreBounce();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!BallBehaviour.TryGetFromRigidbody(collision.rigidbody, out _))
                return;

            RegisterActivation();
        }

        private void RegisterActivation()
        {
            if (activatedThisThrow)
                return;

            activatedThisThrow = true;
            Activated?.Invoke(this);
        }

        private void ApplyTint(Color tint)
        {
            if (hostRenderer == null)
                return;

            propertyBlock ??= new MaterialPropertyBlock();
            hostRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", tint);
            propertyBlock.SetColor("_Color", tint);
            hostRenderer.SetPropertyBlock(propertyBlock);
            hasTint = true;
        }

        private void ClearTint()
        {
            if (!hasTint || hostRenderer == null)
                return;

            hostRenderer.SetPropertyBlock(null);
            hasTint = false;
        }

        private void ApplyBounce(PhysicsMaterial bounceMaterial)
        {
            if (hostCollider == null || bounceMaterial == null)
                return;

            originalMaterial = hostCollider.sharedMaterial;
            hostCollider.sharedMaterial = bounceMaterial;
        }

        private void RestoreBounce()
        {
            if (hostCollider == null)
                return;

            hostCollider.sharedMaterial = originalMaterial;
            originalMaterial = null;
        }
    }
}
