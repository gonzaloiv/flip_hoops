using System;
using DigitalLove.Game.Court;
using DigitalLove.Global;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Pool;

namespace DigitalLove.Game.Basket
{
    public class BasketSpawner : MonoBehaviour
    {
        private const int MaxIterations = 666;
        private const int PoolCapacity = 2;

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private BasketPanel panel;

        private Vector3 position;
        private Vector3 normal;
        private ObjectPool<BasketBehaviour> pool;
        private BasketData currentData;
        private BasketBehaviour basket;
        private int iterations;

        public BasketBehaviour Basket => basket;
        public BasketPanel Panel => panel;

        public Action scored = () => { };

        public Vector3 SpawnAndGetGravityDirection(
            BasketData data,
            GravityData gravity,
            Transform reference,
            float[] distancesToReference)
        {
            iterations = MaxIterations;
            panel.HideAll();
            EnsureBasket(data);
            if (GetPosition(gravity, reference, distancesToReference))
            {
                basket.Show(position, normal, reference.position);
                panel.transform.position = basket.PanelRef.position;
                return -normal;
            }

            return Vector3.zero;
        }

        public void Hide()
        {
            basket?.Hide();
        }

        public void ShowScore(int score, bool hasMultiplier)
        {
            Panel.ShowScore(score, hasMultiplier);
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

        private bool GetPosition(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            GetPositionOnSurface(gravity, reference, distancesToReference);
            if (position != Vector3.zero)
                return true;

            iterations--;
            if (iterations <= 0)
            {
                Debug.LogWarning("Not possible to spawn basket");
                return false;
            }

            return GetPosition(gravity, reference, distancesToReference);
        }

        public void GetPositionOnSurface(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(
                gravity.surfaceTypes,
                basket.Radius,
                new LabelFilter(gravity.sceneLabels),
                out position,
                out normal);
            float distance = Vector3.Distance(
                position,
                new Vector3(reference.position.x, position.y, reference.position.z));
            bool isInSpawnZone = distance > distancesToReference[0] && distance < distancesToReference[1];
            if (!isInSpawnZone)
                position = Vector3.zero;
            Vector3 startPosition = position + normal.normalized * basket.Radius;
            Vector3 endPosition = position + normal.normalized * (basket.Height + basket.Radius);
            if (Physics.CheckCapsule(startPosition, endPosition, basket.Radius, layerMask))
                position = Vector3.zero;
        }

        private void OnBasketScored() => scored.Invoke();

        private void ReleaseBasket()
        {
            if (basket == null)
                return;

            basket.scored.RemoveListener(OnBasketScored);
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

        private void OnDestroy()
        {
            ClearPool();
        }
    }
}
