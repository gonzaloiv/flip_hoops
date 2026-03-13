using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Levels;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundState : BaseState
    {
        [SerializeField] private LevelSelector levelSelector;

        [SerializeField] private RoundCountdownChecker countdownChecker;
        [SerializeField] private RoundScoreChecker scoreChecker;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            round = new Round();
            memoryDataClient.Put(round);
            countdownChecker.SetOnComplete(OnComplete);
            scoreChecker.SetOnComplete(OnComplete);
        }

        public override void Enter()
        {
            round.Reset();
            Play play = memoryDataClient.Get<Play>();
            GameLevelData levelData = levelSelector.GetCurrent(play.RoundCount());
            BaseRoundChecker checker = levelData.IsWarmUp ? scoreChecker : countdownChecker;
            checker.DoStart();
        }

        private void OnComplete()
        {
            ToNextState();
        }

        public override void Exit()
        {

        }
    }
}