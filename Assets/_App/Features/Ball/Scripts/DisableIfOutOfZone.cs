using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class DisableIfOutOfZone : MonoBehaviour
    {
        [SerializeField] private float maxMetersToOnGrabPosition = 1;

        private bool isGrabbed;
        private Vector3 onGrabPosition;

        private void OnEnable()
        {
            isGrabbed = false;
        }

        public void OnGrabStart()
        {
            isGrabbed = true;
            onGrabPosition = transform.position;
        }

        private void Update()
        {
            if (isGrabbed)
            {
                float distanceToOnGrabPosition = Vector3.Distance(onGrabPosition, transform.position);
                if (distanceToOnGrabPosition > maxMetersToOnGrabPosition)
                    this.SetActive(false);
            }
        }

        public void OnGrabStop()
        {
            isGrabbed = false;
        }
    }
}