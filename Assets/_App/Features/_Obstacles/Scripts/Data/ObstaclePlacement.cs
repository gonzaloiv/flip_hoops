using System;
using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    [Serializable]
    public class ObstaclePlacement
    {
        public ObstacleData obstacle;
        public bool obligatory;

        [Tooltip("Lateral / height / depth each in {-1, 0, 1}; height −1 = floor")]
        public Vector3Int cell;
    }
}
