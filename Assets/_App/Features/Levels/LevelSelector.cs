using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.Game.Levels;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private GameLevelData[] levels;

        [Header("Debug")]
        [SerializeField] private GameLevelData current;

        [Inject] private MemoryDataClient memoryDataClient;

        public GameLevelData GetCurrent()
        {
            int roundIndex = memoryDataClient.Get<Play>().Tries;
            if (levels.Length <= roundIndex)
                current = levels[levels.Length - 1];
            else
                current = levels[roundIndex];
            return current;
        }
    }
}
