using System.Linq;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.Game.Levels;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private GameLevelData[] firstSessionLevels;
        [SerializeField] private GameLevelData[] levels;

        [Header("Debug")]
        [SerializeField] private GameLevelData current;

        [Inject] private MemoryDataClient memoryDataClient;

        public GameLevelData GetCurrent()
        {
            GameLevelData[] data = memoryDataClient.Get<PlayerData>().IsFirstSession ? firstSessionLevels : levels;
            int roundIndex = memoryDataClient.Get<Play>().Tries;
            if (data.Length <= roundIndex)
            {
                current = data.Last();
            }
            else
            {
                current = data[roundIndex];
            }
            return current;
        }
    }
}