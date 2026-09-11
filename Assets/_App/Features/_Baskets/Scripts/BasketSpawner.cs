using System;
using DigitalLove.Game.Court;
using DigitalLove.Global;
using UnityEngine;
using UnityEngine.Pool;

namespace DigitalLove.Game.Basket
{
    public class BasketSpawner : MonoBehaviour
    {
        private const int PoolCapacity = 2;
        private const float WallClearance = 0.12f;

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private BasketPanel panel;

        private Vector3 position;
        private Vector3 normal;
        private ObjectPool<BasketBehaviour> pool;
        private BasketData currentData;
        private BasketBehaviour basket;

        public BasketBehaviour Basket => basket;
        public BasketPanel Panel => panel;

        public Action scored = () => { };

        public Vector3 SpawnOnAxis(
            BasketData data,
            GravityData gravity,
            Transform reference,
            float desiredMeters,
            float scaleFactor = 1f)
        {
            panel.HideAll();
            EnsureBasket(data);
            basket.ApplyScale(scaleFactor);
            Vector3 forward = FlatForward(reference);
            if (BasketThrowAxisPose.TryPlaceAtDistance(
                    gravity, reference.position, forward, desiredMeters, AcceptPose))
            {
                basket.Show(position, normal, reference.position);
                panel.transform.position = basket.PanelRef.position;
                return -normal;
            }

            basket.ResetScale();
            Debug.LogWarning("Not possible to spawn basket on axis");
            return Vector3.zero;
        }

        public void Hide()
        {
            basket?.ResetScale();
            basket?.Hide();
        }

        public void ShowScore(int score, bool hasMultiplier) => Panel.ShowScore(score, hasMultiplier);

        private bool AcceptPose(Vector3 candidate, Vector3 candidateNormal)
        {
            Vector3 n = candidateNormal.normalized;
            float push = basket.Radius + WallClearance;
            Vector3 start = candidate + n * push;
            Vector3 end = candidate + n * (basket.Height + push);
            float checkRadius = Mathf.Max(0.05f, basket.Radius * 0.7f);
            if (Physics.CheckCapsule(start, end, checkRadius, layerMask))
                return false;

            position = candidate;
            normal = candidateNormal;
            return true;
        }

        private static Vector3 FlatForward(Transform reference)
        {
            Vector3 forward = reference.forward;
            forward.y = 0f;
            return forward.sqrMagnitude < 0.0001f ? Vector3.zero : forward.normalized;
        }

        private void EnsureBasket(BasketData data)
        {
            if (basket != null && currentData == data)
                return;

            ReleaseBasket();
            EnsurePool(data);
            basket = pool.Get();
            basket.scored.AddListener(OnBasketScored);
        }

        private void EnsurePool(BasketData data)
        {
            if (pool != null && currentData == data)
                return;

            ClearPool();
            currentData = data;
            pool = ComponentObjectPool.Create(data.prefab, transform, null, PoolCapacity);
            ComponentObjectPool.Warm(pool, 1);
        }

        private void OnBasketScored() => scored.Invoke();

        private void ReleaseBasket()
        {
            if (basket == null)
                return;

            basket.scored.RemoveListener(OnBasketScored);
            basket.ResetScale();
            basket.Hide();
            pool.Release(basket);
            basket = null;
        }

        private void ClearPool()
        {
            ReleaseBasket();
            pool?.Clear();
            pool = null;
            currentData = null;
        }

        private void OnDestroy() => ClearPool();
    }
}
