using System.Collections.Generic;

namespace DigitalLove.Game.BankShot
{
    public static class ObligatoryActivatableExtensions
    {
        public static bool AllObligatorySatisfied<T>(this IReadOnlyList<T> activatables)
            where T : class, IObligatoryActivatable
        {
            if (activatables == null || activatables.Count == 0)
                return true;

            for (int i = 0; i < activatables.Count; i++)
            {
                T activatable = activatables[i];
                if (activatable == null || !activatable.IsObligatory)
                    continue;

                if (!activatable.ActivatedThisThrow)
                    return false;
            }

            return true;
        }

        public static bool HasAnyObligatory<T>(this IReadOnlyList<T> activatables)
            where T : class, IObligatoryActivatable
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

        public static void ResetAllForThrow<T>(this IReadOnlyList<T> activatables)
            where T : class, IObligatoryActivatable
        {
            if (activatables == null)
                return;

            for (int i = 0; i < activatables.Count; i++)
            {
                if (activatables[i] != null)
                    activatables[i].ResetForThrow();
            }
        }

        public static bool CanCreditMake<T>(this IReadOnlyList<T> activatables)
            where T : class, IObligatoryActivatable
        {
            if (!activatables.HasAnyObligatory())
                return true;

            return activatables.AllObligatorySatisfied();
        }

        public static bool CanCreditMake<TFirst, TSecond>(
            IReadOnlyList<TFirst> first,
            IReadOnlyList<TSecond> second)
            where TFirst : class, IObligatoryActivatable
            where TSecond : class, IObligatoryActivatable
        {
            return first.CanCreditMake() && second.CanCreditMake();
        }
    }
}
