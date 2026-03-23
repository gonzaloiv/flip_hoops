using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class GrabBallPanel : MonoBehaviour
    {
        [SerializeField] private GameObject label;
        [SerializeField] private GameObject video;
        [SerializeField] private float maxDistanceForVideo = 1;

        [Inject] private MemoryDataClient memoryDataClient;

        private bool isActive;
        private Camera cam;

        private Camera Cam => cam ??= Camera.main;

        public void Show()
        {
            isActive = true;
            label.SetActive(true);
        }

        private void Update()
        {
            if (isActive && memoryDataClient.Get<Play>().Tries == 0)
            {
                float distanceToCamera = Vector3.Distance(video.transform.position, Cam.transform.position);
                if (distanceToCamera < maxDistanceForVideo)
                    video.SetActive(true);
            }
            else
            {
                if (video.activeInHierarchy)
                    video.SetActive(false);
            }
        }

        public void Hide()
        {
            isActive = false;
            label.SetActive(false);
            video.SetActive(false);
        }
    }
}