using System.Collections;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.FlowControl;
using DigitalLove.Game.Ball;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game
{
    public class RoundCompleteState : BaseState
    {
        private int RoundCompleteSecs = 5;

        [SerializeField] private GravityBehaviour gravitiesBehaviour;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;

        [Inject] private MemoryDataClient memoryDataClient;

        public override void Enter()
        {
            ballSpawner.Unspawn();
            CountRound();
            CountDown();
        }

        private void CountRound()
        {
            Play play = memoryDataClient.Get<Play>();
            play.IncreaseTries();
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
                ToNextState();
            }
            StartCoroutine(CoundownRoutine());
        }

        protected override void ToNextState()
        {
            throwZone.Unspawn();
            gravitiesBehaviour.Unspawn();
            basketSpawner.Unspawn();
            base.ToNextState();
        }

        public override void Exit()
        {

        }
    }
}