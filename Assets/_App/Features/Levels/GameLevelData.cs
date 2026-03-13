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
        private const string LevelInfoKey = "info";

        public GravityFilter gravityFilter;
        public int basketsToScore;
        public List<BallData> balls;

        public bool IsWarmUp => basketsToScore > 0;
        public string InfoKey => $"level_{id}_{LevelInfoKey}";
    }
}