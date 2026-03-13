using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScoreboardSpawner : MonoBehaviour
    {
        private const int MaxIterations = 666;

        [SerializeField] private ScoreboardPanel panel;
        [SerializeField] private MRUK.SurfaceType surfaceTypes;
        [SerializeField] private MRUKAnchor.SceneLabels sceneLabels;

        private int iterations;
        private bool hasBeenSpawned;
        private Vector3 candidate;
        private Vector3 rotation;
        private Transform reference;

        public bool HasBeenSpawned => hasBeenSpawned;
        public ScoreboardPanel Panel => panel;

        public void Spawn(Transform reference)
        {
            this.reference = reference;
            iterations = MaxIterations;
            if (GetValidPosition())
            {
                hasBeenSpawned = true;
                panel.transform.position = new Vector3(candidate.x, panel.transform.position.y, candidate.z);
                panel.transform.rotation = Quaternion.Euler(rotation);
            }
        }

        private bool GetValidPosition()
        {
            GetPositionOnSurface();
            if (candidate == Vector3.zero)
            {
                iterations--;
                if (iterations > 0)
                {
                    return GetValidPosition();
                }
                else
                {
                    Debug.LogWarning("Not possible to spawn scoreboard");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void GetPositionOnSurface()
        {
            Ray ray = new Ray(new Vector3(reference.position.x, panel.transform.position.y, reference.position.y), reference.forward);
            Pose pose = MRUK.Instance.GetCurrentRoom().GetBestPoseFromRaycast(ray, 10, new LabelFilter(sceneLabels), out MRUKAnchor sceneAnchor, out Vector3 normal, MRUK.PositioningMethod.DEFAULT);
            candidate = pose.position;
            rotation = pose.rotation.eulerAngles;
        }

    }
}
