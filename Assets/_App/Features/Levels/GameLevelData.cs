using DigitalLove.Casual.Levels;
using DigitalLove.Game.Court;
using DigitalLove.Game.Balls;
using UnityEngine;

namespace DigitalLove.Game.Levels
{
    [CreateAssetMenu(fileName = "GameLevelData", menuName = "DigitalLove/Game/GameLevelData")]
    public class GameLevelData : LevelData
    {
        public const int BasketsToScore = 2;

        [Header("GameLevelData")]
        public GravityData gravity;
        public BallData ball;
        public DistanceData distance;
        public bool isCountdownLevel = false;

        public string InitKey => $"level_{id}_init";
    }
}