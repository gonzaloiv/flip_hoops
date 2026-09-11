using System.Collections.Generic;

namespace DigitalLove.Game.Modifiers
{
    public static class ModifierBehaviourExtensions
    {
        public static bool AllObligatorySatisfied(this IReadOnlyList<ModifierBehaviour> modifiers)
        {
            if (modifiers == null || modifiers.Count == 0)
                return true;

            for (int i = 0; i < modifiers.Count; i++)
            {
                ModifierBehaviour modifier = modifiers[i];
                if (modifier == null || !modifier.IsObligatory)
                    continue;

                if (!modifier.ActivatedThisThrow)
                    return false;
            }

            return true;
        }

        public static bool HasAnyObligatory(this IReadOnlyList<ModifierBehaviour> modifiers)
        {
            if (modifiers == null)
                return false;

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i] != null && modifiers[i].IsObligatory)
                    return true;
            }

            return false;
        }

        public static void ResetAllForThrow(this IReadOnlyList<ModifierBehaviour> modifiers)
        {
            if (modifiers == null)
                return;

            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i] != null)
                    modifiers[i].ResetForThrow();
            }
        }
    }
}
