using System.Collections.Generic;
using DigitalLove.DataAccess;
using DigitalLove.Game.BankShot;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Furniture;
using DigitalLove.Game.Levels;
using DigitalLove.Game.Modifiers;
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
        [SerializeField] private ModifierSpawner modifierSpawner;
        [SerializeField] private FurnitureSpawner furnitureSpawner;
        [SerializeField] private BankShotRejectFeedback rejectFeedback;
        [SerializeField] private WallStackSpawner wallStackSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;
        private readonly List<ThrowScoreOp> scoreOps = new();

        public override void DoStart(GameLevelData levelData)
        {
            round = memoryDataClient.Get<Round>();
            basketSpawner.scored += OnScored;
            round.SeedRemainingMakes(GameLevelData.BasketsToScore);
            RefreshHud();
        }

        private void OnScored()
        {
            if (!CanCreditMake())
            {
                rejectFeedback?.PlayReject();
                return;
            }

            ApplyModifierScoreOps();
            int points = round.ResolveActiveThrow();
            round.DecrementRemainingMakes();
            RefreshHud();
            if (scoreOps.Count > 0)
                basketSpawner.ShowScore(points, true);

            if (round.RemainingMakes <= 0)
                OnComplete();
        }

        private bool CanCreditMake() =>
            ObligatoryActivatableExtensions.CanCreditMake(
                obstacleSpawner != null ? obstacleSpawner.Spawned : null,
                modifierSpawner != null ? modifierSpawner.Spawned : null);

        private void ApplyModifierScoreOps()
        {
            scoreOps.Clear();
            if (modifierSpawner != null)
                modifierSpawner.CopyActivationOrderScoreOps(scoreOps);
            if (furnitureSpawner != null)
                furnitureSpawner.CopyActivationOrderScoreOps(scoreOps);

            if (scoreOps.Count == 0)
                return;

            round.ApplyActiveThrowOps(scoreOps);
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
