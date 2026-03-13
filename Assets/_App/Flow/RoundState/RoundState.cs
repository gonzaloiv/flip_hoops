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

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            countdownChecker.SetOnComplete(OnComplete);
            scoreChecker.SetOnComplete(OnComplete);
        }

        public override void Enter()
        {
            memoryDataClient.Put(new Round());
            GameLevelData levelData = levelSelector.GetCurrent();
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