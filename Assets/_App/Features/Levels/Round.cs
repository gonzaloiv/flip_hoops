using System.Collections.Generic;

namespace DigitalLove.Game
{
    public class Round
    {
        public int score;
        public List<string> events = new();

        public void Reset()
        {
            score = 0;
            events = new();
        }
    }
}