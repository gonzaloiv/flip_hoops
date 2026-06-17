using System.Collections;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Localization;
using Reflex.Attributes;
using UnityEngine;
using DigitalLove.Audio;
using DigitalLove.Casual.Analytics;
using DigitalLove.Game.Levels;
using System.Threading.Tasks;
using DigitalLove.DataAccess.Leaderboards;
using DigitalLove.Global;
using DigitalLove.XR;

namespace DigitalLove.Game
{
    public class RoundCompleteState : BaseState
    {
        private int RoundCompleteSecs = 5;

        [SerializeField] private string tableName = "Levels";
        [SerializeField] private GravitySelector gravitiesBehaviour;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private StringValue highestScoreKey;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ProgressionEventsHelper progressionEventsHelper;
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private LeaderboardPanel leaderboardPanel;
        [SerializeField] private MetaPlatformClient metaPlatformClient;

        [Inject] private MemoryDataClient memoryDataClient;
        [Inject] private UnityPlayerDataClient unityPlayerDataClient;

        private GameLevelData levelData;
        private Play play;
        private LeaderboardsClient leaderboardsClient = new();

        public override async void Enter()
        {
            levelData = levelSelector.GetCurrent();
            play = memoryDataClient.Get<Play>();
            ballSpawner.Unspawn();
            bool isHighestScore = await CheckScore();
            UpdateLeaderboard(isHighestScore);
            ShowUI(isHighestScore);
            CountDown();
        }

        private async Task<bool> CheckScore()
        {
            Round round = memoryDataClient.Get<Round>();
            progressionEventsHelper.SendLevelCompleteEvent(levelId: levelData.GetIdWithRound(play), score: round.score);
            if (levelData.IsWarmUp)
                return false;
            PlayerData playerData = memoryDataClient.Get<PlayerData>();
            Cookie previousCookie = playerData.GetCookieById(highestScoreKey.value);
            if (previousCookie == null)
            {
                previousCookie = new(highestScoreKey.value) { metadata = round.score.ToString() };
                playerData.AddCookie(previousCookie);
                await unityPlayerDataClient.Put(playerData);
                return true;
            }
            else if (int.Parse(previousCookie.metadata) <= round.score)
            {
                previousCookie.metadata = round.score.ToString();
                await unityPlayerDataClient.Put(playerData);
                return true;
            }
            return false;
        }

        private void UpdateLeaderboard(bool isHighestScore)
        {
            if (!isHighestScore)
                return;
            metaPlatformClient.FetchUserDisplayName(OnUserDisplayNameFetched);
            void OnUserDisplayNameFetched(string displayName)
            {
                if (string.IsNullOrEmpty(displayName))
                    return;
                LeaderboardEntry entry = new() { score = memoryDataClient.Get<Round>().score, playerName = displayName };
                StartCoroutine(AddScoreAndShowLeaderboardRoutine(entry));
            }
        }

        private IEnumerator AddScoreAndShowLeaderboardRoutine(LeaderboardEntry entry)
        {
            Task<bool> task = leaderboardsClient.AddScoreToGlobalLeaderboardAsync(entry);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError(task.Exception?.GetBaseException());
                yield break;
            }

            if (task.Result)
                leaderboardPanel.Show();
        }

        private void ShowUI(bool isHighestScore)
        {
            string initText = LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_title", play.RoundLabelValue());
            string infoText = !isHighestScore ? LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_info") :
                LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_highest_score");
            basketSpawner.Panel.Show(initText, infoText);
        }

        private void CountDown()
        {
            this.PlayWithFadeOut(audioSource, RoundCompleteSecs, audioSource.volume);
            int countdown = RoundCompleteSecs;
            IEnumerator CoundownRoutine()
            {
                while (countdown > 0)
                {
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                ToNextState();
            }
            StartCoroutine(CoundownRoutine());
        }

        protected override void ToNextState()
        {
            play.IncreaseTries();
            throwZone.Unspawn();
            basketSpawner.Hide();
            base.ToNextState();
        }

        public override void Exit()
        {

        }
    }
}