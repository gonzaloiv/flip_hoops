using System.Collections;
using DigitalLove.DataAccess;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.Obstacles;
using DigitalLove.Game.UI;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundCountdownChecker : BaseRoundChecker
    {
        private const int RoundSecs = 33;

        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ObstacleSpawner obstacleSpawner;
        [SerializeField] private BankShotRejectFeedback rejectFeedback;
        [SerializeField] private WallStackSpawner wallStackSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;
        private int countdown;

        public override void DoStart(GameLevelData levelData)
        {
            basketSpawner.scored += OnBasketScored;

            round = memoryDataClient.Get<Round>();
            StartCountdown();
        }

        private void OnBasketScored()
        {
            if (obstacleSpawner != null && !obstacleSpawner.CanCreditMake())
            {
                rejectFeedback?.PlayReject();
                return;
            }

            round.AddScore();
            wallStackSpawner.Panel.SetRightLabel(round.Score);
            basketSpawner.ShowScore(round.Score, false);
        }

        [Button]
        public void CompleteRound() => countdown = 0;

        private void StartCountdown()
        {
            countdown = RoundSecs;
            IEnumerator CoundownRoutine()
            {
                while (countdown > 0)
                {
                    wallStackSpawner.Panel.SetLeftLabel(countdown);
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                wallStackSpawner.Panel.SetLeftLabel(countdown);
                OnComplete();
            }
            StartCoroutine(CoundownRoutine());
        }

        private void OnComplete()
        {
            basketSpawner.scored -= OnBasketScored;

            onComplete();
        }
    }
}
