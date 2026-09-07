using DigitalLove.Casual.Flow;
using DigitalLove.Casual.Levels;
using DigitalLove.DataAccess;
using DigitalLove.Game.Levels;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private ChapterData[] chapters;

        [Header("Debug")]
        [SerializeField] private GameLevelData current;

        private string currentLevelId;

        [Inject] private MemoryDataClient memoryDataClient;

        public void SetCurrentLevelId(string levelId)
        {
            if (!string.IsNullOrEmpty(levelId))
            {
                currentLevelId = levelId;
            }
            else
            {
                currentLevelId = chapters[0].levels[0].id;
            }
        }

        public bool SetNextLevelId()
        {
            int levelIndex = chapters.GetLevelIndex(currentLevelId);
            if (chapters.AreThereMoreLevels(levelIndex))
            {
                currentLevelId = chapters.GetFollowingLevelData(currentLevelId).id;
                return true;
            }
            return false;
        }

        public void SetCurrentPlayerLevelId()
        {
            LevelCompleteCookie lastLevelCompleteCookie = memoryDataClient.Get<PlayerData>().GetLevelCompleteCookies().GetLastLevelCookie();
            string levelId = lastLevelCompleteCookie != null && lastLevelCompleteCookie.IsValid ? lastLevelCompleteCookie.LevelId : string.Empty;
            SetCurrentLevelId(levelId);
        }

        public GameLevelData GetCurrent()
        {
            return chapters.GetLevelData<GameLevelData>(currentLevelId);
        }
    }
}
