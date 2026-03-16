using System.Collections.Generic;
using System.Linq;
using DigitalLove.Game.UI;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class GravitySpawner : MonoBehaviour
    {
        [SerializeField] private PosterBehaviour[] posters;

        private GravityData current;

        public GravityData Spawn(List<GravityData> gravities)
        {
            current = GetRandom(gravities);
            SpawnPosters();
            return current;
        }

        private GravityData GetRandom(List<GravityData> gravities)
        {
            if (current == null || gravities.Count == 1)
            {
                return current = gravities[Random.Range(0, gravities.Count)];
            }
            else
            {
                List<GravityData> selection = gravities.Where(g => g != current).ToList();
                return current = selection[Random.Range(0, selection.Count)];
            }
        }

        private void SpawnPosters()
        {
            foreach (PosterBehaviour poster in posters)
            {
                poster.Show(current.direction);
            }
        }
    }
}