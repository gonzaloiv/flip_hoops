using System.Collections.Generic;
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

        [Inject] private MemoryDataClient memoryDataClient;

        public bool HasPlayCursor => !string.IsNullOrEmpty(currentLevelId);
        public int TotalLevels => chapters.GetTotalLevelsCount();
        public int CurrentLevelIndex => chapters.GetLevelIndex(currentLevelId);

        public GameLevelData Current
        {
            get
            {
                Assert.IsTrue(HasPlayCursor, "Play cursor is not set");
                Assert.IsNotNull(chapters, "Chapters are not set");
                return chapters.GetLevelData<GameLevelData>(currentLevelId);
            }
        }

        public List<GameLevelData> GetLevelsInOrder() => chapters.GetLevelsInOrder<GameLevelData>();

        public void SetPlayCursor(string levelId)
        {
            Assert.IsFalse(string.IsNullOrEmpty(levelId), "Level id is empty");
            Assert.IsNotNull(chapters.GetLevelDataById(levelId), $"Unknown level id {levelId}");
            currentLevelId = levelId;
        }

        public void SeedPlayCursorFromCookies() => currentLevelId = ResolveProgressionFrontierLevelId();

        public void SetCurrentPlayerLevelId() => SeedPlayCursorFromCookies();

        public string GetProgressionFrontierLevelId() => ResolveProgressionFrontierLevelId();

        public bool IsPressable(string levelId)
        {
            if (GetLevelCompleteCookies().HasLevelIdCookie(levelId))
                return true;
            return string.Equals(levelId, GetProgressionFrontierLevelId());
        }

        public void AdvancePlayCursorToFollowing()
        {
            Assert.IsTrue(HasPlayCursor, "Play cursor is not set");
            LevelData following = chapters.GetFollowingLevelData(currentLevelId);
            currentLevelId = following != null ? following.id : chapters[0].levels[0].id;
        }

        public void SetRandom()
        {
            currentLevelId = chapters.GetRandomLevelData<GameLevelData>().id;
        }

        private string ResolveProgressionFrontierLevelId()
        {
            LevelCompleteCookie last = GetLevelCompleteCookies().GetLastLevelCookie();
            string lastId = last != null && last.IsValid ? last.LevelId : string.Empty;
            if (string.IsNullOrEmpty(lastId) || !chapters.AreThereMoreLevels(chapters.GetLevelIndex(lastId)))
                return chapters[0].levels[0].id;

            LevelData following = chapters.GetFollowingLevelData(lastId);
            return following != null ? following.id : chapters[0].levels[0].id;
        }

        private List<LevelCompleteCookie> GetLevelCompleteCookies() =>
            memoryDataClient.Get<PlayerData>().GetLevelCompleteCookies();
    }
}
