using System.Collections;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Global;
using DigitalLove.Localization;
using Reflex.Attributes;
using UnityEngine;
using DigitalLove.Audio;
using DigitalLove.Casual.Analytics;
using DigitalLove.Casual.Levels;
using DigitalLove.Game.Levels;
using System.Threading.Tasks;

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

        [Inject] private MemoryDataClient memoryDataClient;
        [Inject] private UnityPlayerDataClient unityPlayerDataClient;

        private GameLevelData levelData;
        private Play play;

        public override async void Enter()
        {
            levelData = levelSelector.GetCurrent();
            play = memoryDataClient.Get<Play>();
            ballSpawner.Unspawn();
            bool isHighestScore = await CheckScore();
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