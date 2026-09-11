using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class SlingshotOriginBillboard : MonoBehaviour
    {
        private Camera cam;
        private Vector3 origin;

        private Camera Cam => cam ??= Camera.main;

        public void Show(Vector3 origin)
        {
            this.origin = origin;
            gameObject.SetActive(true);
            PinToOrigin();
        }

        public void Hide() => gameObject.SetActive(false);

        private void LateUpdate() => PinToOrigin();

        private void PinToOrigin()
        {
            transform.position = origin;
            if (Cam == null)
                return;
            transform.rotation = Quaternion.LookRotation(-Cam.transform.forward, Cam.transform.up);
        }
    }
}
