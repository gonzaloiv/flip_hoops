using DigitalLove.Casual.Analytics;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Levels;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;
using DigitalLove.Casual.Flow;

namespace DigitalLove.Game
{
    public class CountdownState : BaseState
    {
        [Header("Scene")]
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private CourtSetupHelper courtSetupHelper;
        [SerializeField] private CountdownStateUI ui;

        [Header("Checkers")]
        [SerializeField] private CountdownStateChecker checker;

        [Header("Analytics")]
        [SerializeField] private ProgressionEventsHelper progressionEventsHelper;
        [SerializeField] private RoundEventsHelper roundEventsHelper;

        [Inject] private MemoryDataClient memoryDataClient;

        private Play play;
        private GameLevelData levelData;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            ui.Prepare();
            checker.SetOnComplete(ToNextState);
        }

        public override void Enter()
        {
            play = memoryDataClient.Get<Play>();
            levelData = levelSelector.GetCurrent();
            memoryDataClient.Put(new Round());

            progressionEventsHelper.SendLevelStartedEvent(levelId: levelData.GetIdWithRound(play));
            courtSetupHelper.Spawn(levelData);
            roundEventsHelper.SendBasketHasBeenSpawnedEvent(courtSetupHelper.DistanceToCamera);
            ui.ShowIntro(play, levelData);
            checker.DoStart(levelData, play);
        }

        public override void Exit()
        {
            checker.DoStop();
        }

        #region Debug

        [Button]
        private void Respawn()
        {
            courtSetupHelper.Clear();
            courtSetupHelper.Spawn(levelData);
        }

        #endregion
    }
}
