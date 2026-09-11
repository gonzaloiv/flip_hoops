using System.Collections.Generic;

namespace DigitalLove.Game.BankShot
{
    public static class ObligatoryActivatableExtensions
    {
        public static bool AllObligatorySatisfied(
            this IReadOnlyList<IObligatoryActivatable> activatables)
        {
            if (activatables == null || activatables.Count == 0)
                return true;

            for (int i = 0; i < activatables.Count; i++)
            {
                IObligatoryActivatable activatable = activatables[i];
                if (activatable == null || !activatable.IsObligatory)
                    continue;

                if (!activatable.ActivatedThisThrow)
                    return false;
            }

            return true;
        }

        public static bool HasAnyObligatory(
            this IReadOnlyList<IObligatoryActivatable> activatables)
        {
            if (activatables == null)
                return false;

            for (int i = 0; i < activatables.Count; i++)
            {
                if (activatables[i] != null && activatables[i].IsObligatory)
                    return true;
            }

            return false;
        }

        public static void ResetAllForThrow(
            this IReadOnlyList<IObligatoryActivatable> activatables)
        {
            if (activatables == null)
                return;

            for (int i = 0; i < activatables.Count; i++)
            {
                if (activatables[i] != null)
                    activatables[i].ResetForThrow();
            }
        }

        public static bool CanCreditMake(
            this IReadOnlyList<IObligatoryActivatable> activatables)
        {
            if (!activatables.HasAnyObligatory())
                return true;

            return activatables.AllObligatorySatisfied();
        }

        public static bool CanCreditMake(
            IReadOnlyList<IObligatoryActivatable> first,
            IReadOnlyList<IObligatoryActivatable> second)
        {
            return first.CanCreditMake() && second.CanCreditMake();
        }
    }
}
