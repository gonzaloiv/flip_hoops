using System.Collections.Generic;
using DigitalLove.XR.MRUtilityKit;
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
            if (gravityDirection.y < 0f)
            {
                content.transform.localRotation = Quaternion.identity;
            }
            else if (gravityDirection.y > 0f)
            {
                content.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                content.transform.localRotation = Quaternion.Euler(0, 0, gravityDirection.sqrMagnitude * -90);
            }
        }
    }

    public static class PosterBehaviourExtensions
    {
        public static void Spawn(this IEnumerable<PosterBehaviour> posters, Vector3 direction)
        {
            foreach (PosterBehaviour poster in posters)
            {
                poster.Show(direction);
            }
        }
    }
}
