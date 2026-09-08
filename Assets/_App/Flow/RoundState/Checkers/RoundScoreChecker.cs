using DigitalLove.DataAccess;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundScoreChecker : BaseRoundChecker
    {
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private WallStackSpawner wallStackSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;

        public override void DoStart(GameLevelData levelData)
        {
            round = memoryDataClient.Get<Round>();
            basketSpawner.scored += OnScored;
            round.AddScore(GameLevelData.BasketsToScore);
        }

        private void OnScored()
        {
            round.AddScore(-1);
            wallStackSpawner.Panel.SetRightLabel(round.Score);
            if (round.Score <= 0)
                OnComplete();
        }

        [Button]
        public void CompleteRound()
        {
            round.SetScore(1);
            OnScored();
        }

        private void OnComplete()
        {
            basketSpawner.scored -= OnScored;
            onComplete();
        }
    }
}