using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;
using UnityLeaderboardEntry = Unity.Services.Leaderboards.Models.LeaderboardEntry;

namespace DigitalLove.DataAccess.Leaderboards
{
    [Serializable]
    public class LeaderboardEntryGroup
    {
        public List<LeaderboardEntry> entries = new();

        public bool HasAnyValid
        {
            get
            {
                if (entries.Count == 0)
                    return false;
                for (int i = 0; i < entries.Count; i++)
                {
                    if (!entries[i].IsValid)
                        return false;
                }
                return true;
            }
        }
    }

    public static class LeaderboardEntryGroupExtensions
    {
        public static LeaderboardEntryGroup ToLeaderboardEntryGroup(this LeaderboardScoresPage page)
        {
            LeaderboardEntryGroup group = new LeaderboardEntryGroup();
            group.entries = MapResults(page.Results);
            return group;
        }

        private static List<LeaderboardEntry> MapResults(List<UnityLeaderboardEntry> results)
        {
            List<LeaderboardEntry> entries = new(results.Count);
            for (int i = 0; i < results.Count; i++)
            {
                UnityLeaderboardEntry entry = results[i];
                string playerName = JsonConvert
                    .DeserializeObject<LeaderboardEntry>(entry.Metadata)
                    .playerName;
                entries.Add(new LeaderboardEntry { playerName = playerName, score = entry.Score });
            }
            return entries;
        }
    }
}
