using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class GrabBallPanel : MonoBehaviour
    {
        [SerializeField] private GameObject label;
        [SerializeField] private GameObject video;
        [SerializeField] private float maxDistanceForVideo = 1;

        private bool isActive;
        private bool showVideo;
        private Camera cam;

        private Camera Cam => cam ??= Camera.main;

        public void Show(bool showVideo)
        {
            isActive = true;
            this.showVideo = showVideo;
            label.SetActive(true);
        }

        private void Update()
        {
            if (isActive && showVideo)
            {
                float distanceToCamera = Vector3.Distance(video.transform.position, Cam.transform.position);
                if (distanceToCamera < maxDistanceForVideo)
                    video.SetActive(true);
            }
            else if (video.activeInHierarchy)
            {
                video.SetActive(false);
            }
        }

        public void Hide()
        {
            isActive = false;
            showVideo = false;
            label.SetActive(false);
            video.SetActive(false);
        }
    }
}
