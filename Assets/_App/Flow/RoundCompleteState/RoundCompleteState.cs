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
using DigitalLove.Global;
using DigitalLove.XR;
using DigitalLove.Casual.Levels;

namespace DigitalLove.Game
{
    public class RoundCompleteState : BaseState
    {
        private int RoundCompleteSecs = 5;

        [SerializeField] private string tableName = "Levels";
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private StringValue levelCompleteKey;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ProgressionEventsHelper progressionEventsHelper;
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private MetaPlatformClient metaPlatformClient;

        [Inject] private MemoryDataClient memoryDataClient;
        [Inject] private UnityPlayerDataClient unityPlayerDataClient;

        private GameLevelData levelData;
        private Play play;

        public override void Enter()
        {
            levelData = levelSelector.Current;
            play = memoryDataClient.Get<Play>();
            ballSpawner.Unspawn();
            bool isHighestScore = SetNewScore(levelData);
            UpdatePlayerData();
            ShowUI(isHighestScore);
            CountDown();
        }

        private bool SetNewScore(GameLevelData levelData)
        {
            PlayerData playerData = memoryDataClient.Get<PlayerData>();
            var levelCompleteCookie = new LevelCompleteCookie(levelData.id);
            LevelCompleteCookie previousCookie = null;

            // Find if this LevelCompleteCookie already exists
            foreach (var cookie in playerData.GetLevelCompleteCookies())
            {
                if (cookie.LevelId == levelData.id)
                {
                    previousCookie = cookie;
                    break;
                }
            }

            int score = memoryDataClient.Get<Round>().Score;

            if (previousCookie == null)
            {
                levelCompleteCookie.metadata = score.ToString();
                playerData.AddCookie(levelCompleteCookie);
                return true;
            }
            else if (int.Parse(previousCookie.metadata) <= score)
            {
                previousCookie.metadata = score.ToString();
                return true;
            }
            return false;
        }

        private async void UpdatePlayerData()
        {
            await unityPlayerDataClient.Put(memoryDataClient.Get<PlayerData>());
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