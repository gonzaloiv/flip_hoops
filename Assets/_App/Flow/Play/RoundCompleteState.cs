using System.Collections;
using DigitalLove.FlowControl;
using DigitalLove.Game.Ball;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundCompleteState : MonoState
    {
        private int RoundCompleteSecs = 3;

        [SerializeField] private GravityBehaviour gravitiesBehaviour;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private MonoState nextState;

        public override void Enter()
        {
            gravitiesBehaviour.Unspawn();
            ballSpawner.Unspawn();
            basketSpawner.Unspawn();
            CountDown();
        }

        private void CountDown()
        {
            int countdown = RoundCompleteSecs;
            IEnumerator CoundownRoutine()
            {
                while (countdown > 0)
                {
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                parent.SetCurrentState(nextState.RouteId);
            }
            StartCoroutine(CoundownRoutine());
        }

        public override void Exit()
        {

        }
    }
}