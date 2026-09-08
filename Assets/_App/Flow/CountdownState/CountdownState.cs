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

        [Header("Checker")]
        [SerializeField] private CountdownStateChecker checker;

        [Header("Analytics")]
        [SerializeField] private ProgressionEventsHelper progressionEventsHelper;
        [SerializeField] private RoundEventsHelper roundEventsHelper;

        [Inject] private MemoryDataClient memoryDataClient;

        private Play play;
        private GameLevelData levelData;

        [Header("Debug")]
        [SerializeField] private DebugBool setRandomLevel;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            ui.HideAll();
            checker.SetOnComplete(ToNextState);
            courtSetupHelper.Init();
        }

        public override void Enter()
        {
            play = memoryDataClient.Get<Play>();
            memoryDataClient.Put(new Round());
            levelData = GetLevelData();
            progressionEventsHelper.SendLevelStartedEvent(levelId: levelData.GetIdWithRound(play));
            courtSetupHelper.Spawn(levelData, play, OnSpawned);
        }

        private GameLevelData GetLevelData()
        {
            if (setRandomLevel.Value)
            {
                levelSelector.SetRandom();
            }
            else
            {
                levelSelector.SetCurrentPlayerLevelId();
            }
            return levelSelector.Current;
        }

        private void OnSpawned()
        {
            roundEventsHelper.SendBasketHasBeenSpawnedEvent(courtSetupHelper.DistanceToCamera);
            ui.ShowIntro(levelSelector.CurrentLevelIndex, levelSelector.TotalLevels);
            checker.DoStart(levelData, levelSelector.CurrentLevelIndex);
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
            courtSetupHelper.Spawn(levelData, play, OnSpawned);
        }

        #endregion
    }
}
