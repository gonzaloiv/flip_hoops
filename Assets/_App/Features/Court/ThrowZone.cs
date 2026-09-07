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

        public void Spawn()
        {
            onTheFloorSpawner.Spawn();
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
