using System;
using DigitalLove.Game.Levels;
using UnityEngine;

namespace DigitalLove.Game
{
    public abstract class BaseRoundChecker : MonoBehaviour
    {
        protected Action onComplete;

        public abstract void DoStart(GameLevelData levelData);

        public void SetOnComplete(Action onComplete)
        {
            this.onComplete = onComplete;
        }
    }
}