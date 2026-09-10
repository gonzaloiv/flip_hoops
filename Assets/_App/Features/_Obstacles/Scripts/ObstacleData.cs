using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    [CreateAssetMenu(fileName = "ObstacleData", menuName = "DigitalLove/Game/ObstacleData")]
    public class ObstacleData : ScriptableObject
    {
        public string id;
        public ObstacleBehaviour prefab;
    }
}
