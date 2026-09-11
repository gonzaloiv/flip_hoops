using DigitalLove.Casual.Analytics;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Levels;
using DigitalLove.Global;
using Reflex.Attributes;
using UnityEngine;

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
        private bool isSpawning;

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
            EnsurePlayCursor();
            progressionEventsHelper.SendLevelStartedEvent(levelId: levelData.GetIdWithRound(play));
            ui.SetLevelsInteraction(true);
            ui.SubscribeLevelPressed(OnLevelPressed);
            ui.SubscribeRandomPressed(OnRandomPressed);
            SpawnCourt(OnEnterSpawned);
        }

        public override void Exit()
        {
            ui.UnsubscribeRandomPressed(OnRandomPressed);
            ui.UnsubscribeLevelPressed(OnLevelPressed);
            ui.SetLevelsInteraction(false);
            checker.DoStop();
        }

        private void EnsurePlayCursor()
        {
            if (setRandomLevel.Value)
                levelSelector.SetRandom();
            else if (!levelSelector.HasPlayCursor)
                levelSelector.SeedPlayCursorFromCookies();
            levelData = levelSelector.Current;
        }

        private void SpawnCourt(System.Action onComplete)
        {
            isSpawning = true;
            courtSetupHelper.Spawn(levelData, play, () =>
            {
                isSpawning = false;
                onComplete();
            });
        }

        private void OnEnterSpawned()
        {
            roundEventsHelper.SendBasketHasBeenSpawnedEvent(courtSetupHelper.DistanceToCamera);
            ShowIntroAndArmGrab();
        }

        private void ShowIntroAndArmGrab()
        {
            ui.RefreshLevels(levelSelector, memoryDataClient.Get<PlayerData>());
            ui.ShowIntro(levelSelector.CurrentLevelIndex, levelSelector.TotalLevels, play.Tries);
            checker.DoStart(levelData, levelSelector.CurrentLevelIndex);
        }

        private void OnLevelPressed(string levelId)
        {
            if (isSpawning || !levelSelector.IsPressable(levelId))
                return;
            if (levelSelector.HasPlayCursor && string.Equals(levelSelector.Current.id, levelId))
                return;

            levelSelector.SetPlayCursor(levelId);
            RespawnSelectedLevel();
        }

        private void OnRandomPressed()
        {
            if (isSpawning)
                return;
            levelSelector.SetRandom();
            RespawnSelectedLevel();
        }

        private void RespawnSelectedLevel()
        {
            levelData = levelSelector.Current;
            checker.DoStop();
            courtSetupHelper.Clear();
            SpawnCourt(ShowIntroAndArmGrab);
        }

        #region Debug

        [Button]
        private void Respawn()
        {
            courtSetupHelper.Clear();
            SpawnCourt(ShowIntroAndArmGrab);
        }

        #endregion
    }
}
