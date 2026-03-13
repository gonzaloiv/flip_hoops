using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class GravitySpawner : MonoBehaviour
    {
        [SerializeField] private GravityData[] gravities;
        [SerializeField] private PosterBehaviour[] posters;

        private GravityData current;

        public GravityData Spawn(GravityFilter filter)
        {
            if (filter == GravityFilter.OnTheFloor)
            {
                current = gravities.First(g => g.surfaceTypes == MRUK.SurfaceType.FACING_UP);
            }
            else
            {
                current = GetRandom();
            }
            SpawnPosters();
            return current;
        }

        private GravityData GetRandom()
        {
            if (current == null)
            {
                return current = gravities[Random.Range(0, gravities.Length)];
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
                poster.Spawn(current.direction);
            }
        }
    }
}