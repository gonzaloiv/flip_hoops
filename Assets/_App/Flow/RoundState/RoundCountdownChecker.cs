using System;
using System.Collections;
using DigitalLove.DataAccess;
using DigitalLove.Game;
using DigitalLove.Game.Basket;
using DigitalLove.Game.UI;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove
{
    public class RoundCountdownChecker : BaseRoundChecker
    {
        private const int RoundSecs = 44;

        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private int countdown;

        public override void DoStart()
        {
            basketSpawner.scored -= OnScored;

            StartCountdown();
        }

        private void StartCountdown()
        {
            countdown = RoundSecs;
            IEnumerator CoundownRoutine()
            {
                while (countdown > 0)
                {
                    scoreboardSpawner.Panel.SetTime(countdown);
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                scoreboardSpawner.Panel.SetTime(countdown);
                OnComplete();
            }
            StartCoroutine(CoundownRoutine());
        }

        private void OnScored()
        {
            Round round = memoryDataClient.Get<Round>();
            round.score++;
            scoreboardSpawner.Panel.SetScore(round.score);
        }

        [Button]
        public void CompleteRound() => countdown = 0;

        private void OnComplete()
        {
            basketSpawner.scored -= OnScored;

            onComplete();
        }
    }
}