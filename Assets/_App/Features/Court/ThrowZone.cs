using DigitalLove.XR;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class ThrowZone : MonoBehaviour
    {
        private const int MaxIterations = 666;

        [SerializeField] private float radius;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private MRUKUtil mrukUtil;
        [SerializeField] private MRUK.SurfaceType surfaceTypes;
        [SerializeField] private MRUKAnchor.SceneLabels sceneLabels;

        private int iterations;

        public void Spawn()
        {
            iterations = MaxIterations;
            gameObject.SetActive(true);
            Vector3 spawnPosition = GetPosition();
            transform.position = spawnPosition;
        }

        private Vector3 GetPosition()
        {
            Vector3 candidate = GetPositionOnSurface();
            if (candidate == Vector3.zero)
            {
                iterations--;
                if (iterations > 0)
                {
                    return GetPosition();
                }
                else
                {
                    Debug.LogWarning("Not possible to spawn throw zone");
                    return Vector3.zero;
                }
            }
            else
            {
                return candidate;
            }
        }

        public Vector3 GetPositionOnSurface()
        {
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(surfaceTypes, radius, new LabelFilter(sceneLabels), out Vector3 candidate, out Vector3 normal);
            Vector3 checkSpherePosition = new(candidate.x, radius * 1.05f, candidate.z);
            bool isColliding = Physics.CheckSphere(checkSpherePosition, radius, layerMask);
            if (!isColliding)
            {
                return candidate;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void SetReference(Transform reference)
        {
            transform.LookAt(new Vector3(reference.position.x, transform.position.y, reference.position.z));
        }

        public void Unspawn()
        {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}