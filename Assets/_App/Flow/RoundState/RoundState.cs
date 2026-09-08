using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using UnityEngine;
using DigitalLove.DataAccess;
using Reflex.Attributes;
using DigitalLove.Game.Balls;
using DigitalLove.Game.UI;

namespace DigitalLove.Game
{
    public class RoundState : BaseState
    {
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private BallsSpawner ballsSpawner;
        [SerializeField] private WallStackSpawner wallStackSpawner;
        [SerializeField] private RoundEventsHelper roundEventsHelper;

        [Header("Checkers")]
        [SerializeField] private RoundCountdownChecker countdownChecker;
        [SerializeField] private RoundScoreChecker scoreChecker;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;
        private bool isCountdownLevel;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            countdownChecker.SetOnComplete(OnComplete);
            scoreChecker.SetOnComplete(OnComplete);
        }

        public override void Enter()
        {
            basketSpawner.scored += OnScored;
            ballsSpawner.ballThrown += OnBallThrown;

            round = memoryDataClient.Get<Round>();
            basketSpawner.Basket.SetTriggerActive(true);
            GameLevelData levelData = levelSelector.Current;
            isCountdownLevel = levelData.isCountdownLevel;
            BaseRoundChecker checker = isCountdownLevel ? countdownChecker : scoreChecker;
            checker.DoStart(levelData);
        }

        private void OnScored()
        {
            roundEventsHelper.SendHasScoredEvent();
        }

        private void OnBallThrown()
        {
            round.AddThrow();
            if (isCountdownLevel)
                return;

            round.OpenThrow();
            wallStackSpawner.Panel.SetLeftLabel(round.RemainingMakes);
        }

        private void OnComplete()
        {
            ToNextState();
        }

        public override void Exit()
        {
            basketSpawner.scored -= OnScored;
            ballsSpawner.ballThrown -= OnBallThrown;
        }
    }
}
