using System.Collections;
using DigitalLove.DataAccess;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.Modifiers;
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
        [SerializeField] private ModifiersSpawner modifiersSpawner;
        [SerializeField] private StreakCounter streakCounter;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;
        private int countdown;

        public override void DoStart(GameLevelData levelData)
        {
            basketSpawner.scored += OnBasketScored;
            modifiersSpawner.scored += OnMultiplierScored;

            round = memoryDataClient.Get<Round>();
            streakCounter.Reset();
            if (levelData.HasModifiers)
                modifiersSpawner.DoStart(levelData.modifiers);
            StartCountdown();
        }

        private void OnBasketScored(int score)
        {
            int realScore = DoScore(score);
            basketSpawner.ShowScore(realScore, streakCounter.IsInStreak);
        }

        private void OnMultiplierScored(int score)
        {
            int realScore = DoScore(score);
            modifiersSpawner.ShowScore(realScore, streakCounter.IsInStreak);
        }

        private int DoScore(int score)
        {
            streakCounter.IncrementStreak();
            int realScore = (int)(score * streakCounter.CurrentStreakMultiplier);
            round.score += realScore;
            scoreboardSpawner.Panel.SetScore(realScore);
            return realScore;
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
                    scoreboardSpawner.Panel.SetTime(countdown);
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                scoreboardSpawner.Panel.SetTime(countdown);
                OnComplete();
            }
            StartCoroutine(CoundownRoutine());
        }

        private void OnComplete()
        {
            basketSpawner.scored -= OnBasketScored;
            modifiersSpawner.scored -= OnMultiplierScored;

            modifiersSpawner.DoStop();

            onComplete();
        }
    }
}