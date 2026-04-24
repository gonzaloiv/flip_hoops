using System;
using DigitalLove.Game.Court;
using DigitalLove.XR.MRUtilityKit;
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
        [SerializeField] private Vector3 position;
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
            panel.HideAll();
            if (GetPosition(gravity, reference, distancesToReference))
            {
                basket.Show(position, normal);
                panel.transform.position = basket.PanelRef.position;
                return -normal;
            }
            else
            {
                return Vector3.zero;
            }
        }

        private bool GetPosition(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            GetPositionOnSurface(gravity, reference, distancesToReference);
            if (position == Vector3.zero)
            {
                iterations--;
                if (iterations > 0)
                {
                    return GetPosition(gravity, reference, distancesToReference);
                }
                else
                {
                    Debug.LogWarning("Not possible to spawn basket");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void GetPositionOnSurface(GravityData gravity, Transform reference, float[] distancesToReference)
        {
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(gravity.surfaceTypes, basket.Radius, new LabelFilter(gravity.sceneLabels), out position, out normal);
            float distance = Vector3.Distance(position, new Vector3(reference.position.x, position.y, reference.position.z));
            bool isInSpawnZone = distance > distancesToReference[0] && distance < distancesToReference[1];
            if (!isInSpawnZone)
                position = Vector3.zero;
            Vector3 startPosition = position + normal.normalized * basket.Radius;
            Vector3 endPosition = position + normal.normalized * (basket.Height + basket.Radius);
            bool isColliding = Physics.CheckCapsule(startPosition, endPosition, basket.Radius, layerMask);
            if (isColliding)
                position = Vector3.zero;
        }

        private void OnBasketScored(int score)
        {
            scored.Invoke(score);
        }

        public void Hide()
        {
            basket.Hide();
        }

        private void OnDestroy()
        {
            basket.scored.RemoveListener(OnBasketScored);
        }

        public void ShowScore(int score, bool hasMultiplier)
        {
            Panel.ShowScore(score, hasMultiplier);
        }
    }
}