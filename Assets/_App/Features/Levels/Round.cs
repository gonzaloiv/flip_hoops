using System.Collections.Generic;

namespace DigitalLove.Game
{
    public class Round
    {
        private int score;
        private List<string> events = new();

        public int Score => score;

        public void Reset()
        {
            score = 0;
            events = new();
        }

        public void AddScore(int score = 1)
        {
            this.score += score;
        }

        public void SetScore(int score)
        {
            this.score = score;
        }

        public void AddEvent(string eventName)
        {
            events.Add(eventName);
        }

        public bool HasEvent(string eventName)
        {
            return events.Contains(eventName);
        }
    }
}