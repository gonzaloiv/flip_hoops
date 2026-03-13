using System;
using DigitalLove.Game.Court;
using DigitalLove.XR;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class BasketSpawner : MonoBehaviour
    {
        private const int MaxIterations = 666;

        [SerializeField] private float minDistanceToReference = 1.25f;
        [SerializeField] private float maxDistanceToReference = 1.75f;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private MRUKUtil mrukUtil;

        private int iterations;

        private BasketBehaviour basket;
        public BasketBehaviour Basket => basket;

        private BasketPanel panel;
        public BasketPanel Panel => panel ??= Basket.GetComponent<BasketPanel>();

        public Action scored = () => { };

        public void Spawn(GravityData gravity, Transform reference)
        {
            iterations = MaxIterations;
            basket = Instantiate(gravity.basket, transform).GetComponent<BasketBehaviour>();
            basket.transform.position = GetPosition(gravity, reference);
            basket.scored.AddListener(OnBasketScored);
        }

        private Vector3 GetPosition(GravityData gravity, Transform reference)
        {
            Vector3 candidate = GetPositionOnSurface(gravity, reference);
            if (candidate == Vector3.zero)
            {
                iterations--;
                if (iterations > 0)
                {
                    return GetPosition(gravity, reference);
                }
                else
                {
                    Debug.LogWarning("Not possible to spawn basket");
                    return Vector3.zero;
                }
            }
            else
            {
                return candidate;
            }
        }

        public Vector3 GetPositionOnSurface(GravityData gravity, Transform reference)
        {
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(gravity.surfaceTypes, basket.Radius, new LabelFilter(gravity.sceneLabels), out Vector3 candidate, out Vector3 normal);
            float distanceToReference = Vector3.Distance(candidate, new Vector3(reference.position.x, candidate.y, reference.position.z));
            bool isInSpawnZone = distanceToReference > minDistanceToReference && distanceToReference < maxDistanceToReference;
            if (!isInSpawnZone)
                return Vector3.zero;
            Vector3 checkSpherePosition = candidate - gravity.direction * basket.Radius;
            bool isColliding = Physics.CheckSphere(checkSpherePosition, basket.Radius, layerMask);
            if (isColliding)
                return Vector3.zero;
            return candidate;
        }

        private void OnBasketScored()
        {
            scored.Invoke();
        }

        public void Unspawn()
        {
            basket.scored.RemoveListener(OnBasketScored);
            Destroy(basket.gameObject);
        }
    }
}