using System.Collections;
using DigitalLove.Casual.Analytics;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using DigitalLove.Localization;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class CountdownState : BaseState
    {
        private const int CountdownSecs = 3;

        [Header("Scene")]
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private GravitySpawner gravitiesBehaviour;
        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ThrowZone throwZone;

        [Header("UI")]
        [SerializeField] private string tableName = "Levels";
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;
        [SerializeField] private GrabBallPanel grabBallPanel;
        [SerializeField] private HighestScorePosterBehaviour highestScorePosterBehaviour;
        [SerializeField] private FindTheHoopPanel findTheHoopPanel;

        [Header("Analytics")]
        [SerializeField] private ProgressionEventsHelper progressionEventsHelper;
        [SerializeField] private RoundEventsHelper roundEventsHelper;

        [Inject] private MemoryDataClient memoryDataClient;

        private Play play;
        private GameLevelData levelData;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            grabBallPanel.Hide();
        }

        public override void Enter()
        {
            ballSpawner.ballGrabbed += OnBallGrabbed;

            play = memoryDataClient.Get<Play>();
            levelData = levelSelector.GetCurrent();
            memoryDataClient.Put(new Round());

            progressionEventsHelper.SendLevelStartedEvent(levelId: levelData.GetIdWithRound(play));
            Spawn();
            ShowUI();
        }

        private void OnBallGrabbed()
        {
            ballSpawner.ballGrabbed -= OnBallGrabbed;
            ShowBasketPanel();
            grabBallPanel.Hide();
            roundEventsHelper.SendHasGrabbedBallEvent(levelData.GetIdWithRound(play));
            IEnumerator CountdownRoutine()
            {
                int countdown = CountdownSecs;
                while (countdown > 0)
                {
                    scoreboardSpawner.Panel.SetTime(countdown);
                    basketSpawner.Panel.ShowCountdown(countdown);
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                scoreboardSpawner.Panel.SetTime(countdown);
                basketSpawner.Panel.ShowCountdown(countdown);
                ToNextState();
            }
            StartCoroutine(CountdownRoutine());
        }

        private void ShowBasketPanel()
        {
            string initText;
            string infoText;
            if (levelData.IsWarmUp)
            {
                initText = LocalizationUtil.GetValue(tableName: tableName, levelData.IntroKey);
                infoText = LocalizationUtil.GetValue(tableName: tableName, levelData.InfoKey, levelData.basketsToScore);
            }
            else
            {
                initText = LocalizationUtil.GetValue(tableName: tableName, "high_score_level_init", play.RoundLabelValue());
                infoText = LocalizationUtil.GetValue(tableName: tableName, "high_score_level_info");
            }
            basketSpawner.Panel.Show(initText, infoText);
        }

        private void ShowUI()
        {
            grabBallPanel.Show();
            scoreboardSpawner.ShowRound(play.RoundLabelValue());
            highestScorePosterBehaviour.Show();
            findTheHoopPanel.Show();
        }

        private void Spawn()
        {
            GravityData gravity = gravitiesBehaviour.Spawn(levelData.gravities);
            throwZone.Spawn();
            basketSpawner.Spawn(gravity, throwZone.transform, levelData.distance);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(levelData.balls, gravity);
            roundEventsHelper.SendBasketBeenSpawnedEvent(levelData.GetIdWithRound(play));
        }
    }
}