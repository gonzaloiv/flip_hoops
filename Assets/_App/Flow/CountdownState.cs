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
using DigitalLove.Global;
using DigitalLove.Localization;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalLove.Game
{
    public class CountdownState : BaseState
    {
        private const int CountdownSecs = 3;

        [Header("Scene")]
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private GravitySelector gravitySelector;
        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ThrowZone throwZone;

        [Header("UI")]
        [SerializeField] private string tableName = "Levels";
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;
        [SerializeField] private GrabBallPanel grabBallPanel;
        [SerializeField] private HighestScorePosterBehaviour highestScorePosterBehaviour;
        [SerializeField] private FindTheHoopPanel findTheHoopPanel;
        [SerializeField] private PosterBehaviour[] posters;

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
            basketSpawner.Hide();
        }

        public override void Enter()
        {
            ballSpawner.ballGrabbed += OnBallGrabbed;

            play = memoryDataClient.Get<Play>();
            levelData = levelSelector.GetCurrent();
            memoryDataClient.Put(new Round());

            progressionEventsHelper.SendLevelStartedEvent(levelId: levelData.GetIdWithRound(play));
            Spawn();
            SendBasketHasSpawnEvent();
            ShowUI();
        }

        [Button]
        private void InvokeOnBallGrabbed() => OnBallGrabbed();


        private void OnBallGrabbed()
        {
            ballSpawner.ballGrabbed -= OnBallGrabbed;
            ShowBasketPanel();
            grabBallPanel.Hide();
            roundEventsHelper.SendHasGrabbedBallEvent();
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

        [Button]
        private void Respawn()
        {
            ballSpawner.Unspawn();
            throwZone.Unspawn();
            basketSpawner.Hide();
            Spawn();
        }

        private void Spawn()
        {
            GravityData gravity = gravitySelector.SelectRandom(levelData.gravities);
            throwZone.Spawn(); // ? Basket spawning gets position based on distance to throw zone
            Vector3 gravityDirection = basketSpawner.SpawnAndGetGravityDirection(gravity, throwZone.transform, levelData.distance);
            posters.Spawn(gravityDirection);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(levelData.balls, gravityDirection);
        }

        private void SendBasketHasSpawnEvent()
        {
            float distanceToCamera = Vector3.Distance(basketSpawner.Basket.WorldPosition, Camera.main.transform.position);
            roundEventsHelper.SendBasketHasBeenSpawnedEvent(distanceToCamera);
        }
    }
}