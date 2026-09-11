using DigitalLove.VFX;
using DigitalLove.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class ThrowZone : MonoBehaviour
    {
        [SerializeField] private OnTheFloorSpawner onTheFloorSpawner;
        [SerializeField] private GameObject floor;
        [SerializeField] private AudioSource onSpawnSource;
        [SerializeField] private AudioSource onUnspawnSource;
        [SerializeField] private ScalePunch scalePunch;

        public Vector3 WorldPosition => onTheFloorSpawner.WorldPosition;

        public bool TrySpawnForBand(
            DistanceData band,
            float maxRayDistance,
            out float scaleFactor,
            out float desiredMeters)
        {
            scaleFactor = 1f;
            desiredMeters = 0f;
            if (!ThrowAxisBandPose.TryPlaceForBand(
                    band,
                    maxRayDistance,
                    onTheFloorSpawner.Radius,
                    onTheFloorSpawner.ClearanceMask,
                    out Vector3 position,
                    out Vector3 forward,
                    out scaleFactor,
                    out desiredMeters))
                return false;

            ApplySpawn(position, forward);
            return true;
        }

        private void ApplySpawn(Vector3 position, Vector3 forward)
        {
            onTheFloorSpawner.ApplySpawn(position);
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            PlaySpawnFx();
        }

        private void PlaySpawnFx()
        {
            onSpawnSource.Play();
            floor.SetActive(true);
            scalePunch.Animate();
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
    }
}
