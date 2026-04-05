using DigitalLove.VFX;
using DigitalLove.XR;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class ThrowZone : MonoBehaviour
    {
        private const int MaxIterationsCheckingCollisions = 450;
        private const int MaxIterationsNotCheckingCollisions = 50;

        [Header("Spawning")]
        [SerializeField] private float radius;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private MRUKUtil mrukUtil;
        [SerializeField] private MRUK.SurfaceType surfaceTypes;
        [SerializeField] private MRUKAnchor.SceneLabels sceneLabels;
        [SerializeField] private GameObject floor;

        [Header("Components")]
        [SerializeField] private AudioSource onSpawnSource;
        [SerializeField] private AudioSource onUnspawnSource;
        [SerializeField] private ScalePunch scalePunch;

        private int iterations;
        private bool checkCollisions;

        public Vector3 WorldPosition => transform.position;

        public void Spawn()
        {
            iterations = MaxIterationsCheckingCollisions;
            checkCollisions = true;
            Vector3 spawnPosition = GetPosition();
            transform.position = spawnPosition;
            onSpawnSource.Play();
            floor.SetActive(true);
            scalePunch.Animate();
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

                    checkCollisions = false;
                    iterations = MaxIterationsNotCheckingCollisions;
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
            if (checkCollisions)
            {
                Vector3 checkSpherePosition = new(candidate.x, radius * 2, candidate.z);
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
            else
            {
                return candidate;
            }
        }

        public void SetReference(Transform reference)
        {
            transform.LookAt(new Vector3(reference.position.x, transform.position.y, reference.position.z));
        }

        public void Unspawn()
        {
            scalePunch.Animate();
            onUnspawnSource.Play();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}