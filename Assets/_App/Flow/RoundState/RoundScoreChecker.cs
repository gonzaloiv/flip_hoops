using DigitalLove.DataAccess;
using DigitalLove.Game;
using DigitalLove.Game.Basket;
using DigitalLove.Game.UI;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove
{
    public class RoundScoreChecker : BaseRoundChecker
    {
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;

        public override void DoStart()
        {
            round = memoryDataClient.Get<Round>();
            basketSpawner.scored += OnScored;
            round.score = levelSelector.GetCurrent().basketsToScore;
        }

        private void OnScored(int value)
        {
            round.score--;
            scoreboardSpawner.Panel.SetScore(round.score);
            if (round.score <= 0)
                OnComplete();
        }

        [Button]
        public void CompleteRound()
        {
            round.score = 1;
            OnScored(2);
        }

        private void OnComplete()
        {
            basketSpawner.scored -= OnScored;
            onComplete();
        }
    }
}