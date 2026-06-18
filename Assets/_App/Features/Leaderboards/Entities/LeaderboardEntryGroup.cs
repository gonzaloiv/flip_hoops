using System.Collections.Generic;
using System.Linq;
using Unity.Services.Leaderboards.Models;
using System;
using Newtonsoft.Json;

namespace DigitalLove.DataAccess.Leaderboards
{
    [Serializable]
    public class LeaderboardEntryGroup
    {
        public List<LeaderboardEntry> entries = new();

        public bool HasAnyValid => entries.Count > 0 && entries.All(entry => entry.IsValid);
    }

    public static class LeaderboardEntryGroupExtensions
    {
        public static LeaderboardEntryGroup ToLeaderboardEntryGroup(this LeaderboardScoresPage page)
        {
            LeaderboardEntryGroup group = new LeaderboardEntryGroup();

            group.entries = page.Results.Select(entry =>
            {
                string playerName = JsonConvert.DeserializeObject<LeaderboardEntry>(entry.Metadata).playerName;
                return new LeaderboardEntry { playerName = playerName, score = entry.Score };
            }).Where(entry => entry != null).ToList();
            return group;
        }
    }
}
