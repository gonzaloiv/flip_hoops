using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class BallTrail : MonoBehaviour
    {
        [SerializeField] private GameObject trailPrefab;
        [SerializeField] private Transform body;

        private GameObject trail;

        public void SpawnTrail()
        {
            trail = Instantiate(trailPrefab, body);
        }

        private void OnDisable()
        {
            if (trail != null)
                Destroy(trail);
        }
    }
}