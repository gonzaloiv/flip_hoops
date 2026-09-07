using System.Collections;
using DigitalLove.DataAccess;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
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
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;
        private int countdown;

        public override void DoStart(GameLevelData levelData)
        {
            basketSpawner.scored += OnBasketScored;

            round = memoryDataClient.Get<Round>();
            StartCountdown();
        }

        private void OnBasketScored(int score)
        {
            round.AddScore(score);
            scoreboardSpawner.Panel.SetRightLabel(score);
            basketSpawner.ShowScore(score, false);
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
                    scoreboardSpawner.Panel.SetLeftLabel(countdown);
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                scoreboardSpawner.Panel.SetLeftLabel(countdown);
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