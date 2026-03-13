using System.Collections;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Ball;
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

        [SerializeField] private string tableName = "Levels";
        [SerializeField] private LevelSelector levelSelector;
        [SerializeField] private GravityBehaviour gravitiesBehaviour;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private GameObject grabBallPanel;
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        private Play play;
        private GameLevelData levelData;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            grabBallPanel.SetActive(false);
            scoreboardSpawner.Panel.Hide();
        }

        public override void Enter()
        {
            ballSpawner.ballGrabbed += OnBallGrabbed;

            play = memoryDataClient.Get<Play>();
            levelData = levelSelector.GetCurrent(play.Tries);

            Spawn();
            ShowUI();
        }

        private void ShowUI()
        {
            grabBallPanel.SetActive(true);
            if (!scoreboardSpawner.HasBeenSpawned)
                scoreboardSpawner.Spawn(throwZone.transform);
            scoreboardSpawner.Panel.Show();
            scoreboardSpawner.Panel.SetRound(play.RoundCount());
        }

        private void Spawn()
        {
            GameLevelData levelData = levelSelector.GetCurrent(play.Tries);
            GravityData gravity = gravitiesBehaviour.GetGravity(levelData.gravityFilter);
            throwZone.Spawn();
            basketSpawner.Spawn(gravity, throwZone.transform);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(gravity);
        }

        private void OnBallGrabbed()
        {
            ShowBasketPanel();
            grabBallPanel.SetActive(false);
            IEnumerator CountdownRoutine()
            {
                int countdown = CountdownSecs;
                while (countdown > 0)
                {
                    scoreboardSpawner.Panel.SetTime(countdown);
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                scoreboardSpawner.Panel.SetTime(countdown);
                ToNextState();
            }
            StartCoroutine(CountdownRoutine());
        }

        private void ShowBasketPanel()
        {
            string initText = LocalizationUtil.GetValue(tableName: tableName, levelData.IntroKey);
            if (string.IsNullOrEmpty(initText))
                initText = LocalizationUtil.GetValue(tableName: tableName, "default_round_init", play.RoundCount());
            string infoText = LocalizationUtil.GetValue(tableName: tableName, levelData.InfoKey);
            basketSpawner.Panel.Show(initText, infoText);
        }

        public override void Exit()
        {
            ballSpawner.ballGrabbed -= OnBallGrabbed;
        }
    }

    public static class Extensions
    {
        public static int RoundCount(this Play play) => play.Tries + 1;
    }
}
