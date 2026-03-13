using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class BallTrail : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trail;

        public void ShowTrail()
        {
            trail.Clear();
            trail.enabled = true;
        }

        private void OnDisable()
        {
            trail.enabled = false;
        }
    }
}