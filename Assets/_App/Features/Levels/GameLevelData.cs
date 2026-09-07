using System.Collections.Generic;
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
        public List<GravityData> gravities;
        public BallData ball;
        public DistanceData distance;
        public bool isCountdownLevel = false;

        public string InfoKey => $"level_{id}_info";
    }
}