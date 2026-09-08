using System;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    [Serializable]
    public class ScoredEventArgs
    {
        public int score;
        public Transform panelRef;

        public ScoredEventArgs(int score, Transform panelRef)
        {
            this.score = score;
            this.panelRef = panelRef;
        }
    }
}