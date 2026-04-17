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

        [Inject] private MemoryDataClient memoryDataClient;

        public GameLevelData GetCurrent()
        {
            GameLevelData[] data = memoryDataClient.Get<PlayerData>().IsFirstSession ? firstSessionLevels : levels;
            int roundIndex = memoryDataClient.Get<Play>().Tries;
            if (data.Length <= roundIndex)
            {
                return data.Last();
            }
            else
            {
                return data[roundIndex];
            }
        }
    }
}