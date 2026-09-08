using System.Collections.Generic;

namespace DigitalLove.Game
{
    public class Round
    {
        public const int BaseMakePoints = 2;

        private int score;
        private int throws;
        private int remainingMakes;
        private Throw activeThrow;
        private List<string> events = new();

        public int Score => score;
        public int Throws => throws;
        public int RemainingMakes => remainingMakes;

        public int StarsEarned()
        {
            if (throws <= 3)
                return 3;
            if (throws <= 5)
                return 2;
            return 1;
        }

        public void Reset()
        {
            score = 0;
            throws = 0;
            remainingMakes = 0;
            activeThrow = null;
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

        public void SeedRemainingMakes(int count)
        {
            remainingMakes = count;
        }

        public void DecrementRemainingMakes()
        {
            if (remainingMakes > 0)
                remainingMakes--;
        }

        public void AddThrow()
        {
            throws++;
        }

        public void OpenThrow(int basePoints = BaseMakePoints)
        {
            activeThrow = new Throw(basePoints);
        }

        public int ResolveActiveThrow()
        {
            int points = activeThrow != null ? activeThrow.ResolvedPoints : BaseMakePoints;
            activeThrow = null;
            AddScore(points);
            return points;
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
