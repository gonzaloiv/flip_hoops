using DigitalLove.Casual.Flow;
using DigitalLove.Casual.Levels;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundState : BaseState
    {
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private RoundEventsHelper roundEventsHelper;

        [Header("Checkers")]
        [SerializeField] private RoundCountdownChecker countdownChecker;
        [SerializeField] private RoundScoreChecker scoreChecker;

        [Inject] private MemoryDataClient memoryDataClient;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            countdownChecker.SetOnComplete(OnComplete);
            scoreChecker.SetOnComplete(OnComplete);
        }

        public override void Enter()
        {
            basketSpawner.scored += OnScored;

            basketSpawner.Basket.SetTriggerActive(true);
            GameLevelData levelData = levelSelector.GetCurrent();
            BaseRoundChecker checker = levelData.IsWarmUp ? scoreChecker : countdownChecker;
            checker.DoStart();
        }

        private void OnScored(int score)
        {
            Play play = memoryDataClient.Get<Play>();
            LevelData levelData = levelSelector.GetCurrent();
            roundEventsHelper.SendHasScoredEvent(levelData.GetIdWithRound(play));
        }

        private void OnComplete()
        {
            ToNextState();
        }

        public override void Exit()
        {
            basketSpawner.scored -= OnScored;
        }
    }
}