using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.Modifiers;
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
            checker.DoStart(levelData);
        }

        private void OnScored(int score)
        {
            roundEventsHelper.SendHasScoredEvent();
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