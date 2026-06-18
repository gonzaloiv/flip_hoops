using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

namespace DigitalLove.DataAccess.Leaderboards
{
    public class LeaderboardsClient
    {
        private const string GlobalLeaderboardId = "global";
        private const int DefaultScoresPageLimit = 10;

        public async Task<LeaderboardEntryGroup> GetScoresPageAsync(string leaderboardId)
        {
            LeaderboardScoresPage page = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = DefaultScoresPageLimit, IncludeMetadata = true });
            return page.ToLeaderboardEntryGroup();
        }

        public async Task<LeaderboardEntryGroup> GetGlobalScoresPageAsync()
        {
            return await GetScoresPageAsync(GlobalLeaderboardId);
        }

        public async Task<bool> AddScoreToGlobalLeaderboardAsync(LeaderboardEntry entry)
        {
            Unity.Services.Leaderboards.Models.LeaderboardEntry result = await LeaderboardsService.Instance.AddPlayerScoreAsync(
                GlobalLeaderboardId,
                entry.score,
                new AddPlayerScoreOptions
                {
                    Metadata = new Dictionary<string, object> { { "playerName", entry.playerName } }
                });
            return result != null;
        }
    }
}
