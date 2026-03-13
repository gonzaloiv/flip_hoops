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

namespace DigitalLove.Game
{
    public class RoundCompleteState : BaseState
    {
        private int RoundCompleteSecs = 5;

        [SerializeField] private string tableName = "Levels";
        [SerializeField] private GravitySpawner gravitiesBehaviour;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private StringValue highestScoreKey;

        [Inject] private MemoryDataClient memoryDataClient;
        [Inject] private UnityPlayerDataClient unityPlayerDataClient;

        private Play play;
        private bool isHighestScore;

        public override void Enter()
        {
            play = memoryDataClient.Get<Play>();
            ShowUI();
            ballSpawner.Unspawn();
            SaveData();
            CountDown();
            play.IncreaseTries();
        }

        private void ShowUI()
        {
            string initText = LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_title", play.RoundLabelValue());
            string infoText = LocalizationUtil.GetValue(tableName: tableName, "default_round_complete_info");
            basketSpawner.Panel.Show(initText, infoText);
        }

        private async void SaveData()
        {
            isHighestScore = false;
            Round round = memoryDataClient.Get<Round>();
            PlayerData playerData = memoryDataClient.Get<PlayerData>();
            Cookie current = playerData.GetCookieById(highestScoreKey.value);
            if (current == null)
            {
                current = new Cookie(highestScoreKey.value)
                {
                    metadata = round.score.ToString()
                };
                playerData.AddCookie(current);
                isHighestScore = true;
                await unityPlayerDataClient.Put(playerData);
            }
            else
            {
                int highestScore = int.Parse(current.metadata);
                if (highestScore < round.score)
                {
                    isHighestScore = true;
                    current.metadata = round.score.ToString();
                    await unityPlayerDataClient.Put(playerData);
                }
            }
        }

        private void CountDown()
        {
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
            throwZone.Unspawn();
            basketSpawner.Unspawn();
            base.ToNextState();
        }

        public override void Exit()
        {

        }
    }
}