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
        [SerializeField] private BasketBehaviour basket;
        [SerializeField] private BasketPanel panel;

        [Header("Debug")]
        [SerializeField] private Vector3 normal;

        private int iterations;

        public BasketBehaviour Basket => basket;
        public BasketPanel Panel => panel;

        public Action<int> scored = (score) => { };

        private void Start()
        {
            basket.scored.AddListener(OnBasketScored);
        }

        public Vector3 SpawnAndGetGravityDirection(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            iterations = MaxIterations;
            Vector3 candidate = GetPosition(gravity, reference, distancesToReference);
            panel.HideAll();
            if (candidate == Vector3.zero)
            {
                return Vector3.zero;
            }
            else
            {
                basket.Show(candidate, normal);
                // panel.transform.up = normal;
                return -normal;
            }
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
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(gravity.surfaceTypes, basket.Radius, new LabelFilter(gravity.sceneLabels), out Vector3 candidate, out normal);
            float distance = Vector3.Distance(candidate, new Vector3(reference.position.x, candidate.y, reference.position.z));
            bool isInSpawnZone = distance > distancesToReference[0] && distance < distancesToReference[1];
            if (!isInSpawnZone)
                return Vector3.zero;
            Debug.LogWarning($"gravity.surfaceTypes {gravity.surfaceTypes}");
            float radiusOffset = basket.Radius;
            Vector3 startPosition = candidate + normal.normalized * basket.Radius;
            Debug.LogWarning($"startPosition {startPosition}");
            Vector3 endPosition = candidate + normal.normalized * (basket.Height + basket.Radius);
            Debug.LogWarning($"endPosition {endPosition}");
            bool isColliding = Physics.CheckCapsule(startPosition, endPosition, basket.Radius, layerMask);
            if (isColliding)
                return Vector3.zero;
            return candidate;
        }

        private void OnBasketScored(int score)
        {
            scored.Invoke(score);
            Panel.ShowScore(score);
        }

        public void Hide()
        {
            basket.Hide();
        }

        private void OnDestroy()
        {
            basket.scored.RemoveListener(OnBasketScored);
        }
    }
}