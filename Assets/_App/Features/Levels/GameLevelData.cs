using DigitalLove.Casual.Levels;
using DigitalLove.Game.Court;
using UnityEngine;

namespace DigitalLove.Game.Levels
{
    [CreateAssetMenu(fileName = "GameLevelData", menuName = "DigitalLove/Game/GameLevelData")]
    public class GameLevelData : LevelData
    {
        private const string LevelInfoKey = "info";

        public GravityFilter gravityFilter;
        public int ballsToScore;

        public bool IsWarmUp => ballsToScore > 0;
        public string InfoKey => $"level_{id}_{LevelInfoKey}";
    }
}