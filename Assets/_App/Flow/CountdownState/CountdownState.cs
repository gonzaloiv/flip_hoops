using DigitalLove.Casual.Analytics;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Levels;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;
using DigitalLove.Casual.Flow;
using DigitalLove.Audio;

namespace DigitalLove.Game
{
    public class CountdownState : BaseState
    {
        [Header("Scene")]
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private CourtSetupHelper courtSetupHelper;
        [SerializeField] private CountdownStateUI ui;
        [SerializeField] private TheRadioBehaviour theRadioBehaviour;

        [Header("Checker")]
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
            ui.HideAll();
            checker.SetOnComplete(ToNextState);
        }

        public override void Enter()
        {
            play = memoryDataClient.Get<Play>();
            levelData = levelSelector.GetCurrent();
            if (play.Tries == 0)
                theRadioBehaviour.Spawn();
            memoryDataClient.Put(new Round());

            progressionEventsHelper.SendLevelStartedEvent(levelId: levelData.GetIdWithRound(play));
            courtSetupHelper.Spawn(levelData);
            roundEventsHelper.SendBasketHasBeenSpawnedEvent(courtSetupHelper.DistanceToCamera);
            ui.ShowIntro(play);
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
