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

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private MRUKUtil mrukUtil;

        private int iterations;

        private BasketBehaviour basket;
        public BasketBehaviour Basket => basket;

        private BasketPanel panel;
        public BasketPanel Panel => panel;

        public Action<int> scored = (score) => { };

        public void Spawn(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            iterations = MaxIterations;
            basket = Instantiate(gravity.basket, transform).GetComponent<BasketBehaviour>();
            basket.SetTriggerActive(false);
            basket.transform.position = GetPosition(gravity, reference, distancesToReference);
            basket.scored.AddListener(OnBasketScored);
            panel = basket.GetComponentInChildren<BasketPanel>();
            panel.HideAll();
        }

        private Vector3 GetPosition(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            Vector3 candidate = GetPositionOnSurface(gravity, reference, distancesToReference);
            if (candidate == Vector3.zero)
            {
                iterations--;
                if (iterations > 0)
                {
                    return GetPosition(gravity, reference, distancesToReference);
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

        public Vector3 GetPositionOnSurface(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(gravity.surfaceTypes, basket.Radius, new LabelFilter(gravity.sceneLabels), out Vector3 candidate, out Vector3 normal);
            float distance = Vector3.Distance(candidate, new Vector3(reference.position.x, candidate.y, reference.position.z));
            bool isInSpawnZone = distance > distancesToReference[0] && distance < distancesToReference[1];
            if (!isInSpawnZone)
                return Vector3.zero;
            Vector3 checkSpherePosition = candidate - gravity.direction * basket.Radius * 2;
            bool isColliding = Physics.CheckSphere(checkSpherePosition, basket.Radius, layerMask);
            if (isColliding)
                return Vector3.zero;
            return candidate;
        }

        private void OnBasketScored(int score)
        {
            scored.Invoke(score);
            Panel.ShowScore(score);
        }

        public void Unspawn()
        {
            basket.scored.RemoveListener(OnBasketScored);
            Destroy(basket.gameObject);
        }
    }
}