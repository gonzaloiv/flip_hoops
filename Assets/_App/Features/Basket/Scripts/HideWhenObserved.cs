using DigitalLove.Global;
using DigitalLove.UI.Behaviours;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class HideWhenObserved : MonoBehaviour
    {
        private const float MaxRaycastMeters = 10;

        [SerializeField] private Collider col;
        [SerializeField] private float observedSecsToHide = 2;

        private Camera cam;
        private Camera Cam => cam ??= Camera.main;

        private float countdown;

        private void Update()
        {
            if (Cam.RaycastAllAgainst(col, MaxRaycastMeters))
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0)
                    this.SetActive(false);
            }
            else
            {
                countdown = observedSecsToHide;
            }
        }
    }
}