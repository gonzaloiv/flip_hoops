using System;
using System.Collections.Generic;
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
            Debug.LogWarning($"gravityDirection {gravityDirection}");
            if (!onTheWallSpawner.HasBeenSpawned)
                onTheWallSpawner.Spawn();
            content.transform.localRotation = Quaternion.identity;
            bool isInvertedGravity = gravityDirection.y < 0f;
            Debug.LogWarning($"gravityDirection.y {gravityDirection.y}");
            if (isInvertedGravity)
            {
                content.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                float magnitude = gravityDirection.sqrMagnitude;
                Debug.LogWarning($"magnitude{magnitude}");
                if (magnitude > 0)
                    content.transform.localRotation = Quaternion.Euler(0, 0, magnitude * -90);
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
