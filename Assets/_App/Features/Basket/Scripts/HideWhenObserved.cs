using DigitalLove.Global;
using DigitalLove.UI.Behaviours;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class HideWhenObserved : MonoBehaviour
    {
        private const float MaxRaycastMeters = 10;

        [SerializeField] private Collider col;
        [SerializeField] private float observedSecsToHide = 1;
        [SerializeField] private float secsToHide = 0.66f;

        private Camera cam;
        private Camera Cam => cam ??= Camera.main;

        private float countdown;
        private bool isHiding;

        private void OnEnable()
        {
            isHiding = false;
        }

        private void Update()
        {
            if (isHiding)
                return;
            if (Cam.RaycastAllAgainst(col, MaxRaycastMeters))
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0)
                {
                    isHiding = true;
                    this.InvokeAfterSecs(secsToHide, () => this.SetActive(false));
                }
            }
            else
            {
                countdown = observedSecsToHide;
                isHiding = false;
            }
        }
    }
}