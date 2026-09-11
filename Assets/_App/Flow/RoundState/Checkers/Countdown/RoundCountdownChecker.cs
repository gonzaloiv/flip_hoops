using System.Collections;
using System.Collections.Generic;
using DigitalLove.DataAccess;
using DigitalLove.Game.BankShot;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.Modifiers;
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
        [SerializeField] private ModifierSpawner modifierSpawner;
        [SerializeField] private BankShotRejectFeedback rejectFeedback;
        [SerializeField] private WallStackSpawner wallStackSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;
        private int countdown;
        private readonly List<ThrowScoreOp> scoreOps = new();

        public override void DoStart(GameLevelData levelData)
        {
            basketSpawner.scored += OnBasketScored;

            round = memoryDataClient.Get<Round>();
            StartCountdown();
        }

        private void OnBasketScored()
        {
            if (!CanCreditMake())
            {
                rejectFeedback?.PlayReject();
                return;
            }

            scoreOps.Clear();
            if (modifierSpawner != null)
                modifierSpawner.CopyActivationOrderScoreOps(scoreOps);

            int points = round.CreditCountdownMake(scoreOps);
            wallStackSpawner.Panel.SetRightLabel(round.Score);
            basketSpawner.ShowScore(points, scoreOps.Count > 0);
        }

        private bool CanCreditMake()
        {
            bool obstaclesOk = obstacleSpawner == null || obstacleSpawner.CanCreditMake();
            bool modifiersOk = modifierSpawner == null || modifierSpawner.CanCreditMake();
            return obstaclesOk && modifiersOk;
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
