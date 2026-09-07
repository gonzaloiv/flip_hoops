using DigitalLove.DataAccess;
using DigitalLove.Game;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundWarmUpChecker : BaseRoundChecker
    {
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;

        public override void DoStart(GameLevelData levelData)
        {
            round = memoryDataClient.Get<Round>();
            basketSpawner.scored += OnScored;
            round.AddScore(levelSelector.GetCurrent().basketsToScore);
        }

        private void OnScored(int value)
        {
            round.AddScore(-1);
            scoreboardSpawner.Panel.SetScore(round.Score);
            if (round.Score <= 0)
                OnComplete();
        }

        [Button]
        public void CompleteRound()
        {
            OnScored(999);
        }

        private void OnComplete()
        {
            basketSpawner.scored -= OnScored;
            onComplete();
        }
    }
}