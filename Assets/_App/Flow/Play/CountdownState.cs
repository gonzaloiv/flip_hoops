using System.Collections;
using System.Linq;
using DigitalLove.FlowControl;
using DigitalLove.Game.Ball;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using UnityEngine;

namespace DigitalLove.Game
{
    public class CountdownState : MonoState
    {
        private const int CountdownSecs = 3;

        [SerializeField] private GravityBehaviour gravitiesBehaviour;
        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private MonoState nextState;

        public override void Enter()
        {
            GravityData gravity = gravitiesBehaviour.SpawnRandomGravity();
            SpawnCourt(gravity);
            ballSpawner.Spawn(gravity);
            CountDown();
        }

        private void SpawnCourt(GravityData gravity)
        {
            throwZone.Spawn();
            basketSpawner.Spawn(gravity, throwZone.transform);
            throwZone.SetReference(basketSpawner.Basket.transform);
        }

        private void CountDown()
        {
            IEnumerator CountdownRoutine()
            {
                int countdown = CountdownSecs;
                while (countdown > 0)
                {
                    yield return new WaitForSecondsRealtime(1);
                    countdown--;
                }
                parent.SetCurrentState(nextState.RouteId);
            }
            StartCoroutine(CountdownRoutine());
        }

        public override void Exit()
        {

        }
    }
}
