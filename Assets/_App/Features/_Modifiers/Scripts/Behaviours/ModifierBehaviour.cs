using System;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class ModifierBehaviour : MonoBehaviour
    {
        protected Action<int> scored;

        public virtual void Init(Transform throwZoneTransform, Transform basketTransform, Action<int> scored)
        {
            this.scored = scored;
        }

        public virtual void ShowScore(int score, bool hasMultiplier)
        {
            // ? Optional to override
        }
    }
}