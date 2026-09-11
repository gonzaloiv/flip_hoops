using System.Collections.Generic;

namespace DigitalLove.Game.Obstacles
{
    public static class ObstacleBehaviourExtensions
    {
        public static bool AllObligatorySatisfied(this IReadOnlyList<ObstacleBehaviour> obstacles)
        {
            if (obstacles == null || obstacles.Count == 0)
                return true;

            for (int i = 0; i < obstacles.Count; i++)
            {
                ObstacleBehaviour obstacle = obstacles[i];
                if (obstacle == null || !obstacle.IsObligatory)
                    continue;

                if (!obstacle.ActivatedThisThrow)
                    return false;
            }

            return true;
        }

        public static bool HasAnyObligatory(this IReadOnlyList<ObstacleBehaviour> obstacles)
        {
            if (obstacles == null)
                return false;

            for (int i = 0; i < obstacles.Count; i++)
            {
                if (obstacles[i] != null && obstacles[i].IsObligatory)
                    return true;
            }

            return false;
        }

        public static void ResetAllForThrow(this IReadOnlyList<ObstacleBehaviour> obstacles)
        {
            if (obstacles == null)
                return;

            for (int i = 0; i < obstacles.Count; i++)
            {
                if (obstacles[i] != null)
                    obstacles[i].ResetForThrow();
            }
        }
    }
}
