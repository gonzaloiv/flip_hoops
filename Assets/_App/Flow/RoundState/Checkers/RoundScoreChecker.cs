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
    public class RoundScoreChecker : BaseRoundChecker
    {
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ObstacleSpawner obstacleSpawner;
        [SerializeField] private BankShotRejectFeedback rejectFeedback;
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private WallStackSpawner wallStackSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;

        public override void DoStart(GameLevelData levelData)
        {
            round = memoryDataClient.Get<Round>();
            basketSpawner.scored += OnScored;
            round.SeedRemainingMakes(GameLevelData.BasketsToScore);
            RefreshHud();
        }

        private void OnScored()
        {
            if (obstacleSpawner != null && !obstacleSpawner.CanCreditMake())
            {
                rejectFeedback?.PlayReject();
                return;
            }

            round.ResolveActiveThrow();
            round.DecrementRemainingMakes();
            RefreshHud();
            if (round.RemainingMakes <= 0)
                OnComplete();
        }

        [Button]
        public void CompleteRound()
        {
            round.SeedRemainingMakes(1);
            OnScored();
        }

        private void RefreshHud()
        {
            wallStackSpawner.Panel.SetLeftLabel(round.RemainingMakes);
            wallStackSpawner.Panel.SetRightLabel(round.Score);
        }

        private void OnComplete()
        {
            basketSpawner.scored -= OnScored;
            onComplete();
        }
    }
}
