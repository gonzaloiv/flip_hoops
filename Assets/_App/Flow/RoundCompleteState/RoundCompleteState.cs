using System.Collections;
using DigitalLove.Audio;
using DigitalLove.Casual.Analytics;
using DigitalLove.Casual.Flow;
using DigitalLove.Casual.Levels;
using DigitalLove.DataAccess;
using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using DigitalLove.Global;
using DigitalLove.Localization;
using Reflex.Attributes;
using UnityEngine;

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
        [SerializeField] private LevelsPanel levelsPanel;

        [Inject] private MemoryDataClient memoryDataClient;
        [Inject] private UnityPlayerDataClient unityPlayerDataClient;

        private GameLevelData levelData;
        private Play play;

        public override void Enter()
        {
            levelData = levelSelector.Current;
            play = memoryDataClient.Get<Play>();
            ballSpawner.Unspawn();
            bool wasAlreadyPassed = HasPassCookie(levelData.id);
            bool isHighestScore = SetNewScore(levelData);
            ApplyPlayCursorAfterRound(wasAlreadyPassed);
            levelsPanel?.Refresh(LevelItemDataBuilder.Build(levelSelector, memoryDataClient.Get<PlayerData>()));
            UpdatePlayerData();
            ShowUI(isHighestScore);
            CountDown();
        }

        private bool HasPassCookie(string levelId) =>
            memoryDataClient.Get<PlayerData>().GetLevelCompleteCookies().HasLevelIdCookie(levelId);

        private bool SetNewScore(GameLevelData levelData)
        {
            PlayerData playerData = memoryDataClient.Get<PlayerData>();
            string cookieId = new LevelCompleteCookie(levelData.id).id;
            int score = memoryDataClient.Get<Round>().Score;
            Cookie stored = playerData.GetCookieById(cookieId);
            if (stored == null)
            {
                playerData.AddCookie(new LevelCompleteCookie(levelData.id).SetMetadata(score.ToString()));
                return true;
            }

            if (string.IsNullOrEmpty(stored.metadata) || int.Parse(stored.metadata) <= score)
            {
                stored.metadata = score.ToString();
                return true;
            }
            return false;
        }

        private void ApplyPlayCursorAfterRound(bool wasAlreadyPassed)
        {
            if (!wasAlreadyPassed)
                levelSelector.AdvancePlayCursorToFollowing();
        }

        private async void UpdatePlayerData()
        {
            await unityPlayerDataClient.Put(memoryDataClient.Get<PlayerData>());
        }

        private void ShowUI(bool isHighestScore)
        {
            string initText = LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_title", play.RoundLabelValue());
            string infoText = !isHighestScore
                ? LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_info")
                : LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_highest_score");
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
