using System.Collections;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Basket;
using DigitalLove.Game.UI;
using Reflex.Attributes;
using UnityEditor;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundState : MonoState
    {
        private const int RoundSecs = 33;

        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ScorePanel scorePanel;
        [SerializeField] private MonoState nextState;

        [Inject] private MemoryDataClient memoryDataClient;

        private Round round;

        public override void Init(StateMachine parent)
        {
            base.Init(parent);
            scorePanel.Hide();
        }

        public override void Enter()
        {
            basketSpawner.scored += OnScored;
            round = new();
            CountDown();
        }

        private void OnScored()
        {
            round.score++;
            scorePanel.Show(round.score);
        }

        private void CountDown()
        {
            int countdown = RoundSecs;
            IEnumerator CoundownRoutine()
            {
                while (countdown > 0)
                {
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                OnCountdownComplete();
            }
            StartCoroutine(CoundownRoutine());
        }

        private void OnCountdownComplete()
        {
            parent.SetCurrentState(nextState.RouteId);
        }

        public override void Exit()
        {
            basketSpawner.scored -= OnScored;

            scorePanel.Hide();
        }
    }
}