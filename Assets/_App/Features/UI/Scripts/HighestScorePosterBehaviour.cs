using DigitalLove.DataAccess;
using DigitalLove.Global;
using DigitalLove.XR.MRUtilityKit;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class HighestScorePosterBehaviour : MonoBehaviour
    {
        [SerializeField] private OnTheWallSpawner onTheWallSpawner;
        [SerializeField] private StringValue highestScoreKey;
        [SerializeField] private TextMeshProUGUI scoreLabel;

        [Inject] private MemoryDataClient memoryDataClient;

        public void Show()
        {
            if (HasHighestScore(out int score))
            {
                scoreLabel.text = $"- {score} -";
                if (!onTheWallSpawner.HasBeenSpawned)
                    onTheWallSpawner.Spawn();
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private bool HasHighestScore(out int score)
        {
            PlayerData playerData = memoryDataClient.Get<PlayerData>();
            Cookie current = playerData.GetCookieById(highestScoreKey.value);
            if (current == null)
            {
                score = 0;
                return false;
            }
            else
            {
                int value = int.Parse(current.metadata);
                score = value;
                return score <= 0 ? false : true;
            }
        }
    }
}