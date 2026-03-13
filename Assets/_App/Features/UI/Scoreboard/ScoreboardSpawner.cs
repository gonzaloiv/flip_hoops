using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScoreboardSpawner : MonoBehaviour
    {
        [SerializeField] private ScoreboardPanel panel;
        [SerializeField] private MRUK.SurfaceType surfaceTypes;
        [SerializeField] private MRUKAnchor.SceneLabels sceneLabels;
        [SerializeField] private float minY = 1.25f;

        private bool hasBeenSpawned;

        public bool HasBeenSpawned => hasBeenSpawned;
        public ScoreboardPanel Panel => panel;

        public void Spawn()
        {
            hasBeenSpawned = true;
            MRUK.Instance.GetCurrentRoom().GenerateRandomPositionOnSurface(surfaceTypes, 0.66f, new LabelFilter(sceneLabels), out Vector3 position, out Vector3 normal);
            panel.transform.position = position;
            if (panel.transform.position.y < minY)
                panel.transform.position = new Vector3(panel.transform.position.x, minY, panel.transform.position.z);
            panel.transform.forward = normal;
        }
    }
}
