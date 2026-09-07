using System.Collections.Generic;
using UnityEngine;

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

        public static BallBehaviour PickRandomInactiveWithId(
            this List<BallBehaviour> balls,
            List<string> validIds)
        {
            int matchCount = CountInactiveWithId(balls, validIds);
            int pick = Random.Range(0, matchCount);
            return FindInactiveWithIdAt(balls, validIds, pick);
        }

        private static int CountInactiveWithId(List<BallBehaviour> balls, List<string> validIds)
        {
            int count = 0;
            for (int i = 0; i < balls.Count; i++)
            {
                if (IsInactiveWithId(balls[i], validIds))
                    count++;
            }
            return count;
        }

        private static BallBehaviour FindInactiveWithIdAt(
            List<BallBehaviour> balls,
            List<string> validIds,
            int pick)
        {
            int index = 0;
            for (int i = 0; i < balls.Count; i++)
            {
                BallBehaviour ball = balls[i];
                if (!IsInactiveWithId(ball, validIds))
                    continue;
                if (index == pick)
                    return ball;
                index++;
            }
            return null;
        }

        private static bool IsInactiveWithId(BallBehaviour ball, List<string> validIds)
        {
            return !ball.IsActive && validIds.Contains(ball.Data.id);
        }
    }
}
