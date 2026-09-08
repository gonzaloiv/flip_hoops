using System;
using System.Collections;
using DigitalLove.Game.Analytics;
using DigitalLove.Game.Levels;
using DigitalLove.Global;
using UnityEngine;
using DigitalLove.Casual.Flow;
using DigitalLove.Game.Balls;

namespace DigitalLove.Game
{
    public class CountdownStateChecker : MonoBehaviour
    {
        private const int CountdownSecs = 3;

        [SerializeField] private CountdownStateUI ui;
        [SerializeField] private RoundEventsHelper roundEventsHelper;
        [SerializeField] private BallsSpawner ballSpawner;

        private Action onComplete;
        private GameLevelData levelData;
        private int levelIndex;

        public void SetOnComplete(Action onComplete) => this.onComplete = onComplete;

        public void DoStart(GameLevelData levelData, int levelIndex)
        {
            this.levelData = levelData;
            this.levelIndex = levelIndex;
            ballSpawner.ballGrabbed += OnBallGrabbed;
        }

        public void DoStop()
        {
            ballSpawner.ballGrabbed -= OnBallGrabbed;
        }

        private void OnBallGrabbed()
        {
            ballSpawner.ballGrabbed -= OnBallGrabbed;
            ui.ShowBasketInstructions(levelData, levelIndex);
            ui.HideGrabBallPanel();
            roundEventsHelper.SendHasGrabbedBallEvent();
            StartCoroutine(CountdownRoutine());
        }

        private IEnumerator CountdownRoutine()
        {
            int countdown = CountdownSecs;
            while (countdown > 0)
            {
                ui.ShowCountdown(countdown);
                yield return new WaitForSecondsRealtime(1);
                countdown--;
            }
            ui.ShowCountdown(countdown);
            onComplete();
        }

        #region Debug

        [Button]
        public void InvokeOnBallGrabbed() => OnBallGrabbed();

        #endregion
    }
}
