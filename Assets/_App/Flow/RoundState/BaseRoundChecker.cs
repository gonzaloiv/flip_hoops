using System;
using UnityEngine;

namespace DigitalLove.Game
{
    public abstract class BaseRoundChecker : MonoBehaviour
    {
        protected Action onComplete;

        public abstract void DoStart();

        public void SetOnComplete(Action onComplete)
        {
            this.onComplete = onComplete;
        }
    }
}