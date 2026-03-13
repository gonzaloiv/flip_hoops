using UnityEngine;

namespace DigitalLove.Game.Balls
{
    [CreateAssetMenu(fileName = "BallData", menuName = "DigitalLove/Game/BallData")]
    public class BallData : ScriptableObject
    {
        public string id;
        public int score;
    }
}
