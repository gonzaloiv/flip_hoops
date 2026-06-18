using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using DigitalLove.Global;

namespace DigitalLove.DataAccess.Leaderboards
{
    public class LeaderboardDebugUtil : MonoBehaviour
    {
        [SerializeField] private LeaderboardEntry entryToAdd;
        [SerializeField] private LeaderboardEntryGroup loadedEntries;
        [SerializeField] private LeaderboardPanel leaderboardPanel;

        private LeaderboardsClient leaderboardsClient = new();

        [Button]
        public void AddEntryToGlobalLeaderboard()
        {
            StartCoroutine(AddEntryRoutine());
        }

        [Button]
        public void LoadEntriesFromGlobalLeaderboard()
        {
            StartCoroutine(LoadEntriesRoutine());
        }

        private IEnumerator AddEntryRoutine()
        {
            Task<bool> task = leaderboardsClient.AddScoreToGlobalLeaderboardAsync(entryToAdd);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception?.GetBaseException());
                yield break;
            }

            if (task.Result)
            {
                Debug.LogWarning("Entry added to global leaderboard");
                if (leaderboardPanel != null)
                    leaderboardPanel.Show();
            }
        }

        private IEnumerator LoadEntriesRoutine()
        {
            Task<LeaderboardEntryGroup> task = leaderboardsClient.GetGlobalScoresPageAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception?.GetBaseException());
                yield break;
            }

            if (task.Result != null && task.Result.HasAnyValid)
            {
                loadedEntries = task.Result;
                if (leaderboardPanel != null)
                    leaderboardPanel.Show();
            }
        }
    }
}
