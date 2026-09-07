using System.Collections.Generic;
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
                return current = gravities[Random.Range(0, gravities.Count)];

            GravityData next;
            do
            {
                next = gravities[Random.Range(0, gravities.Count)];
            }
            while (next == current);

            return current = next;
        }
    }
}
