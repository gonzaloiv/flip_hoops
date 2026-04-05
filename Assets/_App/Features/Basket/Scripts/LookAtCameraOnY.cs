using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class LookAtCameraOnY : MonoBehaviour
    {
        private Camera cam;
        private Camera Cam => cam ??= Camera.main;

        private void Update()
        {
            if (Cam == null)
                return;
            transform.rotation = GetTargetRotation();
        }

        private Quaternion GetTargetRotation()
        {
            Vector3 targetPosition = new(Cam.transform.position.x, transform.position.y, Cam.transform.position.z);
            Vector3 lookPosition = transform.position - targetPosition;
            Quaternion targetRotation = Quaternion.LookRotation(lookPosition, transform.up);
            return targetRotation;
        }
    }
}