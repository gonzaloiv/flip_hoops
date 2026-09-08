using DigitalLove.Casual.Levels;
using DigitalLove.DataAccess;
using DigitalLove.Game.Levels;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace DigitalLove.Game
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private ChapterData[] chapters;

        private string currentLevelId;

        public int TotalLevels => chapters.GetTotalLevelsCount();

        [Inject] private MemoryDataClient memoryDataClient;

        public GameLevelData Current
        {
            get
            {
                Assert.IsNotNull(chapters, "Chapters are not set");
                return chapters.GetLevelData<GameLevelData>(currentLevelId);
            }
        }

        public int CurrentLevelIndex => chapters.GetLevelIndex(currentLevelId);

        public void SetCurrentPlayerLevelId()
        {
            LevelCompleteCookie lastLevelCompleteCookie = memoryDataClient.Get<PlayerData>().GetLevelCompleteCookies().GetLastLevelCookie();
            string lastLevelCompleteId = lastLevelCompleteCookie != null && lastLevelCompleteCookie.IsValid ? lastLevelCompleteCookie.LevelId : string.Empty;
            if (!chapters.AreThereMoreLevels(chapters.GetLevelIndex(lastLevelCompleteId)) || string.IsNullOrEmpty(lastLevelCompleteId))
            {
                currentLevelId = chapters[0].levels[0].id;
                return;
            }

            currentLevelId = chapters.GetFollowingLevelData(lastLevelCompleteId).id;
        }

        // ? Mainly for debug reasons
        public void SetRandom()
        {
            currentLevelId = chapters.GetRandomLevelData<GameLevelData>().id;
        }
    }
}
