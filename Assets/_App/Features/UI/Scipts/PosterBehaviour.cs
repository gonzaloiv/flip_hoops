using DigitalLove.XR;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class PosterBehaviour : MonoBehaviour
    {
        [SerializeField] private OnTheWallSpawner onTheWallSpawner;
        [SerializeField] private GameObject content;

        public void Show(Vector3 gravityDirection)
        {
            if (!onTheWallSpawner.HasBeenSpawned)
                onTheWallSpawner.Spawn();
            content.transform.localRotation = Quaternion.Euler(0, 0, gravityDirection.y == -1 ? 0 : 180);
        }
    }
}
