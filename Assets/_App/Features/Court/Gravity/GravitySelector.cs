using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class GravitySelector : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private GravityData current;

        public GravityData SelectRandom(List<GravityData> gravities)
        {
            return GetRandom(gravities);
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
    }
}