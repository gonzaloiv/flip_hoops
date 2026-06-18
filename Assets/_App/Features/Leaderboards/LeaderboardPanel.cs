using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using DigitalLove.UI.DesignSystem;
using System;

namespace DigitalLove.DataAccess.Leaderboards
{
    public class LeaderboardPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] labels;
        [SerializeField] private Canvas canvas;
        [SerializeField] private LayoutUpdater layoutUpdater;

        public UnityEvent onShow;

        private LeaderboardsClient leaderboardsClient = new();

        public void Show()
        {
            StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            Task<LeaderboardEntryGroup> task = leaderboardsClient.GetGlobalScoresPageAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to load global leaderboard: {task.Exception?.GetBaseException()}");
                Hide();
                yield break;
            }

            LeaderboardEntryGroup group = task.Result;
            if (group.HasAnyValid)
            {
                onShow.Invoke();
                UpdateLabels(group);
                canvas.enabled = true;
                layoutUpdater.ForceUpdate();
            }
            else
            {
                Hide();
            }
        }

        private void UpdateLabels(LeaderboardEntryGroup group)
        {
            int index = 0;
            Array.ForEach(labels, label => label.gameObject.SetActive(false));
            foreach (LeaderboardEntry entry in group.entries)
            {
                if (index >= labels.Length)
                    break;
                labels[index].text = entry.playerName + " - " + entry.score;
                labels[index].gameObject.SetActive(true);
                index++;
            }
        }

        public void Hide()
        {
            canvas.enabled = false;
        }
    }
}
