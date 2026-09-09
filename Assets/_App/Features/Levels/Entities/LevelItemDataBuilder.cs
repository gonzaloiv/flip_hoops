using System.Collections.Generic;
using DigitalLove.Casual.Levels;
using DigitalLove.DataAccess;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;

namespace DigitalLove.Game
{
    public static class LevelItemDataBuilder
    {
        public static List<LevelItemData> Build(LevelSelector levelSelector, PlayerData playerData)
        {
            List<GameLevelData> levels = levelSelector.GetLevelsInOrder();
            string frontierId = levelSelector.GetProgressionFrontierLevelId();
            string selectedId = levelSelector.HasPlayCursor
                ? levelSelector.Current.id
                : frontierId;
            List<LevelCompleteCookie> cookies = playerData.GetLevelCompleteCookies();
            List<LevelItemData> entries = new();
            for (int i = 0; i < levels.Count; i++)
                entries.Add(BuildEntry(levelSelector, levels[i], i, frontierId, selectedId, cookies));
            return entries;
        }

        private static LevelItemData BuildEntry(
            LevelSelector levelSelector,
            GameLevelData level,
            int index,
            string frontierId,
            string selectedId,
            List<LevelCompleteCookie> cookies)
        {
            LevelCompleteCookie cookie = cookies.GetLevelIdCookie(level.id);
            bool passed = cookie != null;
            int stars = 0;
            string scoreText = string.Empty;
            if (passed)
                FillPassedScore(level, cookie, out stars, out scoreText);

            return new LevelItemData
            {
                levelId = level.id,
                identityLabel = $"{index + 1:00}",
                scoreText = scoreText,
                stars = stars,
                passed = passed,
                selected = string.Equals(level.id, selectedId),
                frontier = string.Equals(level.id, frontierId),
                locked = !levelSelector.IsPressable(level.id)
            };
        }

        private static void FillPassedScore(
            GameLevelData level,
            LevelCompleteCookie cookie,
            out int stars,
            out string scoreText)
        {
            stars = 0;
            scoreText = string.Empty;
            if (level.isCountdownLevel)
            {
                scoreText = cookie.metadata ?? string.Empty;
                return;
            }

            LevelClearBests bests = LevelClearBestsCodec.ParseScoreMode(cookie.metadata);
            if (!bests.HasValue)
                return;

            stars = bests.Stars;
            scoreText = bests.Points.ToString();
        }
    }
}
