using DigitalLove.Casual.Levels;
using DigitalLove.Game.Court;
using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Obstacles;
using UnityEngine;

namespace DigitalLove.Game.Levels
{
    [CreateAssetMenu(fileName = "GameLevelData", menuName = "DigitalLove/Game/GameLevelData")]
    public class GameLevelData : LevelData
    {
        public const int BasketsToScore = 2;

        [Header("GameLevelData.Core")]
        public BallData ball;
        public BasketData basket;
        public DistanceData distance;
        public GravityData gravity;

        [Header("GameLevelData.Obstacles")]
        public ObstaclePlacement[] obstacles;

        [Header("GameLevelData.Other")]
        public bool isCountdownLevel = false;
    }
}
