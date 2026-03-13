using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class PosterBehaviour : MonoBehaviour
    {
        [SerializeField] private MRUK.SurfaceType surfaceTypes;
        [SerializeField] private MRUKAnchor.SceneLabels sceneLabels;
        [SerializeField] private GameObject posterContent;

        private bool hasBeenSpawned;

        public void Spawn(Vector3 gravityDirection)
        {
            if (!hasBeenSpawned)
            {
                MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(surfaceTypes, 0.66f, new LabelFilter(sceneLabels), out Vector3 position, out Vector3 normal);
                transform.position = position;
                transform.forward = normal;
                hasBeenSpawned = true;
            }
            posterContent.transform.localRotation = Quaternion.Euler(0, 0, gravityDirection.y == -1 ? 0 : 180);
        }
    }
}
