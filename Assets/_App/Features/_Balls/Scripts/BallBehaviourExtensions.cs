using System.Collections.Generic;

namespace DigitalLove.Game.Balls
{
    public static class BallBehaviourExtensions
    {
        public static BallBehaviour FindValid(this List<BallBehaviour> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                BallBehaviour ball = balls[i];
                if (ball.IsActive && !ball.HasBeenUnselected)
                    return ball;
            }
            return null;
        }

        public static float TotalVolume(this List<BallBehaviour> balls)
        {
            float total = 0f;
            for (int i = 0; i < balls.Count; i++)
                total += balls[i].Volume;
            return total;
        }
    }
}
