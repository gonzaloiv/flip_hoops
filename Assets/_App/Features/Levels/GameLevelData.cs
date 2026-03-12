using DigitalLove.Casual.Levels;
using UnityEngine;

namespace DigitalLove.Game.Levels
{
    [CreateAssetMenu(fileName = "GameLevelData", menuName = "DigitalLove/Game/GameLevelData")]
    public class GameLevelData : LevelData
    {
        public GravityType gravityType;
    }

    public enum GravityType
    {
        OnTheFloor,
        Any
    }
}
