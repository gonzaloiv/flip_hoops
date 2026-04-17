using System.Collections.Generic;
using DigitalLove.Casual.Levels;
using DigitalLove.Game.Court;
using DigitalLove.Game.Balls;
using UnityEngine;
using DigitalLove.Game.Modifiers;

namespace DigitalLove.Game.Levels
{
    [CreateAssetMenu(fileName = "GameLevelData", menuName = "DigitalLove/Game/GameLevelData")]
    public class GameLevelData : LevelData
    {
        private const string LevelInfoKey = "info";

        [Header("GameLevelData")]
        public List<GravityData> gravities;
        public int basketsToScore;
        public List<BallData> balls;
        [Range(0.5f, 2f)] public float[] distance = new[] { 1.25f, 1.75f };
        public List<ModifierDataPercentagePair> modifiers;

        public bool IsWarmUp => basketsToScore > 0;
        public string InfoKey => $"level_{id}_{LevelInfoKey}";
        public bool HasModifiers => modifiers != null && modifiers.Count > 0;

    }
}