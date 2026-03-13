using DigitalLove.FlowControl;
using UnityEngine;

namespace DigitalLove.Game
{
    public class BaseState : MonoState
    {
        [SerializeField] private MonoState nextState;

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        protected virtual void ToNextState() => parent.SetCurrentState(nextState.RouteId);
    }
}