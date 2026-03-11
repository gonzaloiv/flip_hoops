using System.Collections;
using DigitalLove.FlowControl;
using DigitalLove.Game.Ball;
using DigitalLove.Game.Basket;
using UnityEngine;

namespace DigitalLove.Game
{
    public class CountdownState : MonoState
    {
        private int CountdownStartValue = 3;

        [SerializeField] private BallSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;

        [SerializeField] private MonoState nextState;

        private int countdown;

        public override void Enter()
        {
            IEnumerator CountdownRoutine()
            {
                countdown = CountdownStartValue;
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
