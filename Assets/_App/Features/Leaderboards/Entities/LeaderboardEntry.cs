using System;

namespace DigitalLove.DataAccess.Leaderboards
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public double score;

        public bool IsValid => !string.IsNullOrEmpty(playerName) && score > 0;
    }
}